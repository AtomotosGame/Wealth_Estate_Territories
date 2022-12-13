using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamReposition : MonoBehaviour
{
    GlobalObjects Global_Script;
    CameraRig Cam_script;
    Controller CTRL_script;
    bool recall;
    public float Ydefault;
    public float Xdefault;
    public float Zdefault;
    public float Yaim;
    public float Xaim;
    public float Zaim;
    public float YInCar;
    public float XInCar;
    public float ZInCar;

    // Use this for initialization
    void Start()
    {
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
        Cam_script = GetComponent<CameraRig>();
        if (Cam_script.target != null)
        {
            CTRL_script = Global_Script.LocalPlayer.GetComponent<Controller>();
            recall = false;  
        }
        else
        {
            recall = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (recall)
        {
            if (Cam_script.target != null)
            {
                CTRL_script = Global_Script.LocalPlayer.GetComponent<Controller>();
            }
        }

        if (CTRL_script != null)
        {
            if (!CTRL_script.InCar)
            {
                Cam_script.movement.movementLerpSpeed = 50f;
                if (CTRL_script.Aim)
                {
                    Cam_script.cameraSettings.camPositionOffsetRight.y = Yaim;
                    Cam_script.cameraSettings.camPositionOffsetRight.z = Zaim;
                    Cam_script.cameraSettings.camPositionOffsetRight.x = Xaim;
                    Cam_script.cameraSettings.camPositionOffsetLeft.y = Yaim;
                    Cam_script.cameraSettings.camPositionOffsetLeft.z = Zaim;
                    Cam_script.cameraSettings.camPositionOffsetLeft.x = -Xaim;
                }
                else
                {
                    Cam_script.cameraSettings.camPositionOffsetRight.y = Ydefault;
                    Cam_script.cameraSettings.camPositionOffsetRight.z = Zdefault;
                    Cam_script.cameraSettings.camPositionOffsetRight.x = Xdefault;
                    Cam_script.cameraSettings.camPositionOffsetLeft.y = Ydefault;
                    Cam_script.cameraSettings.camPositionOffsetLeft.z = Zdefault;
                    Cam_script.cameraSettings.camPositionOffsetLeft.x = -Xdefault;
                }

            }
            else
            {
                Cam_script.movement.movementLerpSpeed = 15f;
                if (CTRL_script.GetComponent<Action_Car>().MySit.CUC.TypeID == 0)
                {
                    Cam_script.cameraSettings.camPositionOffsetRight.y = YInCar;
                    Cam_script.cameraSettings.camPositionOffsetRight.z = ZInCar * CTRL_script.GetComponent<Action_Car>().MySit.CUC.VehicleSize;
                    Cam_script.cameraSettings.camPositionOffsetRight.x = XInCar;
                    Cam_script.cameraSettings.camPositionOffsetLeft.y = YInCar;
                    Cam_script.cameraSettings.camPositionOffsetLeft.z = ZInCar * CTRL_script.GetComponent<Action_Car>().MySit.CUC.VehicleSize;
                    Cam_script.cameraSettings.camPositionOffsetLeft.x = -XInCar;
                }
                if (CTRL_script.GetComponent<Action_Car>().MySit.CUC.TypeID == 2)
                {
                    Cam_script.cameraSettings.camPositionOffsetRight.y = YInCar;
                    Cam_script.cameraSettings.camPositionOffsetRight.z = ZInCar * CTRL_script.GetComponent<Action_Car>().MySit.CUC.VehicleSize;
                    Cam_script.cameraSettings.camPositionOffsetRight.x = XInCar;
                    Cam_script.cameraSettings.camPositionOffsetLeft.y = YInCar;
                    Cam_script.cameraSettings.camPositionOffsetLeft.z = ZInCar * CTRL_script.GetComponent<Action_Car>().MySit.CUC.VehicleSize;
                    Cam_script.cameraSettings.camPositionOffsetLeft.x = -XInCar;
                }
                if (CTRL_script.GetComponent<Action_Car>().MySit.CUC.TypeID == 1)
                {
                    Cam_script.cameraSettings.camPositionOffsetRight.y = YInCar;
                    Cam_script.cameraSettings.camPositionOffsetRight.z = ZInCar * CTRL_script.GetComponent<Action_Car>().MySit.CUC.VehicleSize;
                    Cam_script.cameraSettings.camPositionOffsetRight.x = XInCar;
                    Cam_script.cameraSettings.camPositionOffsetLeft.y = YInCar;
                    Cam_script.cameraSettings.camPositionOffsetLeft.z = ZInCar * CTRL_script.GetComponent<Action_Car>().MySit.CUC.VehicleSize;
                    Cam_script.cameraSettings.camPositionOffsetLeft.x = -XInCar;
                }
            }
        }
    }
}
