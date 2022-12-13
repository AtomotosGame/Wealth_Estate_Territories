using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public enum Door_Int
{
    Closed,
    Opened
}


public enum Door_Side
{
    Left,
    Right
}

public class DoorInteraction : MonoBehaviour {
    public GameObject User;
    public VehiclManager CUC;
    public DoorInteraction AlternateEntrance;
    public Door_Int Door_State;
    public Door_Side OpenDirection;
    public bool UpDir;
    public float OpenSpeed;
    public float OpenAngle;
    public bool isDriver;
    public bool CanShuffleSit;
    public Transform StandPos;
    public Transform SitPos;
    public Transform DoorMesh;
    GlobalObjects Global_Script;
    [HideInInspector] public bool isBike;
    // Use this for initialization
    void Start () {
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
    }
	
	// Update is called once per frame
	void Update () {
        if (User != null)
        {
            AssignStates();
            if (isDriver)
            {
                if (CUC.CUCAI)
                {
                    if (User.GetComponent<MASAIBrain>())
                    {
                        CUC.AIDriver = true;
                        if (User.GetComponent<MASAIBrain>().FollowTarget)
                        {
                            CUC.CUCAI.SetTarget(User.GetComponent<MASAIBrain>().FollowTarget);
                        }
                        else {
                            if (User.GetComponent<MASAIBrain>().WayPoint) {
                                CUC.CUCAI.SetTarget(User.GetComponent<MASAIBrain>().WayPoint);
                            }
                        }
                    }
                    else
                    {
                        CUC.AIDriver = false;
                    }
                }
                else {
                    CUC.AIDriver = false;
                }
            }
        }
        else
        {
            if (isDriver)
            {
                CUC.UserInput = false;
            }
        }

        if (CUC.TypeID != 2)
        {
            if (Door_State == Door_Int.Closed)
            {
                Quaternion ClosedRot = Quaternion.Euler(0, 0, 0);
                DoorMesh.localRotation = Quaternion.Lerp(DoorMesh.localRotation, ClosedRot, OpenSpeed);
            }
            if (Door_State == Door_Int.Opened)
            {
                if (!UpDir)
                {
                    if (OpenDirection == Door_Side.Left)
                    {
                        Quaternion OpenRot = Quaternion.Euler(0, OpenAngle, 0);
                        DoorMesh.localRotation = Quaternion.Lerp(DoorMesh.localRotation, OpenRot, OpenSpeed);
                    }
                    if (OpenDirection == Door_Side.Right)
                    {
                        Quaternion OpenRot = Quaternion.Euler(0, -OpenAngle, 0);
                        DoorMesh.localRotation = Quaternion.Lerp(DoorMesh.localRotation, OpenRot, OpenSpeed);
                    }
                }
                else
                {
                    if (OpenDirection == Door_Side.Left)
                    {
                        Quaternion OpenRot = Quaternion.Euler(0, 0, -OpenAngle * 2);
                        DoorMesh.localRotation = Quaternion.Lerp(DoorMesh.localRotation, OpenRot, OpenSpeed);
                    }
                    if (OpenDirection == Door_Side.Right)
                    {
                        Quaternion OpenRot = Quaternion.Euler(0, 0, OpenAngle * 2);
                        DoorMesh.localRotation = Quaternion.Lerp(DoorMesh.localRotation, OpenRot, OpenSpeed);
                    }
                }
            }
            isBike = false;
        }
        else {
            Door_State = Door_Int.Opened;
            isBike = true;
        }
	}

    void AssignStates()
    {
        if (User.GetComponent<Action_Car>())
        {
            if (isDriver)
            {
                User.GetComponent<Action_Car>().SeatID = 1;
                if (User.GetComponent<Controller>().InCar && User == Global_Script.LocalPlayer)
                {
                    CUC.UserInput = true;
                }
                else
                {
                    CUC.UserInput = false;
                }
            }
            else
            {
                User.GetComponent<Action_Car>().SeatID = 0;
            }
            if (Door_State == Door_Int.Closed)
            {
                User.GetComponent<Action_Car>().DoorOpened = false;
            }
            else
            {
                User.GetComponent<Action_Car>().DoorOpened = true;
            }

        }
        if (User.GetComponent<MASAIBrain>()) { 
            if (isDriver)
            {
                User.GetComponent<AICarAction>().SeatID = 1;
                if (User.GetComponent<MASAIBrain>().InCar)
                {
                    CUC.UserInput = true;
                }
                else
                {
                    CUC.UserInput = false;
                }
            }
            else
            {
                User.GetComponent<AICarAction>().SeatID = 0;
            }
            if (Door_State == Door_Int.Closed)
            {
                User.GetComponent<AICarAction>().DoorOpened = false;
            }
            else
            {
                User.GetComponent<AICarAction>().DoorOpened = true;
            }

        }
    }
}
