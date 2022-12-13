using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MoneyPickup : MonoBehaviour
{
    public int MoneyAmount;
    GlobalObjects Global_Script;
    // Use this for initialization
    void Start()
    {
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
        MoneyAmount = Random.Range(15, 500);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Controller>()) {
            if (other.gameObject == Global_Script.LocalPlayer.gameObject)
            {
                Global_Script.AddMoney(MoneyAmount);
                Destroy(transform.parent.gameObject);
            }
            else {
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
