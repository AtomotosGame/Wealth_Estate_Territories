using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaddeTriggers : MonoBehaviour
{
    GlobalObjects GameMan;
    public bool FromTop; // Bool to start Climbing ladder
    public Transform LadderPos; // Start Position for ladder
    public Transform HandPos; // Start Position for ladder
    public Transform FootPos; // Start Position for ladder


    void Start()
    {
        GameMan = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameMan.LocalPlayer)
        {
            if (GameMan.LocalPlayer.GetComponent<Controller>().Ladder == true)
            {
                // if player is climbing ladder
                if (FromTop)
                {
                    // if it starts from bottom
                    GameMan.LocalPlayer.GetComponent<Controller>().Ladder = false;
                    GameMan.LocalPlayer.GetComponent<Controller>().LadderStart = 0;
                    GameMan.LocalPlayer.GetComponent<Controller>().LadderHandPos = HandPos;
                    GameMan.LocalPlayer.GetComponent<Controller>().LadderFootPos = null;
                }
                else
                {
                    // if it starts from top
                    GameMan.LocalPlayer.GetComponent<Controller>().Ladder = false;
                    GameMan.LocalPlayer.GetComponent<Controller>().LadderStart = 1;
                    GameMan.LocalPlayer.GetComponent<Controller>().LadderHandPos = HandPos;
                    GameMan.LocalPlayer.GetComponent<Controller>().LadderFootPos = FootPos;
                }
            }
            else
            {
                if (GameMan.LocalPlayer.GetComponent<Animator>().GetFloat("BZWep") == 0)
                {
                    // if press the interaction button
                    if (!FromTop)
                    {
                        // if it starts from bottom
                        GameMan.LocalPlayer.GetComponent<Controller>().LadderPos = LadderPos;
                        GameMan.LocalPlayer.GetComponent<Controller>().LadderHandPos = HandPos;
                        GameMan.LocalPlayer.GetComponent<Controller>().LadderFootPos = null;
                        GameMan.LocalPlayer.GetComponent<Controller>().Ladder = true;
                        GameMan.LocalPlayer.GetComponent<Controller>().LadderStart = 0;
                    }
                    else
                    {
                        // if it starts from top
                        GameMan.LocalPlayer.GetComponent<Controller>().LadderPos = LadderPos;
                        GameMan.LocalPlayer.GetComponent<Controller>().LadderHandPos = HandPos;
                        GameMan.LocalPlayer.GetComponent<Controller>().LadderFootPos = FootPos;
                        GameMan.LocalPlayer.GetComponent<Controller>().Ladder = true;
                        GameMan.LocalPlayer.GetComponent<Controller>().LadderStart = 1;
                    }
                }
            }
        }
    }
}
