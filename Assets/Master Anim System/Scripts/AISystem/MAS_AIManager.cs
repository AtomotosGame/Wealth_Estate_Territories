using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAS_AIManager : MonoBehaviour {
    public bool TemporaryAI;
    public float VisibleDistance = 250f;
    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (TemporaryAI)
        {

            if ((transform.position - Camera.main.transform.position).sqrMagnitude > VisibleDistance * VisibleDistance)
            {
                DestroyAI();
            }
        }
    }

    public void DestroyAI() {
        Destroy(this.gameObject);
    }
}
