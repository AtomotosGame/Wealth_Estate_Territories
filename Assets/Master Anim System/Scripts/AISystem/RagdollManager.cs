using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class RagdollManager : MonoBehaviour
{
    public bool Ragdoll = false;
    AICarAction AC_Script;
    public Rigidbody[] BoneRig;

    // Use this for initialization
    void Start()
    {
        AC_Script = GetComponent<AICarAction>();
        //BoneRig = GetComponentsInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!AC_Script.MySit)
        {
            if (Ragdoll)
            {
                if (!GetComponent<Health>().alive)
                {
                    this.GetComponent<Animator>().enabled = false;
                    foreach (Rigidbody rb in BoneRig)
                    {
                        if (rb != this.GetComponent<Rigidbody>())
                        {
                            rb.isKinematic = false;
                            rb.GetComponent<Collider>().enabled = true;
                        }
                    }
                }
                else {
                    this.GetComponent<Animator>().enabled = true;
                    foreach (Rigidbody rb in BoneRig)
                    {
                        if (rb != this.GetComponent<Rigidbody>())
                        {
                            rb.isKinematic = true;
                            rb.GetComponent<Collider>().enabled = false;
                        }
                    }
                }
            }
        }
        else {
            this.GetComponent<Animator>().enabled = true;
            foreach (Rigidbody rb in BoneRig)
            {
                rb.isKinematic = true;
                rb.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
