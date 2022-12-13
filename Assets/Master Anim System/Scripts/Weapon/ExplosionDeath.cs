
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDeath : MonoBehaviour {
    public float radius = 5f;
    public float power = 10000000F;
    public float upwardsModifier = 0;
    public ForceMode forcemode;
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        Vector3 explosionPos = transform.position;
        if (other.GetComponent<Rigidbody>())
        {
            other.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, radius, 3.0F);
        }

        if (other.GetComponent<Hitmark>())
        {
            if (other.GetComponent<Hitmark>().HealthScript.alive)
            {
                other.GetComponent<Hitmark>().HealthScript.Takedamage(100, null);
            }
        }

        foreach (Collider col in Physics.OverlapSphere(transform.position, radius)) {
            if (col.GetComponent<Rigidbody>()) {
                col.GetComponent<Rigidbody>().AddExplosionForce(power, transform.position, radius, upwardsModifier, forcemode);
            }
        }
    }
}
