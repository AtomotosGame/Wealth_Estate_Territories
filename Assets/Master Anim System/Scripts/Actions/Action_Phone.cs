using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Phone : MonoBehaviour {
    public bool isPhoneOn;
    public bool TakingPicture;
    public bool Selfie;
    public GameObject PhoneObject;
    public GameObject CameraObject;
    public Transform PhoneHoldPos;
    public Transform PhoneCameraPos;
    public Transform SelfiCamPos;
    public Transform BackCamPos;
    public Transform PhoneIKTarget;
    Controller CTRL_Script;
    GlobalObjects Global_Script;
    Health health;
    [HideInInspector] public bool AllowPhone = true;
	// Use this for initialization
	void Start () {
        CTRL_Script = GetComponent<Controller>();
        health = GetComponent<Health>();
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
	}
	
	// Update is called once per frame
	void Update () {
        ActionLogic();
        if (CTRL_Script.InCar)
        {
            if (health.alive)
            {
                if (CTRL_Script.GetInput && AllowPhone)
                {
                    ActionCommands();
                }
                else
                {
                    isPhoneOn = false;
                }
            }
            else
            {
                isPhoneOn = false;
            }
        }
        else {
            if (health.alive && !CTRL_Script.Falling)
            {
                if (CTRL_Script.GetInput && AllowPhone)
                {
                    ActionCommands();
                }
                else
                {
                    isPhoneOn = false;
                }
            }
            else
            {
                isPhoneOn = false;
            }
        }
    }

    void ActionLogic() {
        if (isPhoneOn)
        {
            if (TakingPicture)
            {
                PhoneObject.transform.parent = PhoneCameraPos.parent;
                PhoneObject.transform.localPosition = PhoneCameraPos.localPosition;
                PhoneObject.transform.localRotation = PhoneCameraPos.localRotation;
                CameraObject.SetActive(true);
                if (Selfie)
                {
                    CameraObject.transform.localPosition = SelfiCamPos.localPosition;
                    CameraObject.transform.localRotation = SelfiCamPos.localRotation;
                }
                else
                {
                    CameraObject.transform.localPosition = BackCamPos.localPosition;
                    CameraObject.transform.localRotation = BackCamPos.localRotation;
                }

                if (Input.GetButtonDown("PhoneCamSwitch")) {
                    Selfie = !Selfie;
                }
            }
            else {
                CameraObject.SetActive(false);
                PhoneObject.transform.parent = PhoneHoldPos.parent;
                PhoneObject.transform.localPosition = PhoneHoldPos.localPosition;
                PhoneObject.transform.localRotation = PhoneHoldPos.localRotation;
            }
            CTRL_Script.InMenu = true;
            Global_Script.PhoneUI.SetActive(true);
            Invoke("TurnOnPhone", 0.5f);
        }
        else {
            CancelInvoke("TurnOnPhone");
            CTRL_Script.InMenu = false;
            Global_Script.PhoneUI.SetActive(false);
            PhoneObject.SetActive(false);
            TakingPicture = false;
            Selfie = false;
            CameraObject.SetActive(false);
        }
    }

    void TurnOnPhone() {
        PhoneObject.SetActive(true);
    }

    void ActionCommands() {
        if (Input.GetButtonDown("PhoneOn"))
        {
            if (!isPhoneOn)
            {
                isPhoneOn = true;
            }
        }
        if (Input.GetButtonDown("PhoneBack") || Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            if (isPhoneOn)
            {
                if (Global_Script.HomePage.activeSelf)
                {
                    isPhoneOn = false;
                }
                else
                {
                    //Go Back in Menu
                    Global_Script.GoBack();
                    TakingPicture = false;
                }
            }
        }
    }
}
