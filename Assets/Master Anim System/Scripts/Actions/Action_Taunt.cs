using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Taunt : MonoBehaviour {
    Animator Anim;
    public WeaponControl Weap_Script;
    public Controller CTRL_Script;
	// Use this for initialization
	void Start () {
        Anim = GetComponent<Animator>();

	}

    // Update is called once per frame
    void Update()
    {
        if (Anim.GetBool("Dead") == false)
        {
            if (!CTRL_Script.Aim && !CTRL_Script.Attack && !CTRL_Script.Agressive && !CTRL_Script.InCar && !CTRL_Script.Swim && !CTRL_Script.Ladder && !CTRL_Script.Skydiving)
            {
                if (Anim.GetFloat("ParkourState") < 0.1f)
                {
                    if (Input.GetKey(KeyCode.CapsLock))
                    {
                        Anim.SetBool("ActionTaunt", true);
                        Weap_Script.CurrentSlot = 0;
                    }
                    else
                    {
                        Anim.SetBool("ActionTaunt", false);
                    }
                }
                else
                {
                    Anim.SetBool("ActionTaunt", false);
                }
            }
            else
            {
                Anim.SetBool("ActionTaunt", false);
            }
        }
        else
        {
            Anim.SetBool("ActionTaunt", false);
        }
    }
}
