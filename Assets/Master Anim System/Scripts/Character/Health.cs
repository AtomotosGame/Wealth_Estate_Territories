using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public float health = 100; // The health value for the player
    public bool Invincible; // check if player is in god mode
    public bool alive; // check if player is alive
    public bool canRespawn; // check if can respawn
    public Transform RespawnPosition;
    Animator anim; // The animator attached to the player
    public AudioClip hurtSFX; // Sound clip to be played when player is hit by a bullet 
    [HideInInspector] public GameObject DeathScreenUI; // The UI gameobject carrying the death Info
     public GameObject LastShooter;
    [HideInInspector] public GlobalObjects GameManager;
    public GameObject moneyOBJ;
    bool faded;
    bool respawned;
    bool droppedMoney = false;
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (health < 1)
        {
            // if dead
            alive = false;
            anim.SetBool("Dead", true);
        }
        else {
            // if alive
            alive = true;
            anim.SetBool("Dead", false);
        }

        if (canRespawn)
        {
            // if can respawn
            if (!alive)
            {
                if (!respawned)
                {
                    // if dead
                    Invoke("Respawn", 5f);
                    respawned = true;
                }
            }
        }
        else
        {
            if (!alive)
            {
                if (GetComponent<AICarAction>())
                {
                    if (GetComponent<AICarAction>().MySit)
                    {
                        GetComponent<AICarAction>().GetOut();
                    }
                }
                Destroy(this.gameObject, 15);
            }
        }

        //dropMoney
        if (moneyOBJ)
        {
            if (!alive)
            {
                if (!droppedMoney)
                {
                    DropMoney();
                    droppedMoney = true;
                }
            }
        }

        // if offline mode
        if (DeathScreenUI)
        {
            if (health < 1)
            {
                // if dead
                DeathScreenUI.SetActive(true);
            }
            else
            {
                // if alive
                DeathScreenUI.SetActive(false);
            }
        }
	}

    public void Takedamage(float damage , GameObject Shooter) {
        // take hit from bullet and/or explosions
        if (!Invincible)
        {
            // if not in god mode
            health = health - damage;
            AudioSource.PlayClipAtPoint(hurtSFX, transform.position, 0.7f);
        }
        LastShooter = Shooter;
    }

    public void ReactDamage() {
        anim.SetFloat("HitReactID", Random.Range(0, 4));
        anim.SetTrigger("HitReact");
    }

    void DropMoney() {
        Vector3 ReTarget = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        GameObject currentMoney = (GameObject)(Instantiate(moneyOBJ , ReTarget , transform.rotation));
        GameObject.Destroy(currentMoney, 15);
    }

    void Respawn() {
        // respawn mechanic
        if (RespawnPosition)
        {
            if (!faded)
            {
                GameManager.FadeAnim.SetTrigger("FadeTransit");
                faded = true;
            }
            else {
                GameManager.FadeAnim.ResetTrigger("FadeTransit");
            }
            Invoke("FadedRespawn", 1f);
        }
        else {
            health = 100;
            LastShooter = null;
        }
    }


    void FadedRespawn() {
        GameManager.WantedLevel = 0;
        GameManager.notorietyLevel = 0;
        transform.position = RespawnPosition.position;
        transform.rotation = RespawnPosition.rotation;
        health = 100;
        LastShooter = null;
        faded = false;
        respawned = false;
        droppedMoney = false;
        GetComponent<WeaponControl>().CurrentSlot = 0;
        GetComponent<Action_Car>().GetOut();
        GameManager.Purchase(500);
    }

    public void HealthGenerate()
    {
        // Health regeneration Function
        if (health < 70)
        {
            health += Time.deltaTime;
        }
    }

}
