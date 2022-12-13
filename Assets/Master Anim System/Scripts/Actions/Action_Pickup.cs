using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Pickup : MonoBehaviour
{
    WeaponInfo WP_Info;
    GlobalObjects Global_Script;
    public AudioClip PickupSFX;
    // Use this for initialization
    void Start()
    {
        WP_Info = GetComponentInParent<WeaponInfo>();
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<WeaponControl>())
        {
            if (other.gameObject == Global_Script.LocalPlayer.gameObject)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (WP_Info.SlotID == 1)
                    {
                        if (other.GetComponent<WeaponControl>().Pistol == null)
                        {
                            AudioSource.PlayClipAtPoint(PickupSFX, transform.position, 1f);
                            other.GetComponent<WeaponControl>().Pistol = transform.parent.gameObject;
                            transform.parent.transform.parent = other.transform;
                        }
                    }
                    if (WP_Info.SlotID == 2)
                    {
                        if (other.GetComponent<WeaponControl>().Heavy == null)
                        {
                            AudioSource.PlayClipAtPoint(PickupSFX, transform.position, 1f);
                            other.GetComponent<WeaponControl>().Heavy = transform.parent.gameObject;
                            transform.parent.transform.parent = other.transform;
                        }
                    }
                    if (WP_Info.SlotID == 3)
                    {
                        if (other.GetComponent<WeaponControl>().SMG == null)
                        {
                            AudioSource.PlayClipAtPoint(PickupSFX, transform.position, 1f);
                            other.GetComponent<WeaponControl>().SMG = transform.parent.gameObject;
                            transform.parent.transform.parent = other.transform;
                        }
                    }
                    if (WP_Info.SlotID == 4)
                    {
                        if (other.GetComponent<WeaponControl>().Assault == null)
                        {
                            AudioSource.PlayClipAtPoint(PickupSFX, transform.position, 1f);
                            other.GetComponent<WeaponControl>().Assault = transform.parent.gameObject;
                            transform.parent.transform.parent = other.transform;
                        }
                    }
                    if (WP_Info.SlotID == 5)
                    {
                        if (other.GetComponent<WeaponControl>().Sniper == null)
                        {
                            AudioSource.PlayClipAtPoint(PickupSFX, transform.position, 1f);
                            other.GetComponent<WeaponControl>().Sniper = transform.parent.gameObject;
                            transform.parent.transform.parent = other.transform;
                        }
                    }
                    if (WP_Info.SlotID == 6)
                    {
                        if (other.GetComponent<WeaponControl>().Launchers == null)
                        {
                            AudioSource.PlayClipAtPoint(PickupSFX, transform.position, 1f);
                            other.GetComponent<WeaponControl>().Launchers = transform.parent.gameObject;
                            transform.parent.transform.parent = other.transform;
                        }
                    }
                }
            }
        }
    }
}
