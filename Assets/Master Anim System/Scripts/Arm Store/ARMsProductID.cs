using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARMsProductID : MonoBehaviour {
    [Header ("Item Information")]
    public string GunName;
    public int Price;

    [Header("General Setting")]
    public GlobalObjects Game_Manager;
    [Range(1,7)]
    public int SlotID = 1;
    public GameObject Item_Prefab;

    [Header("UI Setting")]
    public Text Text_UI;
    public Text Price_UI;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        CheckInformation();
        if (Game_Manager.Money < Price)
        {
            Price_UI.color = Color.red;
        }
        else {
            Price_UI.color = Color.black;
        }
	}

    public void CheckInformation() {
        Text_UI.text = GunName;
        if (Price > 0)
        {
            Price_UI.text = "$" + Price.ToString();
        }
        else {
            Price_UI.text = "FREE";
        }

    }

    public void PurchaseItem() {
        if (Game_Manager.Money >= Price)
        {
            AssignArmID();
        }
    }

    void AssignArmID() {
        if (SlotID == 1)
        {
            if (Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Pistol == null)
            {
                GameObject PurchasedItem = (GameObject)Instantiate(Item_Prefab, Vector3.zero, Quaternion.identity);
                Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Pistol = PurchasedItem;
                PurchasedItem.transform.parent = Game_Manager.LocalPlayer.transform;
                Game_Manager.Purchase(Price);
            }
            else {
                if (GunName == Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Pistol.GetComponent<WeaponInfo>().GunName)
                {
                    Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Pistol.GetComponent<WeaponInfo>().megazine += 5;
                    Game_Manager.Purchase(Price);
                }
            }
        }
        if (SlotID == 2)
        {
            if (Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Heavy == null)
            {
                GameObject PurchasedItem = (GameObject)Instantiate(Item_Prefab, Vector3.zero, Quaternion.identity);
                Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Heavy = PurchasedItem;
                PurchasedItem.transform.parent = Game_Manager.LocalPlayer.transform;
                Game_Manager.Purchase(Price);
            }
            else
            {
                if (GunName == Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Heavy.GetComponent<WeaponInfo>().GunName)
                {
                    Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Heavy.GetComponent<WeaponInfo>().megazine += 5;
                    Game_Manager.Purchase(Price);
                }
            }
        }
        if (SlotID == 3)
        {
            if (Game_Manager.LocalPlayer.GetComponent<WeaponControl>().SMG == null)
            {
                GameObject PurchasedItem = (GameObject)Instantiate(Item_Prefab, Vector3.zero, Quaternion.identity);
                Game_Manager.LocalPlayer.GetComponent<WeaponControl>().SMG = PurchasedItem;
                PurchasedItem.transform.parent = Game_Manager.LocalPlayer.transform;
                Game_Manager.Purchase(Price);
            }
            else
            {
                if (GunName == Game_Manager.LocalPlayer.GetComponent<WeaponControl>().SMG.GetComponent<WeaponInfo>().GunName)
                {
                    Game_Manager.LocalPlayer.GetComponent<WeaponControl>().SMG.GetComponent<WeaponInfo>().megazine += 5;
                    Game_Manager.Purchase(Price);
                }
            }
        }
        if (SlotID == 4)
        {
            if (Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Assault == null)
            {
                GameObject PurchasedItem = (GameObject)Instantiate(Item_Prefab, Vector3.zero, Quaternion.identity);
                Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Assault = PurchasedItem;
                PurchasedItem.transform.parent = Game_Manager.LocalPlayer.transform;
                Game_Manager.Purchase(Price);
            }
            else
            {
                if (GunName == Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Assault.GetComponent<WeaponInfo>().GunName)
                {
                    Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Assault.GetComponent<WeaponInfo>().megazine += 5;
                    Game_Manager.Purchase(Price);
                }
            }
        }
        if (SlotID == 5)
        {
            if (Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Sniper == null)
            {
                GameObject PurchasedItem = (GameObject)Instantiate(Item_Prefab, Vector3.zero, Quaternion.identity);
                Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Sniper = PurchasedItem;
                PurchasedItem.transform.parent = Game_Manager.LocalPlayer.transform;
                Game_Manager.Purchase(Price);
            }
            else
            {
                if (GunName == Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Sniper.GetComponent<WeaponInfo>().GunName)
                {
                    Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Sniper.GetComponent<WeaponInfo>().megazine += 5;
                    Game_Manager.Purchase(Price);
                }
            }
        }
        if (SlotID == 6)
        {
            if (Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Launchers == null)
            {
                GameObject PurchasedItem = (GameObject)Instantiate(Item_Prefab, Vector3.zero, Quaternion.identity);
                Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Launchers = PurchasedItem;
                PurchasedItem.transform.parent = Game_Manager.LocalPlayer.transform;
                Game_Manager.Purchase(Price);
            }
            else
            {
                if (GunName == Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Launchers.GetComponent<WeaponInfo>().GunName)
                {
                    Game_Manager.LocalPlayer.GetComponent<WeaponControl>().Launchers.GetComponent<WeaponInfo>().megazine += 5;
                    Game_Manager.Purchase(Price);
                }
            }
        }
        if (SlotID == 7)
        {
            if (!Game_Manager.LocalPlayer.GetComponent<Action_SkyDive>().HasParachute)
            {
                Game_Manager.LocalPlayer.GetComponent<Action_SkyDive>().HasParachute = true;
                Game_Manager.Purchase(Price);
            }
        }
    }
}
