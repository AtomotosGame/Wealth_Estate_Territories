using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointDistribution : MonoBehaviour
{

    [System.Serializable]
    public class NPC_SpawnInfo
    {
        public GameObject[] NPC_prefab;
        public GameObject COP_prefab;
    }

    [System.Serializable]
    public class NPC_SettingInfo
    {
        public float SpawnerDistance = 50f;
        public Transform SpawnPos;
        public WayPointDistribution Destination1;
        public WayPointDistribution Destination2;
    }

    [Header("Info")]
    public bool InSight;
    public bool Spawned;
    public bool Schecked;
    public Transform NextWayPoint;
    GlobalObjects Global_Script;

    [Header("Options")]
    public NPC_SpawnInfo NPCInformation;
    public NPC_SettingInfo NPCSettings;

    // Use this for initialization
    void Start()
    {
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
    }

    // Update is called once per frame
    void Update()
    {
        InSightChecker();
        SpawnChecker();
    }

    void InSightChecker()
    {
        if (gameObject.GetComponent<Renderer>().isVisible)
        {
            InSight = true;
        }
        else
        {
            InSight = false;
        }
    }

    public void SetDestination(int decision) {
        if (decision > 3)
        {
            NextWayPoint = NPCSettings.Destination1.transform;
        }
        else {
            NextWayPoint = NPCSettings.Destination2.transform;
        }
    }

    void SpawnChecker()
    {
        if (!InSight)
        {
            if ((transform.position - Camera.main.transform.position).sqrMagnitude > 2000)
            {
                if ((transform.position - Camera.main.transform.position).sqrMagnitude < NPCSettings.SpawnerDistance)
                {
                    if (!Schecked)
                    {
                        Spawn(Random.Range(0, 5));
                        Debug.Log("Checking");
                    }
                }
                else
                {
                    Schecked = false;
                    Spawned = false;
                }
            }
            else
            {
                Schecked = false;
                Spawned = false;
            }
        }
        else
        {
            if ((transform.position - Camera.main.transform.position).sqrMagnitude > 3000)
            {
                if ((transform.position - Camera.main.transform.position).sqrMagnitude < 50000)
                {
                    if (!Schecked)
                    {
                        Spawn(Random.Range(0, 5));
                        Debug.Log("Checking");
                    }
                }
                else {
                    Schecked = false;
                    Spawned = false;
                }
            }
            else {
                Schecked = false;
                Spawned = false;
            }
        }

    }

    void Spawn(int decision)
    {
        if (decision > 3)
        {
            if (!Spawned)
            {
                if (Global_Script.WantedLevel != 0)
                {
                    SpawnCops();
                    Spawned = true;
                }
                else
                {
                    SpawnNPC();
                    Spawned = true;
                }
            }
        }
        else {
            if (Global_Script.AIsInScene.Length < Global_Script.AILimit)
            {
                if (!Spawned)
                {
                    SpawnNPC();
                    Spawned = true;
                }
            }
        }
        Schecked = true;
    }

    public void SpawnNPC() {
        GameObject NPC = (GameObject)(Instantiate(NPCInformation.NPC_prefab[Random.Range(0, NPCInformation.NPC_prefab.Length)], NPCSettings.SpawnPos.position, NPCSettings.SpawnPos.rotation));
        NPC.GetComponent<MAS_AIManager>().TemporaryAI = true;
        SetDestination(Random.Range(0, 5));
        NPC.GetComponent<MASAIBrain>().WayPoint = NextWayPoint;
    }

    public void SpawnCops() {
        GameObject NPC = (GameObject)(Instantiate(NPCInformation.COP_prefab, NPCSettings.SpawnPos.position, NPCSettings.SpawnPos.rotation));
        NPC.GetComponent<MAS_AIManager>().TemporaryAI = true;
    }
}
