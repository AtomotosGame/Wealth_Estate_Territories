using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmStoreTrigger : MonoBehaviour {

    public GlobalObjects GameMan;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == GameMan.LocalPlayer)
        {
            if (!GameMan.InHardMenu)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    GameMan.GunStoreUI.SetActive(true);
                    GameMan.InHardMenu = true;
                }
            }
        }
    } 
}
