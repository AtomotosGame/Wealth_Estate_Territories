using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVTrigger : MonoBehaviour {
    public GlobalObjects GameMan;
    public TV_Display TV;
    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == GameMan.LocalPlayer)
        {
            if (Input.GetButtonDown("Interact"))
            {
                Invoke("SwitchTV", 0.5f);
            }
        }
    }


    void SwitchTV()
    {
        TV.TV_On = !TV.TV_On;
    }
}
