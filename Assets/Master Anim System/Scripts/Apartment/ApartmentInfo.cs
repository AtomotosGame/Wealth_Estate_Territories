using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentInfo : MonoBehaviour {
    public GlobalObjects GlobalMan;
    [HideInInspector] public bool PurchasedApartment = false;
    public string ApartmentName;
    public int ApartmentCost;
    public string ApartmentDescription;
    public bool PlayerInApartment;
    public GameObject ApartmentInterior;
    public GameObject ExclusiveTriggers;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (PlayerInApartment)
        {
            ApartmentInterior.SetActive(true); 
        }
        else {
            ApartmentInterior.SetActive(false);
        }

        if (PurchasedApartment)
        {
            ExclusiveTriggers.SetActive(true);
        }
        else {
            ExclusiveTriggers.SetActive(false);
        }
	}
}
