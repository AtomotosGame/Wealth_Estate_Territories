using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHieght : MonoBehaviour
{
    public Animator anim;
    public CharacterController ControllerC;
    public Rigidbody RB_Playa;
    [Header("Default Details")]
    public Vector3 ColliderCentre;
    public float ColliderHieght = 1.76f;
    // Use this for initialization
    void Start()
    {
        RB_Playa = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetFloat("ParkourState") > 0)
        {
            ControllerC.center = new Vector3(0, anim.GetFloat("ColY"), 0.06f);
            ControllerC.height = anim.GetFloat("ColH");
        }
        else
        {
            ControllerC.center = ColliderCentre;
            ControllerC.height = ColliderHieght;
        }

        if (GetComponent<Controller>().Swim || GetComponent<Controller>().Ladder)
        {
            RB_Playa.useGravity = false;
        }
        else
        {
            RB_Playa.useGravity = true;
        }

        if (GetComponent<Action_Car>().MySit == null)
        {
            if (anim.GetFloat("ColOff") != 0)
            {
                ControllerC.enabled = false;
            }
            else
            {
                ControllerC.enabled = true;
            }
        }
    }
}
