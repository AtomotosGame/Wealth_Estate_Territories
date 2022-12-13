using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
    public bool InputOn;
    public AudioSource HelicopterSound;
    ControlPanel ControlPanel;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;
    public Transform GroundDetector;
    public float TurnForce = 3f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 100f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 1.3f;

    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    public bool IsOnGround = true;
    public float MaxBladeSpeed = 50;
    public ParticleSystem UnderFog;
    bool fogOn;
    [HideInInspector] public float currentBladeSpeed;
    Ray GRRay;
    RaycastHit GRHit;

    // Use this for initialization
    void Start()
    {
        ControlPanel = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<ControlPanel>();
        ControlPanel.KeyPressed += OnKeyPressed;
    }

    void Update()
    {
        GroundedCheck();
    }

    void FixedUpdate()
    {
        if (InputOn)
        {
            LiftProcess();
            MoveProcess();
            TiltProcess();

            if (currentBladeSpeed < MaxBladeSpeed)
            {
                currentBladeSpeed += 0.1f;
            }
            else
            {
                currentBladeSpeed -= 0.1f;
            }
        }
        else
        {
            if (currentBladeSpeed > 0)
            {
                currentBladeSpeed -= 0.1f;
            }
        }
        if (UnderFog)
        {
            if (fogOn)
            {
                if (currentBladeSpeed > 10)
                {
                    UnderFog.transform.position = GRHit.point;
                    UnderFog.Emit(1);
                }
            }
        }
        currentBladeSpeed = Mathf.Clamp(currentBladeSpeed, 0, MaxBladeSpeed);
        MainRotorController.RotarSpeed = currentBladeSpeed * 80;
        SubRotorController.RotarSpeed = currentBladeSpeed * 40;
        HelicopterSound.pitch = Mathf.Clamp(currentBladeSpeed / 40, 0, 1.2f);
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
    }

    private void LiftProcess()
    {
        var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    private void OnKeyPressed(PressedKeyCode[] obj)
    {
        if (InputOn)
        {
            float tempY = 0;
            float tempX = 0;

            // stable forward
            if (hMove.y > 0)
                tempY = -Time.fixedDeltaTime;
            else
                if (hMove.y < 0)
                tempY = Time.fixedDeltaTime;

            // stable lurn
            if (hMove.x > 0)
                tempX = -Time.fixedDeltaTime;
            else
                if (hMove.x < 0)
                tempX = Time.fixedDeltaTime;


            foreach (var pressedKeyCode in obj)
            {
                switch (pressedKeyCode)
                {
                    case PressedKeyCode.SpeedUpPressed:

                        EngineForce += 0.2f;
                        break;
                    case PressedKeyCode.SpeedDownPressed:

                        EngineForce -= 1.3f;
                        if (EngineForce < 0) EngineForce = 0;
                        break;

                    case PressedKeyCode.ForwardPressed:

                        if (IsOnGround) break;
                        tempY = Time.fixedDeltaTime;
                        break;
                    case PressedKeyCode.BackPressed:

                        if (IsOnGround) break;
                        tempY = -Time.fixedDeltaTime;
                        break;
                    case PressedKeyCode.LeftPressed:

                        if (IsOnGround) break;
                        tempX = -Time.fixedDeltaTime;
                        break;
                    case PressedKeyCode.RightPressed:

                        if (IsOnGround) break;
                        tempX = Time.fixedDeltaTime;
                        break;
                    case PressedKeyCode.TurnRightPressed:
                        {
                            if (IsOnGround) break;
                            var force = (turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass;
                            HelicopterModel.AddRelativeTorque(0f, force, 0);
                        }
                        break;
                    case PressedKeyCode.TurnLeftPressed:
                        {
                            if (IsOnGround) break;

                            var force = -(turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass;
                            HelicopterModel.AddRelativeTorque(0f, force, 0);
                        }
                        break;

                }
            }

            hMove.x += tempX;
            hMove.x = Mathf.Clamp(hMove.x, -1, 1);

            hMove.y += tempY;
            hMove.y = Mathf.Clamp(hMove.y, -1, 1);

        }
        else {
            EngineForce = 0;
        }
    }

    void GroundedCheck()
    {
        GRRay = new Ray(GroundDetector.position, GroundDetector.forward);


        if (Physics.Raycast(GRRay, out GRHit, 1f))
        {
            IsOnGround = true;

        }
        else
        {
            IsOnGround = false;
        }

        if (Physics.Raycast(GRRay, out GRHit, 15f))
        {
            fogOn = true;

        }
        else
        {
            fogOn = false;
        }
    }
}