using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSystem : MonoBehaviour {
    BoxCollider WaterCol;
    Vector3 WaterPoint;

	// Use this for initialization
	void Start () {
        WaterCol = this.GetComponent<BoxCollider>();
	}

    void OnTriggerStay(Collider other) {
        if (other.gameObject.GetComponent<Controller>()) {
            WaterPoint = other.transform.position;
            WaterPoint.y = (WaterCol.bounds.extents.y) + (WaterCol.transform.position.y) - 1.65f;
            other.transform.position = Vector3.Lerp(other.transform.position, WaterPoint, Time.deltaTime * 5);
            other.gameObject.GetComponent<Controller>().WaterSplash.transform.position = new Vector3(WaterPoint.x,WaterPoint.y + 1.65f , WaterPoint.z);
            other.gameObject.GetComponent<Controller>().Swim = true;

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Controller>())
        {
            other.gameObject.GetComponent<Controller>().Swim = false;
        }
    }
}

