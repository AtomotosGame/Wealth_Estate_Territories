using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialGuideSystem : MonoBehaviour
{
    [System.Serializable]
    public class TutorialsSettings
    {
        [Header ("General")]
        [Range(0, 17)]
        public int CurrentStage;
        public GlobalObjects GameManager;

        [Header("UI Elements")]
        public GameObject[] AllMenus;
    }
    [System.Serializable]
    public class TutorialStages
    {
        public float DelayTimer;
        [Header("Welcome Menu Criteria")]
        public bool StartTutorial;

        [Header("Melee 1 Criteria")]
        public bool DidMelee;

        [Header("Melee 2 Criteria")]
        public bool ExitedMelee;

        [Header("Parkour 1 Criteria")]
        public bool pressedJump;

        [Header("Parkour 2 Criteria")]
        public bool vaulted;
        public bool climbed;
        public bool Steppedup;
        public bool GotUp;
        public bool Jumped;
        public Text ParkourPerformedUI;

        [Header("Weap 1 Criteria")]
        public bool hasPistol;
        public bool hasHeavy;
        public bool hasSMG;
        public bool hasAssault;
        public bool hasSniper;
        public bool hasRPG;
        public Text AcquiredUI;

        [Header("Weap 2 Criteria")]
        public bool Aimed;
        public bool Fired;

        [Header("Cover 1 Criteria")]
        public bool TookCover;

        [Header("Cover 2 Criteria")]
        public bool BlindFired;
        public bool AimedInCover;

        [Header("Vehicle 1 Criteria")]
        public bool EnteredVehicle;

        [Header("Money Stats Criteria")]
        public bool CheckedMoney;

        [Header("Mobile 1 Criteria")]
        public bool OpenedPhone;

        [Header("End Criteria")]
        public bool EndTutorial;
    }

    public TutorialsSettings Settings;
    public TutorialStages Stages;
    int NextStage;
    int parkournos;
    int weaponnos;


    // Update is called once per frame
    void Update()
    {
        SetupTutorialBars();
        if (!Stages.EndTutorial)
        {
            TutorialSetup();
        }
    }

    void TutorialSetup()
    {
        if (Settings.CurrentStage == 0)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Stages.StartTutorial = true;
                Settings.CurrentStage = 1;
            }
        }
        if (Settings.CurrentStage == 1)
        {
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Agressive)
            {
                Stages.DidMelee = true;
                NextStage = 2;
                Invoke("DelayedSwitch", Stages.DelayTimer);
            }
        }
        if (Settings.CurrentStage == 2)
        {
            if (!Settings.GameManager.LocalPlayer.GetComponent<Controller>().Agressive)
            {
                Stages.ExitedMelee = true;
                Settings.CurrentStage = 3;
            }
        }
        if (Settings.CurrentStage == 3)
        {
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Parkour)
            {
                Stages.pressedJump = true;
                Settings.CurrentStage = 4;
            }
        }
        if (Settings.CurrentStage == 4)
        {
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Parkour && Settings.GameManager.LocalPlayer.GetComponent<Controller>().ParkourID == 0)
            {
                if (!Stages.Jumped) {
                    parkournos += 1;
                    Stages.Jumped = true;
                }
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Parkour && Settings.GameManager.LocalPlayer.GetComponent<Controller>().ParkourID == 1)
            {
                if (!Stages.Steppedup)
                {
                    parkournos += 1;
                    Stages.Steppedup = true;
                }
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Parkour && Settings.GameManager.LocalPlayer.GetComponent<Controller>().ParkourID == 2)
            {
                if (!Stages.vaulted)
                {
                    parkournos += 1;
                    Stages.vaulted = true;
                }
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Parkour && Settings.GameManager.LocalPlayer.GetComponent<Controller>().ParkourID == 3)
            {
                if (!Stages.GotUp)
                {
                    parkournos += 1;
                    Stages.GotUp = true;
                }
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Parkour && Settings.GameManager.LocalPlayer.GetComponent<Controller>().ParkourID == 4)
            {
                if (!Stages.climbed)
                {
                    parkournos += 1;
                    Stages.climbed = true;
                }
            }
            Stages.ParkourPerformedUI.text = "Performed " + parkournos + "/5";
            if (parkournos == 5) {
                Settings.CurrentStage = 5;
            }
        }
        if (Settings.CurrentStage == 5) {
            NextStage = 6;
            Invoke("DelayedSwitch", Stages.DelayTimer);
        }
        if (Settings.CurrentStage == 6) {
            if (Settings.GameManager.LocalPlayer.GetComponent<WeaponControl>().Pistol != null) {
                if (!Stages.hasPistol) {
                    weaponnos += 1;
                    Stages.hasPistol = true;
                }
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<WeaponControl>().Heavy != null)
            {
                if (!Stages.hasHeavy)
                {
                    weaponnos += 1;
                    Stages.hasHeavy = true;
                }
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<WeaponControl>().SMG != null)
            {
                if (!Stages.hasSMG)
                {
                    weaponnos += 1;
                    Stages.hasSMG = true;
                }
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<WeaponControl>().Assault != null)
            {
                if (!Stages.hasAssault)
                {
                    weaponnos += 1;
                    Stages.hasAssault = true;
                }
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<WeaponControl>().Sniper != null)
            {
                if (!Stages.hasSniper)
                {
                    weaponnos += 1;
                    Stages.hasSniper = true;
                }
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<WeaponControl>().Launchers != null)
            {
                if (!Stages.hasRPG)
                {
                    weaponnos += 1;
                    Stages.hasRPG = true;
                }
            }
            Stages.AcquiredUI.text = "Acquired Weapons " + weaponnos + "/6";
            if (weaponnos == 6)
            {
                Settings.CurrentStage = 7;
            }
        }
        if (Settings.CurrentStage == 7) {
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Aim)
            {
                Stages.Aimed = true;
            }
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Attack)
            {
                Stages.Fired = true;
            }

            if (Stages.Fired && Stages.Aimed) {
                Settings.CurrentStage = 8;
            }
        }
        if (Settings.CurrentStage == 8) {
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().InCover)
            {
                Stages.TookCover = true;
                Settings.CurrentStage = 9;
            }
        }
        if (Settings.CurrentStage == 9)
        {
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().InCover)
            {
                if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Aim)
                {
                    Stages.AimedInCover = true;
                }
                if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().Attack)
                {
                    Stages.BlindFired = true;
                }
            }
            if (Stages.BlindFired && Stages.AimedInCover) {
                Settings.CurrentStage = 10;
            }
        }
        if (Settings.CurrentStage == 10) {
            if (Settings.GameManager.LocalPlayer.GetComponent<Controller>().InCar) {
                Stages.EnteredVehicle = true;
                Settings.CurrentStage = 11;
            }
        }
        if (Settings.CurrentStage == 11)
        {
            NextStage = 12;
            Invoke("DelayedSwitch", Stages.DelayTimer*2);
        }
        if (Settings.CurrentStage == 12)
        {
            NextStage = 13;
            Invoke("DelayedSwitch", Stages.DelayTimer);
        }
        if (Settings.CurrentStage == 13)
        {
            if (Input.GetKeyDown(KeyCode.Z)) {
                Stages.CheckedMoney = true;
                NextStage = 14;
                Invoke("DelayedSwitch", Stages.DelayTimer);
            }
        }
        if (Settings.CurrentStage == 14) {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Stages.OpenedPhone = true;
                Settings.CurrentStage = 15;
            }
        }
        if (Settings.CurrentStage == 15)
        {
            NextStage = 16;
            Invoke("DelayedSwitch", Stages.DelayTimer);
        }
        if (Settings.CurrentStage == 16)
        {
            NextStage = 17;
            Stages.EndTutorial = true;
            Invoke("DelayedSwitch", Stages.DelayTimer);
        }
    }

    void DelayedSwitch() {
        Settings.CurrentStage = NextStage;
        CancelInvoke();
    }

    void SetupTutorialBars() {
        for (int i = 0; i < Settings.AllMenus.Length; i++)
        {
            if (i == Settings.CurrentStage)
            {
                Settings.AllMenus[i].SetActive(true);
            }
            else
            {
                Settings.AllMenus[i].SetActive(false);
            }
        }

    }
}
