using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraWheel : MonoBehaviour {
    public Transform CopyWheel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = CopyWheel.rotation;
	}
}
