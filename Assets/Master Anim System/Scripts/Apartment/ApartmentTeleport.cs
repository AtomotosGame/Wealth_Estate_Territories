using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentTeleport : MonoBehaviour {
    public ApartmentInfo Apartment_Set;
    public bool Inside;
    public Transform GoTo;
    Collider TheTouch;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Controller>())
        {
            Apartment_Set.GlobalMan.FadeAnim.SetTrigger("FadeTransit");
            TheTouch = other;
            Invoke("Teleport", 1);
        }
    }

    void Teleport() {
        if (Inside)
        {
            Apartment_Set.PlayerInApartment = true;
        }
        else
        {
            Apartment_Set.PlayerInApartment = false;
        }

        TheTouch.transform.position = GoTo.position;
        TheTouch.transform.rotation = GoTo.rotation;
    }
}
