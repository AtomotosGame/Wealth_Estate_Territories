using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Action_Car : MonoBehaviour {
    public Transform PlayerTR; // User's Transform
    Controller CTRL_Script; // User's Controller Script
    CharacterController CC; // User's Character Controller Component
    Animator anim; // User's Animator Component
    AnimatorStateInfo stateInfo; // Animator's state info generator 
    public DoorInteraction MySit; // User's Car Seat
    [HideInInspector] public float SeatSide;  // The Side of the seat Left/Right
    [HideInInspector] public float SeatID; // Driver/Passenger
    [HideInInspector] public bool DoorOpened; // Check whether User's Car door is open
    DoorInteraction lastCar; // User's last car
    public float prkrdtector = 0.4f; // User's car detecting length
    Transform CarDetector; // The position which raycasts the line that detects the car's door
    Vector3 PositionOffset = new Vector3(0,-0.37f,-0.88f); 
    Vector3 PositionOffset2 = new Vector3(0, -0.22f, 0.16f);
    // Match Track
    Vector3 m_Target = new Vector3();
    private Vector3 dir;
    bool InSit = false;
    private const float m_DoorMatchTargetStart = 0.0f;
    private const float m_DoorMatchTargetStop = 0.30f;
    private const float m_sitMatchTargetStart = 0.05f;
    private const float m_sitMatchTargetStop = 0.90f;
    private const float m_standMatchTargetStart = 0.05f;
    private const float m_standMatchTargetStop = 0.80f;

    // Use this for initialization
    void Start () {
        CTRL_Script = GetComponent<Controller>();
        CarDetector = CTRL_Script.Mcamera.transform;
        CC = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        ProcessMatchTarget();

        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("SitInCar") || stateInfo.IsName("CloseDoor_Sitting"))
        {
            setPosition();
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            CTRL_Script.cam.GetComponent<CameraRig>().target = MySit.CUC.transform;
            CTRL_Script.InCar = true;
            if (MySit)
            {
                PlayerTR.parent = MySit.StandPos.parent;
            }
            if (MySit.CUC.TypeID != 1)
            {
                if (MySit.CUC.speed < 20f)
                {
                    if (anim.GetFloat("BZWep") == 0)
                    {
                        if (Input.GetButtonDown("Interact"))
                        {
                            if (MySit)
                            {
                                CancelCar();
                                OpenDoor();
                                anim.SetTrigger("CarFunctionOut");
                            }
                        }
                    }
                }
            }
            else {
                if (MySit.CUC.speed > 20f)
                {
                    if (GetComponent<Action_SkyDive>().HasParachute)
                    {
                        if (anim.GetFloat("BZWep") == 0)
                        {
                            if (Input.GetButtonDown("Interact"))
                            {
                                if (MySit)
                                {
                                    CancelCar();
                                    OpenDoor();
                                    anim.SetTrigger("CarFunctionOut");
                                }
                            }
                        }
                    }
                }
                else {
                    if (anim.GetFloat("BZWep") == 0)
                    {
                        if (Input.GetButtonDown("Interact"))
                        {
                            if (MySit)
                            {
                                CancelCar();
                                OpenDoor();
                                anim.SetTrigger("CarFunctionOut");
                            }
                        }
                    }
                }
            }
            InSit = true;
        }
        else {
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            CancelCar();
            CTRL_Script.cam.GetComponent<CameraRig>().target = PlayerTR;
            CTRL_Script.InCar = false;
            PlayerTR.parent = null;
            if (Input.GetButtonDown("Interact"))
            {
                if (anim.GetFloat("BZWep") == 0)
                {
                    if (!CTRL_Script.InMenu)
                    {
                        Check4Car();
                    }
                }
            }
            InSit = false;
        }
        if (MySit != null)
        {
            CC.enabled = false;
            MySit.User = this.gameObject;
            if (MySit.AlternateEntrance) {
                MySit.AlternateEntrance.User = this.gameObject;
            }
            PlayerTR.rotation = Quaternion.Lerp(PlayerTR.rotation, MySit.StandPos.rotation, 0.1f);
            anim.SetFloat("V_speed", MySit.CUC.speed);
            if (MySit.isBike)
            {
                if (anim.GetFloat("ForcePosition") != 0)
                {
                    GetComponent<WeaponControl>().CurrentSlot = 0;
                }
            }
            else
            {
                if (anim.GetFloat("ForcePosition") != 0)
                {
                    GetComponent<WeaponControl>().CurrentSlot = 0;
                }
            }

            if (MySit.CUC.TypeID == 0)
            {
                anim.SetFloat("VehicleType", 0);
            }
            if (MySit.CUC.TypeID == 2)
            {
                anim.SetFloat("VehicleType", 1);
            }
            if (MySit.CUC.TypeID == 1)
            {
                anim.SetFloat("VehicleType", -1);
            }
        }


    }

    void setPosition() {
        if (transform.parent != null)
        {
            if (MySit.CUC.TypeID != 2)
            {
                    transform.localPosition = new Vector3(MySit.SitPos.localPosition.x + (PositionOffset.x), MySit.SitPos.localPosition.y + PositionOffset.y, MySit.SitPos.localPosition.z + PositionOffset.z);
            }
            else
            {
                    transform.localPosition = new Vector3(MySit.SitPos.localPosition.x + (PositionOffset2.x), MySit.SitPos.localPosition.y + PositionOffset2.y, MySit.SitPos.localPosition.z + PositionOffset2.z);
            }
        }
    }

    void Check4Car()
    {
        Ray PJumpRay = new Ray(CTRL_Script.ParkourDetector.position, CarDetector.forward);
        RaycastHit hit;

        if (Physics.Raycast(PJumpRay, out hit, 2f))
        {
            if (!CTRL_Script.InCover && MySit == null)
            {
                if (hit.collider.GetComponent<DoorInteraction>())
                {
                    if (hit.collider.GetComponent<DoorInteraction>().CUC.speed < 1.5f)
                    {
                        if (hit.collider.GetComponent<DoorInteraction>().Door_State == Door_Int.Closed)
                        {
                            DoorOpened = false;
                        }
                        else
                        {
                            DoorOpened = true;
                        }

                        if (hit.collider.GetComponent<DoorInteraction>().OpenDirection == Door_Side.Left)
                        {
                            SeatSide = 0;
                        }
                        else
                        {
                            SeatSide = 1;
                        }

                        if (!hit.collider.GetComponent<DoorInteraction>().User)
                        {
                            if (hit.collider.GetComponent<DoorInteraction>().CUC.TypeID == 0 || hit.collider.GetComponent<DoorInteraction>().CUC.TypeID == 1)
                            {
                                if (hit.collider.GetComponent<DoorInteraction>().CUC.Locked)
                                {
                                    MySit = hit.collider.GetComponent<DoorInteraction>();
                                    PlayerTR.parent = MySit.StandPos;
                                    anim.SetTrigger("UnlockCar");
                                    anim.SetBool("Jacking", false);
                                }
                                else
                                {
                                    MySit = hit.collider.GetComponent<DoorInteraction>();
                                    PlayerTR.parent = MySit.StandPos;
                                    anim.SetTrigger("CarFunction");
                                    anim.SetBool("Jacking", false);
                                }
                            }
                            else
                            {
                                if (hit.collider.GetComponent<DoorInteraction>().CUC.Locked == false)
                                {
                                    MySit = hit.collider.GetComponent<DoorInteraction>();
                                    PlayerTR.parent = MySit.StandPos;
                                    anim.SetTrigger("CarFunction");
                                    anim.SetBool("Jacking", false);
                                }
                            }
                        }
                        else
                        {
                            if (hit.collider.GetComponent<DoorInteraction>().CUC.TypeID == 0 || hit.collider.GetComponent<DoorInteraction>().CUC.TypeID == 1)
                            {
                                if (hit.collider.GetComponent<DoorInteraction>().CUC.Locked)
                                {
                                    MySit = hit.collider.GetComponent<DoorInteraction>();
                                    PlayerTR.parent = MySit.StandPos;
                                    anim.SetTrigger("UnlockCar");
                                    anim.SetBool("Jacking", false);
                                }
                                else
                                {
                                    if (hit.collider.GetComponent<DoorInteraction>().User.GetComponent<Health>().alive)
                                    {
                                        GetComponent<WeaponControl>().Global_Script.notorietyLevel += 1;
                                        hit.collider.GetComponent<DoorInteraction>().User.GetComponent<Animator>().SetTrigger("Jack");
                                        MySit = hit.collider.GetComponent<DoorInteraction>();
                                        PlayerTR.parent = MySit.StandPos;
                                        anim.SetTrigger("CarFunction");
                                        anim.SetBool("Jacking", true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                anim.SetBool("Jacking", false);
            }
        }
    }

    public void UnlockTheCar() {
        MySit.CUC.Locked = false;
    }

    void CancelCar()
    {
        if (MySit)
        {
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("OpenDoor"))
            {
                if (MySit.CUC.speed > 1.5f || CTRL_Script.Aim)
                {
                    anim.SetBool("CancelCar", true);
                    MySit.User = null;
                    MySit = null;
                }
                else
                {
                    anim.SetBool("CancelCar", false);
                }
            }
            else
            {
                anim.SetBool("CancelCar", false);
            }
        }
    }

    public void GetOut() {
        if (MySit)
        {
            if (MySit.AlternateEntrance)
            {
                MySit.AlternateEntrance.User = null;
            }
            MySit.User = null;
            MySit = null;
        }
    }

    public void OpenDoor()
    {
        if (MySit)
        {
            MySit.Door_State = Door_Int.Opened;
        }
    }

    public void CloseDoor() {
        MySit.Door_State = Door_Int.Closed;
    }

    void ProcessMatchTarget()
    {
        if (anim.IsInTransition(0))
            return;

        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("OpenDoor"))
        {
            if (MySit != null)
            {
                if (MySit.OpenDirection == Door_Side.Left)
                {
                    anim.MatchTarget(MySit.StandPos.position, new Quaternion(), AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), m_DoorMatchTargetStart, m_DoorMatchTargetStop);
                }
                else
                {
                    anim.MatchTarget(MySit.StandPos.position, new Quaternion(), AvatarTarget.LeftFoot, new MatchTargetWeightMask(Vector3.one, 0), m_DoorMatchTargetStart, m_DoorMatchTargetStop);
                }
            }
        }
        if (stateInfo.IsName("UnlockDoor"))
        {
            if (MySit != null)
            {
                if (MySit.OpenDirection != Door_Side.Left)
                {
                    anim.MatchTarget(MySit.StandPos.position, new Quaternion(), AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), m_DoorMatchTargetStart, m_DoorMatchTargetStop);
                }
                else
                {
                    anim.MatchTarget(MySit.StandPos.position, new Quaternion(), AvatarTarget.LeftFoot, new MatchTargetWeightMask(Vector3.one, 0), m_DoorMatchTargetStart, m_DoorMatchTargetStop);
                }
            }
        }
        if (stateInfo.IsName("GetIn 0"))
        {
            if (MySit != null)
            {
                lastCar = MySit;
                if (MySit.OpenDirection == Door_Side.Left)
                {
                    anim.MatchTarget(MySit.SitPos.position, new Quaternion(), AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), m_sitMatchTargetStart, m_sitMatchTargetStop);
                }
                else
                {
                    anim.MatchTarget(MySit.SitPos.position, new Quaternion(), AvatarTarget.LeftFoot, new MatchTargetWeightMask(Vector3.one, 0), m_sitMatchTargetStart, m_sitMatchTargetStop);
                }
            }
        }
        if (stateInfo.IsName("GetOut 0"))
        {
            if (lastCar.CUC.TypeID != 2)
            {
                if (lastCar.OpenDirection == Door_Side.Left)
                {
                    anim.MatchTarget(lastCar.StandPos.position, new Quaternion(), AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), m_standMatchTargetStart, m_standMatchTargetStop);
                }
                else
                {
                    anim.MatchTarget(lastCar.StandPos.position, new Quaternion(), AvatarTarget.LeftFoot, new MatchTargetWeightMask(Vector3.one, 0), m_standMatchTargetStart, m_standMatchTargetStop);
                }
            }
        }

    }
}
