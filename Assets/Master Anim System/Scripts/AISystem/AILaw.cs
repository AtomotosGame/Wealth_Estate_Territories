using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILaw : MonoBehaviour {
    GlobalObjects Global_Script;
    MASAIBrain AI_Brains;
	// Use this for initialization
	void Start () {
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
        AI_Brains = GetComponent<MASAIBrain>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Global_Script.WantedLevel != 0)
        {
            AI_Brains.Target = Global_Script.LocalPlayer.transform;
        }
        else
        {
            if (AI_Brains.Target)
            {
                if (AI_Brains.Target == Global_Script.LocalPlayer.transform)
                {
                    AI_Brains.Target = null;

                }
            }
            if (AI_Brains.FollowTarget)
            {
                if (AI_Brains.FollowTarget == Global_Script.LocalPlayer.transform)
                {
                    AI_Brains.FollowTarget = null;
                }
            }
        }
	}
}
