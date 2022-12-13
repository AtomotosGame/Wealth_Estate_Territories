using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Melee : MonoBehaviour
{
    Controller CTRL_Script;
    Ray DamageRay;
    RaycastHit DamageHit;

    // Use this for initialization
    void Start()
    {
        CTRL_Script = GetComponent<Controller>();
    }

    public void DetectMeleeDamage()
    {
        DamageRay = new Ray(CTRL_Script.ParkourDetector.position, CTRL_Script.ParkourDetector.forward);

        if (Physics.Raycast(DamageRay, out DamageHit, CTRL_Script.prkrdtector))
        {
            if (DamageHit.collider.GetComponent<Health>())
            {
                DamageHit.collider.GetComponent<Health>().Takedamage(10, this.gameObject);
                DamageHit.collider.GetComponent<Health>().ReactDamage();
                Debug.Log("Hit Melee");
            }
        }
    }

}
