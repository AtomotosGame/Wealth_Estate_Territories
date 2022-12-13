using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public enum CarType {

    Civillian,
    Gang,
    Law
}

public class VehiclManager : MonoBehaviour
{
    [Range(0, 2)]
    public int TypeID;
    public CarType VehicleType;
    [HideInInspector] public bool UserInput;
    public bool Locked = false;
    public Transform IKTargetLeft;
    public Transform IKTargetRight;
    [HideInInspector] public CarUserControl CUC;
    [HideInInspector] public CarAIControl CUCAI;
    [HideInInspector] public HelicopterController HUC;
    [HideInInspector] public RMCRealisticMotorcycleController RMC;
    [HideInInspector] public bool AIDriver;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public GameObject Driver;

    GlobalObjects Global_Script;
    public float speed;
    public float VehicleSize = 1;
    public bool LightsOn;
    [HideInInspector] public bool doorOpen;
    public Light[] Lights;
    public AudioSource AS_Horn;
    public AudioClip AC_Horn;
    public GameObject Siren;
    bool sirenOn = false;
    public Transform[] Doors;
    int dooropened;
    GameObject ColBox;



    void Start()
    {
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
        if (TypeID != 1 && TypeID != 2)
        {
            ColBox = new GameObject(gameObject.name + "IdleCollider");
            ColBox.layer = 14;
            ColBox.AddComponent<BoxCollider>();
            ColBox.GetComponent<BoxCollider>().size = GetComponent<BoxCollider>().size * 1.01f;
            ColBox.GetComponent<BoxCollider>().center = GetComponent<BoxCollider>().center * 1.01f;
        }
        rb = this.GetComponent<Rigidbody>();
        if (TypeID == 0)
        {
            CUC = GetComponent<CarUserControl>();
            CUCAI = GetComponent<CarAIControl>();
        }
        if (TypeID == 1)
        {
            HUC = GetComponent<HelicopterController>();
        }
        if (TypeID == 2)
        {
            RMC = GetComponent<RMCRealisticMotorcycleController>();
        }

        if (AS_Horn != null)
        {
            AS_Horn.clip = AC_Horn;
        }
    }


    void Update()
    {
        SpeedNonMove();
        Headlight();
        SpeedInput();

        if (Siren)
        {
            Siren.SetActive(sirenOn);
        }

        for (int i = 0; i < Doors.Length; i++)
        {
            if (Doors[i].GetComponent<DoorInteraction>().Door_State == Door_Int.Opened)
            {
                dooropened = i;
                doorOpen = true;
            }
            else
            {
                if (i == dooropened)
                {
                    doorOpen = false;
                }
            }

            if (TypeID != 2)
            {
                if (Doors[i].GetComponent<DoorInteraction>().isDriver)
                {
                    if (Doors[i].GetComponent<DoorInteraction>().User) {
                        Driver = Doors[i].GetComponent<DoorInteraction>().User;
                    }
                }
            }
        }

        if (UserInput)
        {
            if (TypeID != 2)
            {
                if (!doorOpen)
                {
                    if (AIDriver)
                    {
                        if (TypeID == 0)
                        {
                            if (!CUCAI.enabled)
                            {
                                this.GetComponent<CarAudio>().VehicleOn = true;
                                //    this.GetComponent<CarController>().Move(0, 1, 0, 0);
                                CUCAI.enabled = true;
                                CUC.enabled = false;
                            }
                        }
                    }
                    else
                    {

                        InteractiveInput();
                        HSCamShake(70);
                        if (TypeID == 0)
                        {
                            if (!CUC.enabled)
                            {
                                this.GetComponent<CarAudio>().VehicleOn = true;
                                this.GetComponent<CarController>().Move(0, 0, 0, 0);
                                CUC.enabled = true;
                                CUCAI.enabled = false;
                            }
                        }
                        if (TypeID == 1)
                        {
                            HUC.InputOn = true;
                        }
                    }
                }
                else
                {
                    if (TypeID == 0)
                    {
                        CUC.enabled = false;
                        CUCAI.enabled = false;
                        this.GetComponent<CarController>().Move(0, 0, 1, 1);
                        this.GetComponent<CarAudio>().VehicleOn = false;
                    }
                }
            }
        }
        else
        {
            if (TypeID == 0)
            {
                CUC.enabled = false;
                CUCAI.enabled = false;
                this.GetComponent<CarAudio>().VehicleOn = false;
            }
            if (TypeID == 1)
            {
                HUC.InputOn = false;
            }
        }

        if (TypeID == 2)
        {
            if (!AIDriver)
            {
                HSCamShake(80);
                InteractiveInput();
                for (int i = 0; i < Doors.Length; i++)
                {
                    if (Doors[i].GetComponent<DoorInteraction>().User)
                    {
                        dooropened = i;
                        if (Doors[i].GetComponent<DoorInteraction>().User.GetComponent<Controller>().InCar)
                        {
                            RMC.InputOn = true;
                        }
                    }
                    else
                    {
                        if (i == dooropened)
                        {
                            RMC.InputOn = false;
                        }
                    }
                }
            }
        }
    }

    void HSCamShake(float TopSpeed)
    {
        if (speed > TopSpeed)
        {
            Global_Script.cam.GetComponent<CameraRig>().ShakeCamera(0.01f, 0.2f);
        }
    }

    void InteractiveInput()
    {
        if (Input.GetButtonDown("vLightSwitch"))
        {
            LightsOn = !LightsOn;
        }

        if (VehicleType != CarType.Law)
        {
            if (AS_Horn != null)
            {
                if (Input.GetButton("vHorn"))
                {
                    if (!AS_Horn.isPlaying)
                    {
                        AS_Horn.Play();
                    }
                }
                else
                {
                    if (AS_Horn.isPlaying)
                    {
                        AS_Horn.Stop();
                    }
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("vHorn"))
            {
                if (Siren)
                {
                    sirenOn = !sirenOn;
                }
            }


        }
    }

    void Headlight()
    {
        if (LightsOn)
        {
            foreach (Light light in Lights)
            {
                light.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Light light in Lights)
            {
                light.gameObject.SetActive(false);
            }
        }
    }

    void SpeedInput()
    {
        if (TypeID == 0)
        {
            speed = this.GetComponent<CarController>().CurrentSpeed;
        }
        if (TypeID == 2)
        {
            speed = RMC.Speed;
        }
        if (TypeID == 1)
        {
            if (HUC.IsOnGround)
            {
                speed = 0;
            }
            else
            {
                speed = 100;
            }
        }

    }

    void SpeedNonMove()
    {
        if (TypeID == 0)
        {
            if (!UserInput)
            {
                rb.isKinematic = true;
                //  rb.constraints = RigidbodyConstraints.FreezeAll;
                ColBox.SetActive(true);
                ColBox.transform.position = transform.position;
                ColBox.transform.rotation = transform.rotation;
                this.GetComponent<CarController>().Move(0, 0, 1, 1);
            }
            else
            {
                rb.isKinematic = false;
                // rb.constraints = RigidbodyConstraints.None;
                ColBox.SetActive(false);
            }
        }
        if (TypeID == 1)
        {
            if (!UserInput)
            {
                if (HUC.IsOnGround)
                {
                    rb.isKinematic = true;
                }
                else {
                    rb.isKinematic = false;
                }
            }
            else
            {
                rb.isKinematic = false;
            }
        }
    }
}
