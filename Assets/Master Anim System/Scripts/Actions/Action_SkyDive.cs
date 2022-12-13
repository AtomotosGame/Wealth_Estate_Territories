using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_SkyDive : MonoBehaviour {
    public bool HasParachute;
    public GameObject ParachuteOBJ;
    public GameObject ChuteOBJ;
    Controller CTL;
    Health HTL;
    Action_Car ACR;
    GlobalObjects Global_Script;
    bool OpenChute = false;
    CharacterController CC;
    Animator anim;
    // Use this for initialization
    void Start () {
        CTL = GetComponent<Controller>();
        HTL = GetComponent<Health>();
        CC = GetComponent<CharacterController>();
        ACR = GetComponent<Action_Car>();
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
        ParachuteOBJ.transform.SetParent(CTL.FPSCamPos.parent);
        anim = GetComponent<Animator>();

    }

    void LateUpdate()
    {
        if (OpenChute)
        {
            ChuteOBJ.transform.rotation = new Quaternion(0, ChuteOBJ.transform.rotation.y, 0, ChuteOBJ.transform.rotation.w);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ParachuteOBJ.SetActive(HasParachute);
        ChuteOBJ.SetActive(OpenChute);

        if (!ACR.MySit && !CTL.Ladder && !CTL.Swim)
        {
            if (CTL.Skydiving && !OpenChute)
            {
                Global_Script.cam.GetComponent<CameraRig>().ShakeCamera(0.01f, 0.2f);
            }

            if (HasParachute)
            {
                if (!HTL.alive)
                {
                    SkyOff();
                    OffParachute();
                }


                if (CTL.AllowSkydive)
                {
                    if (!OpenChute)
                    {
                        if (Input.GetButton("Fire1"))
                        {
                            CTL.Parachuting = true;
                            OpenChute = true;
                        }
                    }

                    CTL.Skydiving = true;
                }
            }
            else
            {
                CTL.Skydiving = false;
                CTL.Parachuting = false;
            }


            if (CTL.Skydiving)
            {
                if (OpenChute)
                {
                    CC.Move(transform.forward * Time.deltaTime * 10);
                    CTL.gravity = 0.5f;
                }
                else
                {
                    CTL.gravity = 5;
                    CC.Move(transform.forward * Time.deltaTime * 7);
                }
            }
            else
            {
                CTL.gravity = 19;
            }
        }
        else
        {
            CTL.Skydiving = false;
            CTL.Parachuting = false;
            CTL.gravity = 19;

        }
    }

    public void SkyOff() {
        OpenChute = false;
        CTL.Skydiving = false;
        CTL.Parachuting = false;
    }

    public void OffParachute() {
        HasParachute = false;
    }
}
