using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCharacterDamage : MonoBehaviour {
    public VehiclManager Veh_Script;
    public float MinSpeed2Dmage;
    // Update is called once per frame


    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collided");
        if (other.collider.GetComponent<Health>())
        {
            if (other.collider.GetComponent<Health>().alive)
            {
                Debug.Log("Collided with Character");
                if (Veh_Script.speed >= MinSpeed2Dmage)
                {
                    other.collider.GetComponent<Health>().Takedamage(100, null);
                }
            }
        }
    }
}
