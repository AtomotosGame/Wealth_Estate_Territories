using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIHieght : MonoBehaviour {
    public Animator anim;
    public AICarAction CAR_ACT;
    public Health health;
    public CapsuleCollider CapsuleC;
    public NavMeshAgent NavAgent;
    public Rigidbody rb;
    AnimatorStateInfo stateInfo;

    void Update()
    {
        if (health.alive)
        {
            if (anim.GetFloat("ParkourState") > 0)
            {
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("FallingLoop"))
                {
                    CapsuleC.enabled = true;
                    anim.applyRootMotion = false;
                    NavAgent.enabled = false;
                    rb.isKinematic = false;
                }
                else
                {
                    CapsuleC.enabled = false;
                    anim.applyRootMotion = true;
                    NavAgent.enabled = false;
                    rb.isKinematic = true;
                }
            }
            else
            {
                if (CAR_ACT.MySit)
                {
                    anim.applyRootMotion = true;
                    CapsuleC.enabled = false;
                    NavAgent.enabled = false;
                    rb.isKinematic = true;
                }
                else
                {
                    CapsuleC.enabled = true;
                    NavAgent.enabled = true;
                    anim.applyRootMotion = false;
                    rb.isKinematic = false;
                }
            }
        }
        else {
            CapsuleC.enabled = false;
            NavAgent.enabled = false;
            rb.isKinematic = true;
        }
    }
}
