using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitTrigger : MonoBehaviour {
    public float SitStyle;
    public GlobalObjects GameMan;
    [HideInInspector] public bool sitTaken;
    [HideInInspector] public GameObject SitPlayer;
    public Transform SitPosition;
    private const float m_sitMatchTargetStart = 0.08f;
    private const float m_sitMatchTargetStop = 0.8f;
    AnimatorStateInfo stateInfo;


    void Update()
    {
        if (SitPlayer) {
            ProcessMatchTarget();
        }

        if (sitTaken)
        {
            if (SitPlayer != null)
            {
                if (SitPlayer == GameMan.LocalPlayer)
                {
                    if (Input.GetButtonDown("Interact"))
                    {
                        GameMan.LocalPlayer.GetComponent<Controller>().Sitting = false;
                        SitPlayer = null;
                        sitTaken = false;
                    }
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!sitTaken) {
            if (other.gameObject == GameMan.LocalPlayer) {
                if (Input.GetButtonDown("Interact")) {
                    SitPlayer = other.gameObject;
                    GameMan.LocalPlayer.GetComponent<Animator>().SetTrigger("Sit");
                    GameMan.LocalPlayer.GetComponent<Controller>().Sitting = true;
                    Invoke("TakeSit", 0.5f);
                }
            }
        }
    }

    void TakeSit() {
        sitTaken = true;
    }

    void ProcessMatchTarget()
    {
        if (SitPlayer.GetComponent<Animator>().IsInTransition(0))
            return;

        stateInfo = SitPlayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("GetInSit"))
        {
            SitPlayer.transform.rotation = Quaternion.Lerp(SitPlayer.transform.rotation, SitPosition.rotation, 0.1f);
            SitPlayer.GetComponent<Animator>().MatchTarget(SitPosition.position, new Quaternion(), AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), m_sitMatchTargetStart, m_sitMatchTargetStop);
        }
    }
}
