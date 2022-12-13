using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_DropWeapon : MonoBehaviour {
    WeaponControl WP_Script;
    Controller CTRL_Script;
    // Use this for initialization
    void Start () {
        WP_Script = GetComponent<WeaponControl>();
        CTRL_Script = GetComponent<Controller>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!CTRL_Script.InCar)
        {
            if (WP_Script.CurrentGun != null)
            {
                if (Input.GetButtonDown("ThrowInt"))
                {
                    WP_Script.CurrentGun.GetComponent<WeaponInfo>().GunUser = null;
                    WP_Script.CurrentGun.transform.parent = null;
                    WP_Script.CurrentGun = null;
                    if (WP_Script.CurrentSlot == 1)
                    {
                        WP_Script.Pistol = null;
                    }
                    if (WP_Script.CurrentSlot == 2)
                    {
                        WP_Script.Heavy = null;
                    }
                    if (WP_Script.CurrentSlot == 3)
                    {
                        WP_Script.SMG = null;
                    }
                    if (WP_Script.CurrentSlot == 4)
                    {
                        WP_Script.Assault = null;
                    }
                    if (WP_Script.CurrentSlot == 5)
                    {
                        WP_Script.Sniper = null;
                    }
                    if (WP_Script.CurrentSlot == 6)
                    {
                        WP_Script.Launchers = null;
                    }

                }
            }
        }
	}
}
