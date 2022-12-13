using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraRig : MonoBehaviour
{
    public Transform target;
    public bool autoTargetPlayer;
    public LayerMask wallLayers;
    GlobalObjects Global_Script;
    [HideInInspector] public float ShakeTimer;
    [HideInInspector] public float ShakeAmount;
    [HideInInspector] public float MoveSpeed = 0.2f;

    public enum Shoulder
    {
        Right, Left
    }
    public Shoulder shoulder;

    [System.Serializable]
    public class CameraSettings
    {
        [Header("-Positioning-")]
        public Vector3 camPositionOffsetLeft;
        public Vector3 camPositionOffsetRight;

        [Header("-Camera Options-")]
		public Camera UICamera;
        public float mouseXSensitivity = 5.0f;
        public float mouseYSensitivity = 5.0f;
        public float minAngle = -30.0f;
        public float maxAngle = 70.0f;
        public float rotationSpeed = 5.0f;
        public float maxCheckDist = 0.1f;

        [Header("-Zoom-")]
        public float fieldOfView = 70.0f;
		public float zoomFieldOfView = 37f;
        public float zoomSpeed = 3.0f;

        [Header("-Visual Options-")]
        public float hideMeshWhenDistance = 0.5f;

        [Header("Car Camera Options")]
        [SerializeField]
        public float rotationDamping;
    }
    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class InputSettings
    {
        public string verticalAxis = "Mouse X";
        public string horizontalAxis = "Mouse Y";
        public string aimButton = "Fire2";
        public string switchShoulderButton = "Fire4";
    }
    [SerializeField]
    public InputSettings input;

    [System.Serializable]
    public class MovementSettings
    {
        public float movementLerpSpeed = 5.0f;
    }
    [SerializeField]
    public MovementSettings movement;
    
    float newX = 0.0f;
	float newY = 0.0f;

    public Camera mainCamera { get; protected set; }
    public Transform pivot { get; set; }

    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
        pivot = transform.GetChild(0);
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            if (Application.isPlaying)
            {
                if (Global_Script.LocalPlayer.GetComponent<Controller>().GetInput == true)
                {
                    //Only rotate if
                    ShakeManager();
                    RotateCamera();
                    if (Global_Script.LocalPlayer.GetComponent<Controller>().ViewPoint == CameraMode.TPS)
                    {
                        CheckWall();
                        CheckMeshRenderer();
                    }
                    if (!Global_Script.LocalPlayer.GetComponent<Controller>().FPSMode)
                    {
                        Zoom(Input.GetButton(input.aimButton));
                    }

                    if (Input.GetButtonDown(input.switchShoulderButton) && Input.GetMouseButton(1))
                    {
                        SwitchShoulders();
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        if (!target)
        {
            TargetPlayer();
        }
        else
        {
            Vector3 targetPostion = target.position;
            Quaternion targetRotation = target.rotation;

            FollowTarget(targetPostion, targetRotation);
            CarCamFol();
        }
    }


    //Finds the plater gameObject and sets it as target
    void TargetPlayer()
    {
        if (autoTargetPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player)
            {
                Transform playerT = player.transform;
                target = playerT;
            }
        }
    }

    //Following the target with Time.deltaTime smoothly
    void FollowTarget(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (!Application.isPlaying) {
			transform.position = targetPosition;
			transform.rotation = targetRotation;
		} else {
            //Vector3 newPos = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * movement.movementLerpSpeed);
            //transform.position = newPos;
            transform.position = targetPosition;
        }
    }


    void CarCamFol()
    {

        if (Global_Script.LocalPlayer != null)
        {
            if (Global_Script.LocalPlayer.GetComponent<Controller>().InCar)
            {
                var currentRotationAngle = transform.eulerAngles.y;
                var wantedRotationAngle = target.eulerAngles.y;
                // Damp the rotation around the y-axis
                currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, cameraSettings.rotationDamping * Time.deltaTime);

                // Convert the angle into a rotation
                var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

                transform.rotation = currentRotation;
            }
        }
    }

    //Rotates the camera with input
    void RotateCamera()
    {
        if (!pivot)
            return;

        newX += cameraSettings.mouseXSensitivity * Input.GetAxis(input.verticalAxis);
        newY += cameraSettings.mouseYSensitivity * Input.GetAxis(input.horizontalAxis);

        Vector3 eulerAngleAxis = new Vector3();
        eulerAngleAxis.x = newY;
        eulerAngleAxis.y = newX;

        newX = Mathf.Repeat(newX, 360);
        newY = Mathf.Clamp(newY, cameraSettings.minAngle, cameraSettings.maxAngle);

        Quaternion newRotation = Quaternion.Slerp(pivot.localRotation, Quaternion.Euler(eulerAngleAxis), Time.deltaTime * cameraSettings.rotationSpeed);

        pivot.localRotation = newRotation;
    }

    //Checks the wall and moves the camera up if we hit
    void CheckWall()
    {
        if (!pivot || !mainCamera)
            return;

        RaycastHit hit;

        Transform mainCamT = mainCamera.transform.parent.parent;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 pivotPos = pivot.position;

        Vector3 start = pivotPos;
        Vector3 dir = mainCamPos - pivotPos;

        float dist = Mathf.Abs(shoulder == Shoulder.Left ? cameraSettings.camPositionOffsetLeft.z : cameraSettings.camPositionOffsetRight.z);

        if(Physics.SphereCast(start, cameraSettings.maxCheckDist, dir, out hit, dist, wallLayers))
        {
            MoveCamUp(hit, pivotPos, dir, mainCamT);
        }
        else
        {
            switch (shoulder)
            {
                case Shoulder.Left:
                    PostionCamera(cameraSettings.camPositionOffsetLeft);
                    break;
                case Shoulder.Right:
                    PostionCamera(cameraSettings.camPositionOffsetRight);
                    break;
            }
        }
    }

    void ShakeManager()
    {
        Transform mainCamT = mainCamera.transform.parent;
        if (ShakeTimer >= 0)
        {
            Vector2 shakePos = Random.insideUnitCircle * ShakeAmount;
            mainCamT.position = new Vector3(mainCamT.position.x + shakePos.x, mainCamT.position.y + shakePos.y, mainCamT.position.z);
            ShakeTimer -= Time.deltaTime;
        }
        else
        {
            mainCamT.localPosition = Vector3.Lerp(mainCamT.localPosition, new Vector3(0, 0, 0), MoveSpeed);
        }
    }

    public void ShakeCamera(float ShakePwr, float ShakeDur)
    {
        ShakeAmount = ShakePwr;
        ShakeTimer = ShakeDur;
    }

    //This moves the camera forward when we hit a wall
    void MoveCamUp(RaycastHit hit, Vector3 pivotPos, Vector3 dir, Transform cameraT)
    {
        float hitDist = hit.distance;
        Vector3 sphereCastCenter = pivotPos + (dir.normalized * hitDist);
        cameraT.position = sphereCastCenter;
    }

    //Postions the cameras localPosition to a given location
    void PostionCamera(Vector3 cameraPos)
    {
        if (!mainCamera)
            return;

        Transform mainCamT = mainCamera.transform.parent.parent;
        Vector3 mainCamPos = mainCamT.localPosition;
        Vector3 newPos = Vector3.Lerp(mainCamPos, cameraPos, Time.deltaTime * movement.movementLerpSpeed);
        mainCamT.localPosition = newPos;
    }

    //Hides the mesh targets mesh renderers when too close
    void CheckMeshRenderer()
    {
        if (!mainCamera || !target)
            return;

        SkinnedMeshRenderer[] meshes = target.GetComponentsInChildren<SkinnedMeshRenderer>();
        Transform mainCamT = mainCamera.transform.parent;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 targetPos = target.position;
        float dist = Vector3.Distance(mainCamPos, (targetPos + target.up));

        if(meshes.Length > 0)
        {
            for(int i = 0; i < meshes.Length; i++)
            {
                if(dist <= cameraSettings.hideMeshWhenDistance)
                {
                    meshes[i].enabled = false;
                }
                else
                {
                    meshes[i].enabled = true;
                }
            }
        }
    }

    //Zooms the camera in and out
    void Zoom(bool isZooming)
    {
        if (!mainCamera)
            return;

        if (isZooming)
        {
            float newFieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSettings.zoomFieldOfView, Time.deltaTime * cameraSettings.zoomSpeed);
            mainCamera.fieldOfView = newFieldOfView;

			if (cameraSettings.UICamera != null) {
				cameraSettings.UICamera.fieldOfView = newFieldOfView;
			}
        }
        else
        {
            float originalFieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSettings.fieldOfView, Time.deltaTime * cameraSettings.zoomSpeed);
            mainCamera.fieldOfView = originalFieldOfView;

			if (cameraSettings.UICamera != null) {
				cameraSettings.UICamera.fieldOfView = originalFieldOfView;
			}
        }
    }

    //Switches the cameras shoulder view
    public void SwitchShoulders()
    {
        switch (shoulder)
        {
            case Shoulder.Left:
                shoulder = Shoulder.Right;
                break;
            case Shoulder.Right:
                shoulder = Shoulder.Left;
                break;
        }
    }
}
