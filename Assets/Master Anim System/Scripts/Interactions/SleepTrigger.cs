using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepTrigger : MonoBehaviour {
    public GlobalObjects GameMan;
    [HideInInspector] public bool sitTaken;
    [HideInInspector] public GameObject SleepPlayer;
    public Transform SleepPosition;
    private const float m_sleepMatchTargetStart = 0.1f;
    private const float m_sleepMatchTargetStop = 0.3f;
    AnimatorStateInfo stateInfo;


    void Update()
    {
        if (SleepPlayer)
        {
            ProcessMatchTarget();
        }

        if (sitTaken)
        {
            if (SleepPlayer != null)
            {
                if (SleepPlayer == GameMan.LocalPlayer)
                {
                    if (Input.GetButtonDown("Interact"))
                    {
                        GameMan.LocalPlayer.GetComponent<Controller>().Sleeping = false;
                        SleepPlayer = null;
                        sitTaken = false;
                    }
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!sitTaken)
        {
            if (other.gameObject == GameMan.LocalPlayer)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    SleepPlayer = other.gameObject;
                    GameMan.LocalPlayer.GetComponent<Animator>().SetTrigger("Sleep");
                    GameMan.LocalPlayer.GetComponent<Controller>().Sleeping = true;
                    Invoke("TakeSleep", 0.5f);
                }
            }
        }
    }

    void TakeSleep()
    {
        sitTaken = true;
    }

    void ProcessMatchTarget()
    {
        if (SleepPlayer.GetComponent<Animator>().IsInTransition(0))
            return;

        stateInfo = SleepPlayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("GetIn_Bed"))
        {
            SleepPlayer.transform.rotation = Quaternion.Lerp(SleepPlayer.transform.rotation, SleepPosition.rotation, 0.01f);
            SleepPlayer.GetComponent<Animator>().MatchTarget(SleepPosition.position, new Quaternion(), AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), m_sleepMatchTargetStart, m_sleepMatchTargetStop);
        }
    }
}
