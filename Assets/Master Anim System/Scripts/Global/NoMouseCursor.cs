using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMouseCursor : MonoBehaviour {
	// Use this for initialization
	void Start () {
        Screen.lockCursor = true;
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {
	}
}
