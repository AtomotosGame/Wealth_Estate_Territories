using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PissTrigger : MonoBehaviour
{
    public GlobalObjects GameMan;
    public Transform SitPosition;
    [HideInInspector] public GameObject SitPlayer;

    void OnTriggerStay(Collider other)
    {
        if (SitPlayer == null)
        {
            if (other.gameObject == GameMan.LocalPlayer)
            {
                if (GameMan.LocalPlayer.GetComponent<Controller>().Pissing == false)
                {
                    if (Input.GetButtonDown("Interact"))
                    {
                        SitPlayer = other.gameObject;
                        GameMan.LocalPlayer.GetComponent<Controller>().Pissing = true;
                        GameMan.LocalPlayer.transform.position = SitPosition.position;
                        GameMan.LocalPlayer.transform.rotation = SitPosition.rotation;
                        Invoke("StopPiss", 7);
                    }
                }
            }
        }
    }

    void StopPiss()
    {
        SitPlayer.GetComponent<Controller>().Pissing = false;
        SitPlayer = null;
    }
}
