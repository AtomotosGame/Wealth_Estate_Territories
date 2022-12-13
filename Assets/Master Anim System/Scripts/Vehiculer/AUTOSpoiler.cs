using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AUTOSpoiler : MonoBehaviour {
    public Animator anim;
    public VehiclManager CarManager;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (CarManager.speed > 60)
        {
            anim.SetBool("HS", true);
        }
        else {
            anim.SetBool("HS", false);
        }
	}
}
