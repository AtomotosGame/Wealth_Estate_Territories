using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentSale : MonoBehaviour
{
    public ApartmentInfo Apartment_Inf;
    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Apartment_Inf.GlobalMan.LocalPlayer)
        {
            Apartment_Inf.GlobalMan.ApartmentName.text = Apartment_Inf.ApartmentName;
            Apartment_Inf.GlobalMan.ApartmentDescription.text = Apartment_Inf.ApartmentDescription;
            Apartment_Inf.GlobalMan.ApartmentCost.text = Apartment_Inf.ApartmentCost.ToString();
            Apartment_Inf.GlobalMan.ApartmentSaleUI.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == Apartment_Inf.GlobalMan.LocalPlayer)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (Apartment_Inf.GlobalMan.Money > Apartment_Inf.ApartmentCost)
                {
                    Apartment_Inf.GlobalMan.Purchase(Apartment_Inf.ApartmentCost);
                    Apartment_Inf.GlobalMan.ApartmentSaleUI.SetActive(false);
                    Apartment_Inf.PurchasedApartment = true;
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}
