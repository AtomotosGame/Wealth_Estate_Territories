using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{
   float WeightValue;
    protected Animator animator;

    public bool ikActive = false;
    [HideInInspector]public Transform rightHandObj = null;
    [HideInInspector]public Transform leftHandObj = null;
    [HideInInspector]public Transform lookObj = null;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (ikActive)
        {
            if (WeightValue < 1)
            {
                WeightValue += 0.02f;
            }
            else
            {
                WeightValue = 1;
            }
        }
        else {
            if (WeightValue > 0)
            {
                WeightValue -= 0.4f;
            }
            else
            {
                WeightValue = 0;
            }
        }
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {
            // Set the look target position, if one has been assigned

            if (lookObj != null)
                {
                    animator.SetLookAtWeight(WeightValue * 0.3f);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, WeightValue);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, WeightValue);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, WeightValue);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, WeightValue);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
        }
    }
}
