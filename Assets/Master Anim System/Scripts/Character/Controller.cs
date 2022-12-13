using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CameraMode
{
    TPS,
    FPS
}

public class Controller : MonoBehaviour {
    [System.Serializable]
    public class OtherSettings
    {
        public float lookSpeed = 10.0f;
        public float lookDistance = 30.0f;
        public bool requireInputForTurn = true;
        public LayerMask aimDetectionLayers;
    }

    [Header("Base Editor")]
    public CameraMode ViewPoint;
    Animator anim;
    CharacterHieght CH_SCRPT;
    Action_Phone Phone_SCRPT;
    Action_Car Car_SCRPT;
    AnimatorStateInfo stateInfo;
    WeaponControl WP_CTRL;
    Health Health_script;
    public bool GetInput;
    public bool InMenu;

    [Header("Animation States")]
    public float Speed;
    public float Dir;
    public float Turn;
    float TurnInt;
    public float ParkourID;
    public bool Aim;
    public bool Attack;
    public bool Agressive;
    public bool InCover;
    public bool Sprint;
    bool DisableSprint;
    public bool Swim;
    public bool Sitting;
    public bool Sleeping;
    public bool Pissing;
    public bool Parkour;
    public bool jog;
    public bool Crouch;
    public bool UsingPhone;
    public bool InCar;
    public bool Falling;
    public bool Skydiving;
    public bool Parachuting;
    public int ActionID;
    public bool Ladder;
    [HideInInspector] public float LadderStart;
    [HideInInspector] public Transform LadderPos;
    [HideInInspector] public Transform LadderHandPos;
    [HideInInspector] public Transform LadderFootPos;
    float allDir;
    Vector3 cross;
    Vector3 crossInt;
    [HideInInspector] public bool AllowSkydive = false;


    [Header("Variable Editor")]
    [SerializeField]
    public OtherSettings other;
    public GameObject cam;
    public Camera Mcamera;
    public Transform FPSCamPos;
    [HideInInspector] public bool FPSMode = false;
    public GameObject[] HideFPS;
    public bool checkAngle;
    public float AngleDifference;
    public GameObject WaterSplash;

    [Header("Parkour Editor")]
    public bool PHigh;
    public bool PLow;
    public bool PStep;
    public bool PDense;
    public bool PJump;
    public Transform ParkourDetector;
    public Transform ParkourDetectorTop;
    public Transform ParkourDetectorDense;
    public Transform ParkourDetectorStep;
    public Transform ParkourDetectorJump;
    public float prkrdtector = 0.4f;
    [HideInInspector] public float VerticalVelocity;
    [HideInInspector] public float gravity = 19.0f;

    [Header("Cover Editor")]
    public LayerMask CastLayer; // casting layers / detecting layers
    bool CoverDhukse; // Local variable to check if player entered cover
    public float cvrdCloseDist = 2f; // value of cover when its counted as close call
    public bool Cover; // covering sit
    public bool CoverOutLeft; // Can go left while cover
    public bool CoverOutRight; // can go right while cover
    public bool BlindFire; // perform blind fire
    public float cvrdtector = 4f; // Detecting distance for cover
    public Transform CoverDetector; // Lower Cover Detector
    public Transform CoverDetectorLeft; // Lower cover detector left
    public Transform CoverDetectorRight; // Lower cover detector right

    // Match Track
    Vector3 m_Target = new Vector3();
    Vector3 c_Target = new Vector3();
    private Vector3 dir;
    bool ShouldDie = false;


    Ray VaultRay;
    RaycastHit VaultHit;
    Ray ClimbRay;
    RaycastHit ClimbHit;
    Ray DenseRay;
    RaycastHit DenseHit;
    Ray StepRay;
    RaycastHit StepHit;
    Ray Coverray;
    RaycastHit Coverhit;

    private const float m_VaultMatchTargetStart = 0.06f;
    private const float m_VaultMatchTargetStop = 0.47f;
    private const float m_ClimbMatchTargetStart = 0.18f;
    private const float m_ClimbMatchTargetStop = 0.35f;
    private const float m_StepMatchTargetStart = 0.05f;
    private const float m_StepMatchTargetStop = 0.30f;
    private const float m_CoverMatchTargetStart = 0.05f;
    private const float m_CoverMatchTargetStop = 0.50f;
    private const float m_LadderSMatchTargetStart = 0.05f;
    private const float m_LadderSMatchTargetStop = 0.40f;
    private const float m_LadderMatchTargetStart = 0.30f;
    private const float m_LadderMatchTargetStop = 0.70f;

    [HideInInspector] public Transform AutoTarget;
    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
        WP_CTRL = GetComponent<WeaponControl>();
        Phone_SCRPT = GetComponent<Action_Phone>();
        Car_SCRPT = GetComponent<Action_Car>();
        CH_SCRPT = GetComponent<CharacterHieght>();
        Health_script = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Health_script.alive)
        {
            if (GetInput)
            {
                InputControls();
                AutoAimMelee();
                if (!InCar)
                {
                    CoverInput();
                }
                if (!InMenu)
                {
                    if (WP_CTRL.CurrentSlot != 0)
                    {
                        if (!InCover)
                        {
                            AbleToAim();
                            AbleToShoot();
                        }
                        else
                        {
                            CoverFire();
                        }
                    }
                    else
                    {
                        Aim = false;
                        AbleToMelee();
                    }
                    if (!Aim)
                    {
                        if (!Swim && !Ladder)
                        {
                            AbleToBrowseWeapon();
                        }
                        else
                        {
                            WP_CTRL.CurrentSlot = 0;
                        }
                        Aim = false;
                        if (!InCover)
                        {
                            if (WP_CTRL.CurrentSlot != 0)
                            {
                                Attack = false;
                            }
                            else
                            {
                                AbleToMelee();
                            }
                        }
                    }
                    ParkourSystem();
                }
                else
                {
                    Aim = false;
                    WP_CTRL.CurrentSlot = 0;
                }
                CharacterLook();
                ParkourDetect();
            }
        }
        else {
            Aim = false;
            Attack = false;
            Agressive = false;
            InCover = false;
            Ladder = false;
            Swim = false;
        }
        GroundedCheck();
        ApplyStates();
        ProcessMatchTarget();
        DetectRotationX();
        LadderSystem();

        if (!InCar)
        {
            if (Cover)
            {
                CoverSystem();
            }
            else
            {
                InCover = false;
            }

            if (Cover && !CoverDhukse)
            {
                // if in cover but didnt go to cover
                if (anim.GetFloat("BZwep") == 0 && !Coverhit.collider.GetComponent<MeshCollider>())
                {
                    CoverDhukse = true;
                    anim.SetTrigger("TakeCover");
                }
            }
            if (!Cover && CoverDhukse)
            {
                if (anim.GetFloat("BZwep") == 0)
                {
                    CoverDhukse = false;
                    GetOutOfCover();
                }
            }
        }

        WaterSplash.SetActive(Swim);
    }

    void ApplyStates() {
        anim.SetFloat("TurnAngle", Turn, 0.7f, Time.deltaTime);
        if (Aim || Ladder)
        {
            anim.SetFloat("SPD", Speed, 0.1f, Time.deltaTime);
        }
        else {
            anim.SetFloat("SPD", Speed, 0.7f, Time.deltaTime);
        }
        anim.SetFloat("DIR", Dir, 0.1f, Time.deltaTime);
        anim.SetBool("Aim", Aim);
        anim.SetBool("Fire", Attack);
        anim.SetBool("Melee", Agressive);
        anim.SetBool("Sprint", Sprint);
        anim.SetBool("Jump", Parkour);
        anim.SetFloat("JumpID", ParkourID);
        anim.SetBool("Crouch", Crouch);
        anim.SetInteger("WeaponSlot", WP_CTRL.CurrentSlot);
        anim.SetFloat("WeapStyle", WP_CTRL.CurrentSlot);
        anim.SetBool("InCar", InCar);
        anim.SetBool("DoorOpened", Car_SCRPT.DoorOpened);
        anim.SetFloat("SeatSide", Car_SCRPT.SeatSide);
        anim.SetFloat("SeatID", Car_SCRPT.SeatID);
        anim.SetBool("Sitting", Sitting);
        anim.SetBool("Fall", Falling);
        anim.SetBool("UsingCam", Phone_SCRPT.TakingPicture);
        anim.SetBool("InCover", InCover);
        anim.SetFloat("AngleDir", (TurnInt) + 45);
        anim.SetBool("Swimming", Swim);
        anim.SetBool("Sleeping", Sleeping);
        anim.SetBool("Pissing", Pissing);
        anim.SetFloat("ActionID", ActionID);
        anim.SetBool("Skydive", Skydiving);
        anim.SetBool("Parachute", Parachuting);
        anim.SetBool("Ladder", Ladder);
        anim.SetFloat("LadderStart", LadderStart);
    }

    public void InputControls() {
        foreach (GameObject disaFPS in HideFPS)
        {
            if (Car_SCRPT.MySit)
            {
                disaFPS.SetActive(true);
            }
            else
            {
                disaFPS.SetActive(!FPSMode);
            }
        }

        if (!InCar)
        {
            if (Input.GetButtonDown("SwitchView") && !Aim) {
                if (ViewPoint == CameraMode.TPS)
                {
                    ViewPoint = CameraMode.FPS;
                    FPSMode = true;
                }
                else {
                    ViewPoint = CameraMode.TPS;
                    FPSMode = false;
                }
            }

            if (Aim || Phone_SCRPT.TakingPicture || InCover || Ladder)
            {
                if (!Sprint)
                {
                    Speed = Input.GetAxis("Vertical");
                    Dir = Input.GetAxis("Horizontal");
                }
                else
                {
                    Speed = Input.GetAxis("Vertical") * 2;
                    Dir = Input.GetAxis("Horizontal") * 2;
                }
            }
            else
            {
                if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") > 0)
                {
                    if (jog && Sprint || !jog && Sprint)
                    {
                        allDir = 3;
                    }
                    if (jog && !Sprint)
                    {
                        allDir = 2;
                    }
                    if (!jog && !Sprint)
                    {
                        allDir = 1;
                    }
                }
                if (Input.GetAxis("Vertical") < 0 || Input.GetAxis("Horizontal") < 0)
                {
                    if (jog && Sprint || !jog && Sprint)
                    {
                        allDir = 3;
                    }
                    if (jog && !Sprint)
                    {
                        allDir = 2;
                    }
                    if (!jog && !Sprint)
                    {
                        allDir = 1;
                    }
                }
                if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
                {
                    allDir = 0;
                    jog = false;
                }
                Speed = allDir;
                Dir = 0;

                if (!DisableSprint)
                {
                    if (Input.GetButtonDown("Sprint"))
                    {
                        jog = true;
                    }
                }
            }

            if (!DisableSprint)
            {
                Sprint = Input.GetButton("Sprint");
            }
            else {
                Sprint = false;
                jog = false;
            }


            if (Input.GetButtonDown("Crouch"))
            {
                Crouch = !Crouch;
            }
        }
        else {
            Speed = Input.GetAxis("Vertical");
            Dir = Input.GetAxis("Horizontal");
        }


        if (Sprint && anim.GetFloat("SPD") > 2.8f)
        {
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Movement"))
            {
                if (anim.GetFloat("BZWep") == 0)
                {
                    anim.SetLayerWeight(1, 0);
                }
                else {
                    anim.SetLayerWeight(1, 1);
                }
            }
            else {
                anim.SetLayerWeight(1, 1);
            }
        }
        else {
            anim.SetLayerWeight(1, 1);
        }
    }

    public void AbleToAim()
    {
        if (!Skydiving)
        {
            if (!Sitting)
            {
                if (!InCar)
                {
                    Aim = Input.GetButton("Fire2") || Input.GetButton("Fire1");

                    if (Aim)
                    {
                        anim.SetBool("Roll", Input.GetButton("Jump"));
                    }
                }
                else
                {
                    if (anim.GetFloat("AngleDir") <= -70f || anim.GetFloat("AngleDir") >= -113f)
                    {
                        Aim = false;
                    }
                    if (anim.GetFloat("AngleDir") >= -70f || anim.GetFloat("AngleDir") <= -113f)
                    {
                        Aim = Input.GetButton("Fire2") || Input.GetButton("Fire1");
                    }
                }
            }
        }
    }

    void AutoAimMelee()
    {
        if (AutoTarget)
        {
            if (WP_CTRL.CurrentSlot == 0)
            {
                if ((transform.position - AutoTarget.position).sqrMagnitude < 0.8f * 0.8f)
                {
                    transform.Translate(Vector3.back * Time.deltaTime);
                }
                if (Attack && !Swim && !Ladder && !InCar)
                {
                    Vector3 lookTarget = AutoTarget.position;
                    Vector3 thisPos = transform.position;
                    Vector3 lookDir = lookTarget - thisPos;
                    Quaternion lookRot = Quaternion.LookRotation(lookDir);
                    lookRot.x = 0;
                    lookRot.z = 0;
                    Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 50);
                    transform.rotation = newRotation;
                }
            }
        }
    }

    void AbleToShoot()
    {
        if (!Sitting)
        {
            if (Input.GetButton("Fire2"))
            {
                Attack = Input.GetButton("Fire1");
                CancelInvoke("AttackDelay");
            }
            else
            {
                if (Input.GetButton("Fire1"))
                {
                    Invoke("AttackDelay", 1f);
                }
                else
                {
                    Attack = false;
                    CancelInvoke("AttackDelay");
                }
            }
        }
    }

    void AbleToMelee()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            Attack = true;
            Agressive = true;
        }
        else
        {
            Attack = false;
        }

        if (Agressive)
        {
            Invoke("UnAggressive", 4f);
        }
        else {
            CancelInvoke("UnAggressive");
        }

        if (Sprint) {
            Agressive = false;
        }
    }

    void UnAggressive() {
        Agressive = false;
    }

    void AbleToBrowseWeapon() {
        int add = 1;
        int sub = -1;

        stateInfo = anim.GetCurrentAnimatorStateInfo(1);
        if (!stateInfo.IsName("Reload"))
        {
            if (Input.GetButtonDown("NextWeapon") || Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                WP_CTRL.SwitchWeapon(add);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                WP_CTRL.SwitchWeapon(sub);
            }
        }
    }

    void AttackDelay() {
        Attack = Input.GetButton("Fire1");
    }

    void ParkourSystem() {
        if (anim.GetFloat("ParkourState") == 0)
        {
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Movement"))
            {
                if (PLow == true && PHigh == false && PStep == true)
                {
                    m_Target = VaultHit.point;
                    m_Target.y = (VaultHit.collider.bounds.extents.y) + (VaultHit.transform.position.y);
                    if (PDense == true)
                    {
                        ParkourID = 3;
                    }
                    else
                    {
                        ParkourID = 2;
                    }
                }
                if (PLow == true && PHigh == false && PStep == false)
                {
                    m_Target = VaultHit.point;
                    m_Target.y = (VaultHit.collider.bounds.extents.y) + (VaultHit.transform.position.y);
                    if (PDense == true)
                    {
                        ParkourID = 3;
                    }
                    else
                    {
                        ParkourID = 2;
                    }
                }
                if (PLow == false && PHigh == false && PStep == true)
                {
                    m_Target = StepHit.point;
                    m_Target.y = (StepHit.collider.bounds.extents.y) + (StepHit.transform.position.y);
                    ParkourID = 1;
                    Parkour = true;
                }
                if (PLow == true && PHigh == true && PStep == true)
                {
                    m_Target = ClimbHit.point;
                    m_Target.y = (ClimbHit.collider.bounds.extents.y) + (ClimbHit.transform.position.y) + 0.1f;
                    ParkourID = 4;
                }
                if (PLow == false && PHigh == false && PStep == false)
                {
                    ParkourID = 0;
                }
                if (PLow == false && PHigh == true && PStep == false)
                {
                    m_Target = ClimbHit.point;
                    m_Target.y = (ClimbHit.collider.bounds.extents.y) + (ClimbHit.transform.position.y) + 0.1f;
                    ParkourID = 4;
                }
                if (Input.GetButtonDown("Jump"))
                {
                    Parkour = true;
                }

            }
            else
            {
                Parkour = false;
            }
        }
        else {
            Swim = false;
            if (!Crouch)
            {
                Aim = false;
                if (ParkourID != 0 && ParkourID != 1 && ParkourID != 2)
                {
                    WP_CTRL.CurrentSlot = 0;
                }
            }
            Parkour = false;
        }
    }

    void GroundedCheck()
    {
        Ray PJumpRay = new Ray(ParkourDetectorJump.position, ParkourDetectorJump.forward);
        RaycastHit hitJump;
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (Physics.Raycast(PJumpRay, out hitJump, 10f))
        {
            AllowSkydive = false;
        }
        else {
            if (!Car_SCRPT.MySit && !stateInfo.IsName("GetOut 0") && !Ladder && !stateInfo.IsName("LadderGetTop") && !stateInfo.IsName("Jump_Action") && !Swim && anim.GetFloat("BZWep") == 0)
            {
                ShouldDie = true;
                AllowSkydive = true;
            }

        }

        if (!Car_SCRPT.MySit && !Ladder && !Swim)
        {
            if (Physics.Raycast(PJumpRay, out hitJump, 1f))
            {
                PJump = true;

                VerticalVelocity = -gravity * Time.deltaTime;
            }
            else
            {
                PJump = false;
                VerticalVelocity -= gravity * Time.deltaTime;
            }
        }
        else {
            VerticalVelocity = 0;
        }

        if (!PJump)
        {
            Falling = true;
        }
        else {
            if (ShouldDie)
            {
                if (Health_script.alive && !Ladder && !Swim)
                {
                    Health_script.Takedamage(100, null);
                }
            }
            Falling = false;
        }

        if (!Health_script.alive || Parachuting || Ladder || stateInfo.IsName("LadderGetTop") || stateInfo.IsName("Jump_Action") || Swim)
        {
            ShouldDie = false;
        }

       // Falling = !PJump;
        Vector3 moveVector = new Vector3(0, VerticalVelocity, 0);
        CH_SCRPT.ControllerC.Move(moveVector * Time.deltaTime);
    }

    void LadderSystem()
    {
        if (anim.IsInTransition(0))
            return;

        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Ladder Climb"))
        {
            // if player is climbing
            Vector3 climbpos = this.transform.position;
            climbpos.x = LadderPos.transform.position.x;
            climbpos.z = LadderPos.transform.position.z;
            this.transform.position = climbpos;
        }
        if (Ladder)
        {
            this.transform.rotation = Quaternion.Lerp(transform.rotation, LadderPos.rotation, Time.deltaTime * 5);
        }
    }

    public void ParkourDetect()
    {
        VaultRay = new Ray(ParkourDetector.position, ParkourDetector.forward);
        ClimbRay = new Ray(ParkourDetectorTop.position, ParkourDetectorTop.forward);
        DenseRay = new Ray(ParkourDetectorDense.position, ParkourDetectorDense.forward);
        StepRay = new Ray(ParkourDetectorStep.position, ParkourDetectorStep.forward);
        dir = transform.TransformDirection(Vector3.forward);


        if (Physics.Raycast(VaultRay, out VaultHit, prkrdtector))
        {
            DisableSprint = true;
            if (VaultHit.collider.gameObject.tag == "Parkour")
            {
                PLow = true;
            }
            else
            {
                PLow = false;
            }
        }
        else
        {
            DisableSprint = false;
            PLow = false;
        }

        if (Physics.Raycast(ClimbRay, out ClimbHit, prkrdtector))
        {
            if (ClimbHit.collider.gameObject.tag == "Parkour")
            {
                PHigh = true;
            }
            else
            {
                PHigh = false;
            }
        }
        else
        {
            PHigh = false;
        }

        if (Physics.Raycast(DenseRay, out DenseHit, prkrdtector))
        {
            if (DenseHit.collider.gameObject.tag == "Parkour")
            {
                PDense = true;
            }
            else
            {
                PDense = false;
            }
        }
        else
        {
            PDense = false;
        }

        if (Physics.Raycast(StepRay, out StepHit, prkrdtector))
        {
            if (StepHit.collider.gameObject.tag == "Parkour")
            {
                PStep = true;
            }
            else
            {
                PStep = false;
            }
        }
        else
        {
            PStep = false;
        }
    }

    void ProcessMatchTarget()
    {
        if (anim.IsInTransition(0))
            return;

        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Jump_Action"))
        {
            if (ParkourID == 2)
            {
                anim.MatchTarget(m_Target, new Quaternion(), AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), m_VaultMatchTargetStart, m_VaultMatchTargetStop);
            }
            if (ParkourID == 3)
            {
                anim.MatchTarget(m_Target, new Quaternion(), AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), m_VaultMatchTargetStart, m_VaultMatchTargetStop);
            }
            if (ParkourID == 1)
            {
                anim.MatchTarget(m_Target, new Quaternion(), AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), m_StepMatchTargetStart, m_StepMatchTargetStop);
            }
            if (ParkourID == 4)
            {
                anim.MatchTarget(m_Target, new Quaternion(), AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), m_ClimbMatchTargetStart, m_ClimbMatchTargetStop);
            }
        }
        if (stateInfo.IsName("Take_Cover"))
        {
            if (anim.GetFloat("BZWep") == 1)
            {
                if (Cover && c_Target != Vector3.zero)
                {
                    anim.MatchTarget(c_Target, new Quaternion(), AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), m_CoverMatchTargetStart, m_CoverMatchTargetStop);
                }
            }
        }
        if (stateInfo.IsName("LadderStart"))
        {
            if (anim.GetFloat("BZWep") == 1)
            {
                if (LadderHandPos)
                {
                    anim.MatchTarget(LadderHandPos.position, new Quaternion(), AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), m_LadderSMatchTargetStart, m_LadderSMatchTargetStop);
                }
            }
        }
        if (stateInfo.IsName("LadderGetTop"))
        {
            if (anim.GetFloat("BZWep") == 1)
            {
                if (LadderFootPos)
                {
                    anim.MatchTarget(LadderFootPos.position, new Quaternion(), AvatarTarget.LeftFoot, new MatchTargetWeightMask(Vector3.one, 0), m_LadderMatchTargetStart, m_LadderMatchTargetStop);
                }
            }
        }
    }

    public void CharacterLook()
    {
        if (ViewPoint == CameraMode.TPS)
        {
            if (!InCar)
            {
                if (WP_CTRL.CurrentGun)
                {
                    if (WP_CTRL.CurrentGun.GetComponent<WeaponInfo>().hasScope)
                    {
                        if (Aim)
                        {
                            Mcamera.transform.position = Vector3.Lerp(Mcamera.transform.position, WP_CTRL.CurrentGun.GetComponent<WeaponInfo>().BulletSpawner.position, 0.3f);
                        }
                        else
                        {
                            Mcamera.transform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                    else
                    {
                        Mcamera.transform.localPosition = new Vector3(0, 0, 0);
                    }
                }
                else
                {
                    Mcamera.transform.localPosition = new Vector3(0, 0, 0);
                }
                if (Aim)
                {
                    other.lookSpeed = 30;
                    Transform mainCamT = Mcamera.transform;
                    Transform pivotT = mainCamT.parent;
                    Vector3 pivotPos = pivotT.position;
                    Vector3 lookTarget = pivotPos + (pivotT.forward * other.lookDistance);
                    Vector3 thisPos = transform.position;
                    Vector3 lookDir = lookTarget - thisPos;
                    Quaternion lookRot = Quaternion.LookRotation(lookDir);
                    lookRot.x = 0;
                    lookRot.z = 0;
                    Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * other.lookSpeed);
                    transform.rotation = newRotation;
                }
                else
                {
                    if (Phone_SCRPT.TakingPicture)
                    {
                        if (!Sitting)
                        {
                            other.lookSpeed = 15;
                            Transform mainCamT = Mcamera.transform;
                            Transform pivotT = mainCamT.parent;
                            Vector3 pivotPos = pivotT.position;
                            Vector3 lookTarget = pivotPos + (pivotT.forward * other.lookDistance);
                            Vector3 thisPos = transform.position;
                            Vector3 lookDir = lookTarget - thisPos;
                            Quaternion lookRot = Quaternion.LookRotation(lookDir);
                            lookRot.x = 0;
                            lookRot.z = 0;
                            Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * other.lookSpeed);
                            transform.rotation = newRotation;
                            Turn = 0;
                        }
                    }
                    else
                    {
                        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                        {
                            Vector3 TargetDirection = (Mcamera.transform.right * Input.GetAxis("Horizontal")) + (Mcamera.transform.forward * Input.GetAxis("Vertical"));
                            Quaternion lookRot = Quaternion.LookRotation(TargetDirection);
                            lookRot.x = 0;
                            lookRot.z = 0;
                            Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * other.lookSpeed);
                            if (transform.rotation != newRotation)
                            {
                                Turn = Vector3.Angle(transform.forward, TargetDirection);
                                cross = Vector3.Cross(transform.forward, TargetDirection);
                                if (cross.y < 0)
                                {
                                    Turn = -Turn;
                                }
                            }
                            else
                            {
                                Turn = 0;
                            }

                            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                            if (stateInfo.IsName("Movement"))
                            {
                                other.lookSpeed = 2;
                                transform.rotation = newRotation;
                            }
                            if (stateInfo.IsName("SkyDivingMode"))
                            {
                                other.lookSpeed = 0.2f;
                                transform.rotation = newRotation;
                            }
                            if (stateInfo.IsName("Parachuting"))
                            {
                                other.lookSpeed = 0.1f;
                                transform.rotation = newRotation;
                            }
                        }

                    }
                }
            }
            else {
                Mcamera.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
        else
        {
            if (Car_SCRPT.MySit == null)
            {
                if (!Sitting)
                {
                    other.lookSpeed = 15;
                    Transform mainCamT = Mcamera.transform;
                    Transform pivotT = mainCamT.parent;
                    Vector3 pivotPos = pivotT.position;
                    Vector3 lookTarget = pivotPos + (pivotT.forward * other.lookDistance);
                    Vector3 thisPos = transform.position;
                    Vector3 lookDir = lookTarget - thisPos;
                    Quaternion lookRot = Quaternion.LookRotation(lookDir);
                    lookRot.x = 0;
                    lookRot.z = 0;
                    Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * other.lookSpeed);
                    transform.rotation = newRotation;
                    Turn = 0;
                }

                if (WP_CTRL.CurrentGun)
                {
                    if (WP_CTRL.CurrentGun.GetComponent<WeaponInfo>().hasScope)
                    {
                        if (Aim)
                        {
                            Mcamera.transform.position = Vector3.Lerp(Mcamera.transform.position, WP_CTRL.CurrentGun.GetComponent<WeaponInfo>().BulletSpawner.position, 0.3f);
                        }
                        else
                        {
                            Mcamera.transform.position = Vector3.Lerp(Mcamera.transform.position, FPSCamPos.position, 0.3f);
                        }
                    }
                    else
                    {
                        Mcamera.transform.position = Vector3.Lerp(Mcamera.transform.position, FPSCamPos.position, 0.3f);
                    }
                }
                else
                {
                    Mcamera.transform.position = Vector3.Lerp(Mcamera.transform.position, FPSCamPos.position, 0.3f);
                }
            }
            else
            {
                Mcamera.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    void CoverInput()
    {
        // Inputs for cover
        if (!Swim)
        {
            Coverray = new Ray(CoverDetector.position, transform.forward);


            if (Physics.Raycast(Coverray, out Coverhit, cvrdtector, CastLayer))
            {
                if (Input.GetButtonDown("TakeCover"))
                {
                    if (anim.GetFloat("BZWep") == 0 && !Coverhit.collider.GetComponent<MeshCollider>())
                    {
                        // If press cover button
                        TakeCover();
                        c_Target = Coverhit.point;
                    }
                }
            }
            else
            {
                Cover = false;
            }

            Debug.DrawRay(Coverhit.point, CoverDetector.forward * -1, Color.blue); // turn gizmo on during play mode to see this in action

            if (Cover)
            {
                // if taking cover
                StartCoroutine(CoverSetup(Coverhit.normal.normalized * -1f));
            }
        }
        else
        {
            Cover = false;
        }
    }

    IEnumerator CoverSetup(Vector3 TargetDirection)
    {
        // This function is to make proper rotation during cover
        if (!Cover)
        {
            yield break;
        }
        Debug.Log("Cover Rotation towards object");
        Quaternion targetRot = Quaternion.LookRotation(TargetDirection, transform.up);
        if (!Cover || Aim)
        {
            yield break;
        }
        Quaternion newRot = Quaternion.Lerp(transform.rotation, targetRot, 8f * Time.deltaTime);
        transform.rotation = new Quaternion(0 , newRot.y ,0 , newRot.w);

        yield return null;
    }

    void CoverSystem()
    {
        // functions for cover
        Ray rayLeft = new Ray(CoverDetectorLeft.position, transform.forward);
        RaycastHit hitLeft;
        Ray rayRight = new Ray(CoverDetectorRight.position, transform.forward);
        RaycastHit hitRight;

        if (Cover)
        {

            // if in cover
            if (Physics.Raycast(rayLeft, out hitLeft, cvrdCloseDist))
            {
                // if left side of cover is not clear
                CoverOutLeft = false;
            }
            else
            {
                // if left side of cover is clear
                CoverOutLeft = true;
            }
            if (Physics.Raycast(rayRight, out hitRight, cvrdCloseDist))
            {
                // if right side of cover is not clear
                CoverOutRight = false;
            }
            else
            {
                // if right side of cover is clear
                CoverOutRight = true;
            }
        }
        else
        {
            // if not in cover
            CoverOutLeft = false;
            CoverOutRight = false;
        }

        InCover = Cover;
    }

    public void TakeCover()
    {
        // take cover function
        Cover = !Cover;
    }

    void GetOutOfCover()
    {
        // get out of cover called online
        anim.SetTrigger("ExitCover");
    }

    void CoverFire()
    {
        Aim = Input.GetButton("Fire2");
        if (anim.GetFloat("BZWep") == 0)
        {
            Attack = Input.GetButton("Fire1");
        }
        else {
            Attack = false;
        }
    }

    void DetectRotationX()
    {
        other.lookSpeed = 2;
        Vector3 TargetDirection = (Mcamera.transform.right) + (Mcamera.transform.forward);
        Quaternion lookRot = Quaternion.LookRotation(TargetDirection);
        lookRot.x = 0;
        lookRot.z = 0;
        Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * other.lookSpeed);
        if (transform.rotation != newRotation)
        {
            TurnInt = Vector3.Angle(transform.forward, TargetDirection);
            crossInt = Vector3.Cross(transform.forward, TargetDirection);
            if (crossInt.y < 0)
            {
                TurnInt = -TurnInt;
            }
        }
        else
        {
            TurnInt = 0;
        }
    }
}
