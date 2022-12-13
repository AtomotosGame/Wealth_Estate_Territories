// THE DIVISION KIT 1.8
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHealth : MonoBehaviour {
    public VehiclManager CarManager;
    public float health; // Car Health
    public bool Invincible = false;
    public bool Exploded; // Check if car exploded
    public GameObject Fire; // Fire Gameobject
    public GameObject Emission; // Explosion Prefab
    public Transform SpawnPosition; // Explosion spawn point
	
	// Update is called once per frame
	void Update () {
        if (!Exploded)
        {
            if (health < 50)
            {
                // start fire if health less than 50
                Fire.SetActive(true);
                if (health <= 0)
                {
                    // if health less than 0
                    if (!Exploded)
                    {
                        GameObject Explosion = (GameObject)(Instantiate(Emission, SpawnPosition.position, SpawnPosition.rotation));
                        GameObject.Destroy(Explosion, 3);
                        Exploded = true;
                    }
                }
            }
            else
            {
                Fire.SetActive(false);
            }
        }
        else
        {
            Fire.SetActive(true);
            foreach (Transform door in CarManager.Doors)
            {
                if (door.GetComponent<DoorInteraction>().User) {
                    door.GetComponent<DoorInteraction>().User.GetComponent<Health>().Takedamage(100 , null);
                    door.GetComponent<DoorInteraction>().User.GetComponent<Action_Car>().MySit = null;
                    door.GetComponent<DoorInteraction>().User = null;
                }
            }
        }
	}

    public void Repair() {
        health = 100;
        Exploded = false;
        Fire.SetActive(false);
    }

    public void Takedamage(float damage)
    {
        if (!Exploded)
        {
            if (!Invincible)
            {
                // take damage from hit
                health -= (damage*0.2f);
            }
        }
    }
}
