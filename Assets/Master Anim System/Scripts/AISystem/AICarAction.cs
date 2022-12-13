using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarAction : MonoBehaviour {

    public Transform PlayerTR;
    MASAIBrain CTRL_Script;
    CapsuleCollider CC;
    Animator anim;
    AnimatorStateInfo stateInfo;
    public DoorInteraction MySit = null;
    [HideInInspector] public float SeatSide;
    [HideInInspector] public float SeatID;
    [HideInInspector] public bool DoorOpened;
    [HideInInspector] public bool GetIn = false;
    DoorInteraction lastCar = null;
    public float prkrdtector = 0.4f;
    Transform CarDetector;
    Vector3 PositionOffset = new Vector3(0, -0.37f, -0.88f);
    Vector3 PositionOffset2 = new Vector3(0, -0.22f, 0.16f);

    // Match Track
    bool InSit = false;
    private const float m_DoorMatchTargetStart = 0.0f;
    private const float m_DoorMatchTargetStop = 0.80f;
    private const float m_sitMatchTargetStart = 0.05f;
    private const float m_sitMatchTargetStop = 0.90f;
    private const float m_standMatchTargetStart = 0.05f;
    private const float m_standMatchTargetStop = 0.80f;

    // Use this for initialization
    void Start()
    {
        CTRL_Script = GetComponent<MASAIBrain>();
        CarDetector = CTRL_Script.ParkourDetector;
        CC = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();

        if (MySit) {
            MySit.User = this.gameObject;
            PlayerTR.rotation = Quaternion.Lerp(PlayerTR.rotation, MySit.StandPos.rotation, 0.1f);
            CTRL_Script.AlreadyInCar = true;
            if (MySit.OpenDirection == Door_Side.Left)
            {
                SeatSide = 0;
            }
            else
            {
                SeatSide = 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessMatchTarget();

        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("SitInCar") || stateInfo.IsName("CloseDoor_Sitting"))
        {
            setPosition();
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            CTRL_Script.InCar = true;
            if (MySit)
            {
                PlayerTR.parent = MySit.StandPos.parent;
            }
            if (!GetIn)
            {
                if (MySit.CUC.speed < 20f)
                {
                    if (MySit)
                    {
                        CancelCar();
                        OpenDoor();
                        anim.SetTrigger("CarFunction");
                    }
                }
            }
            InSit = true;
        }
        else
        {
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            CancelCar();
            CTRL_Script.InCar = false;
            PlayerTR.parent = null;
            if (GetIn)
            {
                if (!stateInfo.IsName("GetOut 0"))
                {
                    Check4Car();
                }
            }
            InSit = false;
        }
        if (MySit != null)
        {
            CC.enabled = false;
            MySit.User = this.gameObject;
            PlayerTR.rotation = Quaternion.Lerp(PlayerTR.rotation, MySit.StandPos.rotation, 0.1f);
            anim.SetFloat("V_speed", MySit.CUC.speed);

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

    void setPosition()
    {
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

            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("GetIn 0"))
            {
                if (hit.collider.GetComponent<DoorInteraction>())
                {
                    if (hit.collider.GetComponent<DoorInteraction>().CUC.speed < 1.5f)
                    {
                        if (MySit == null)
                        {
                            if (!hit.collider.GetComponent<DoorInteraction>().User)
                            {
                                if (hit.collider.GetComponent<DoorInteraction>().OpenDirection == Door_Side.Left)
                                {
                                    SeatSide = 0;
                                }
                                else
                                {
                                    SeatSide = 1;
                                }

                                MySit = hit.collider.GetComponent<DoorInteraction>();
                                anim.SetTrigger("CarFunction");
                            }
                        }
                    }
                }
            }
        }
    }

    void CancelCar()
    {
        if (GetIn)
        {
            if (MySit)
            {
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("OpenDoor"))
                {
                    if (MySit.CUC.speed > 1.5f || CTRL_Script.Aim)
                    {
                        anim.SetBool("CancelCar", true);
                        anim.ResetTrigger("CarFunction");
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
            else
            {
            }
        }
        else {
            anim.SetBool("CancelCar", true);
        }
    }

    public void GetOut()
    {
        MySit.User = null;
        MySit = null;
    }

    public void OpenDoor()
    {
        if (MySit)
        {
            MySit.Door_State = Door_Int.Opened;
        }
    }

    public void CloseDoor()
    {
        if (MySit)
        {
            MySit.Door_State = Door_Int.Closed;
        }
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
            if (lastCar)
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
}
