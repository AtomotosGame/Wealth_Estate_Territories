using UnityEngine;
using System.Collections;

public class CompassDirectioninf : MonoBehaviour {
    [Header("Camera Stuffs")]
    public Controller CTRL_S;
    public WeaponControl Weap_Script;
    public Action_Phone Phone_Script;
    [HideInInspector] public GameObject cam;
    public Transform AimBlockracker;
    public float AimTrackDistance;
    public bool AimBlock;
	Vector3 currentLookPos;
    [HideInInspector] public Camera Mcamera;

	[System.Serializable]
	public class OtherSettings
	{
		public float lookSpeed = 5.0f;
		public float lookDistance = 30.0f;
		public bool requireInputForTurn = true;
		public LayerMask aimDetectionLayers;
	}
	[SerializeField]
	public OtherSettings other;
	// Use this for initialization
	void Start () {
            cam = CTRL_S.cam;
            Mcamera = CTRL_S.Mcamera;
	}

    // Update is called once per frame
    void Update()
    {
        Weap_Script.BlockedAim = AimBlock;
        if (CTRL_S.Aim)
        {
            AimTracker();
        }
        else
        {
            AimBlock = false;
        }
        if (CTRL_S.Aim)
        {
            CharacterLook();
        }
        else {
            if (Phone_Script.TakingPicture) {
                CharacterLook();
            }
        }
    }

	public void CharacterLook()
	{
			Transform mainCamT = Mcamera.transform;
			Transform pivotT = mainCamT.parent;
			Vector3 pivotPos = pivotT.position;
			Vector3 lookTarget = pivotPos + (pivotT.forward * other.lookDistance);
			Vector3 thisPos = transform.position;
			Vector3 lookDir = lookTarget - thisPos;
			Quaternion lookRot = Quaternion.LookRotation (lookDir);
			//lookRot.x = 0;
			//lookRot.z = 0;
			Quaternion newRotation = Quaternion.Lerp (transform.rotation, lookRot, Time.deltaTime * other.lookSpeed);
			transform.rotation = newRotation;
	}

    public void AimTracker()
    {
            Ray ray = new Ray(AimBlockracker.position, AimBlockracker.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, AimTrackDistance))
            {
                AimBlock = true;
            }
            else
            {
                AimBlock = false;
            }
    }


}
