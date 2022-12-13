using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 using System.Linq;
using UnityEngine.EventSystems;


public class GlobalObjects : MonoBehaviour {
    [Header("Player Details")]
    public GameObject LocalPlayer;
    public int LocalPlayerCash;
    public int LocalPlayerBank;
    [HideInInspector] public int Money;

    [Header("Settings")]
    public bool MouseVisibilityControl;
    public bool InHardMenu;
    public bool AllowCrosshair;
    public bool ShowGhostCrosshair = true;
    public bool LeaveMeAlone;
    public Transform RespawnLocation;

    [Header("Camera")]
    public GameObject cam;
    public Camera Mcamera; 

    [Header("LAW Enforcement Settings")]
    [Range(0, 5)]
    public int WantedLevel;
    public AudioClip WantedMusic;
    public int notorietyLevel;
    public GameObject[] COPS;

    [Header("UI Color Settings")]
    public Color CashTextColor;
    public Color BankTextColor;
    public Color CostTextColor;
    public Color EarnTextColor;

    [Header("UI Elements")]
    public Text AmmoText;
    public Slider HealthUI;
    public GameObject InternetUI;
    public GameObject RadarUI;
    public GameObject WantedLevelUI;
    public GameObject[] Wlevels;
    public GameObject GunStoreUI;
    public GameObject GunStoreHomePage;
    public GameObject[] GunStoreSidePages;
    public GameObject PhoneUI;
    public GameObject HomePage;
    public GameObject[] SidePages;
    public GameObject IntMenuUI;
    public GameObject IntMenuHome;
    public GameObject[] IntMenuPages;
    public GameObject DeathScreenUI;
    public AudioSource UIAudio;

    [System.Serializable]
    public class TrackInfo
    {
        public string SongTitle;
        public string SongAuthor;
        public string SongAlbum;
        public AudioClip Track;
        public Sprite AlbumImage;
    }

    [System.Serializable]
    public class WeaponWheelUI
    {
        public bool WeaponWheel;
        public Text WeaponTypeText;
        public Sprite NotSelected;
        public Sprite Selected;
        public GameObject Wheel;
        public AudioClip SwitchSFX;
        public AudioClip SwitchEndSFX;
        public Image Slot0;
        public Image Slot1;
        public Image Slot2;
        public Image Slot3;
        public Image Slot4;
        public Image Slot5;
        public Image Slot6;
        public Image Gun1;
        public Image Gun2;
        public Image Gun3;
        public Image Gun4;
        public Image Gun5;
        public Image Gun6;
    }



    public WeaponWheelUI WeaponUI;
    public GameObject MoneyStats;


    public GameObject Crosshair;
    public Image CrosshairImage;
    public Image CrosshairGhostImage;

    public EventSystem MobileInputEvent;
    public GameObject HomeButton;
    [Range(0, 2)]
    public int CurrentMp3Slot;
    public GameObject MP3Menu;
    public bool PlayingMP3;
    public AudioSource MP3PLAYA;
    public Text SongTitle;
    public Text SongAuthor;
    public Text SongAlbum;
    public Image TrackCover;
    public Slider MusicLengthSlider;
    public Animator FadeAnim;
    public GameObject ApartmentSaleUI;
    public Text ApartmentName;
    public Text ApartmentDescription;
    public Text ApartmentCost;
    public Text CashUI;
    public Text BankUI;
    public Text CostUI;
    public Text CurrentActionText;
    public Slider ActionIDSlider;
    string ActionName;
    int ActionID;
    public AudioClip PurchaseSound;
    public AudioClip AddMoneySound;
    public AudioSource WindCarSFX;
    [Header("Songs Playlist")]
    public TrackInfo[] Songs;


    [Header("AIs Info")]
    public int AILimit = 20; 
    public GameObject[] AIsInScene;

    //Temporary Variables
    bool disabledphon = false;
    bool recall = false;
    // Use this for initialization
    void Start() {
        if (LocalPlayer)
        {
            recall = false;
            LocalPlayer.GetComponent<Health>().DeathScreenUI = DeathScreenUI;
            LocalPlayer.GetComponent<Health>().GameManager = this.GetComponent<GlobalObjects>();
            if (RespawnLocation)
            {
                LocalPlayer.GetComponent<Health>().RespawnPosition = RespawnLocation;
            }
        }
        else {
            recall = true;
        }
        CashUI.color = CashTextColor;
        BankUI.color = BankTextColor;

    }

    // Update is called once per frame
    void Update()
    {
        MP3PlayerManager();
        MoneyUI();
        WantedLevelUISetup();
        WantedLevelSetup();
        WantedLevelMusic();
        AIProgression();
        CheckPlayerCam();

        if (LocalPlayer)
        {
            if (MouseVisibilityControl)
            {
                Screen.lockCursor = true;
                Cursor.visible = false;
            }
            InteractionMenuFunc();
            RecallFunc();
            CarWindSFX();
            PlayerHealth();
            PlayerAmmo();

            if (InHardMenu)
            {
                LocalPlayer.GetComponent<Controller>().GetInput = false;
                LocalPlayer.GetComponent<Controller>().Speed = 0;
                LocalPlayer.GetComponent<Controller>().Dir = 0;
                RadarUI.SetActive(false);
                if (!disabledphon)
                {
                    LocalPlayer.GetComponent<Action_Phone>().isPhoneOn = false;
                    LocalPlayer.GetComponent<Action_Phone>().AllowPhone = false;
                    disabledphon = true;
                }
            }
            else
            {
                LocalPlayer.GetComponent<Controller>().GetInput = true;
                RadarUI.SetActive(true);
                if (disabledphon)
                {
                    LocalPlayer.GetComponent<Action_Phone>().AllowPhone = true;
                    disabledphon = false;
                }
            }

            if (GunStoreUI.activeSelf == true)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (GunStoreHomePage.activeSelf)
                    {
                        GunStoreUI.SetActive(false);
                        InHardMenu = false;
                    }
                    else
                    {
                        GoBackGunStore();
                    }
                }
            }

            if (InternetUI.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (GunStoreHomePage.activeSelf)
                    {
                        InternetUI.SetActive(false);
                        InHardMenu = false;
                    }
                    else
                    {
                        // GoBackGunStore();
                    }
                }
            }

            if (WeaponUI.WeaponWheel)
            {
                CheckGunSlot();
                CheckGunIcon();
                if (LocalPlayer.GetComponent<Controller>().GetInput)
                {
                    // If player has Input
                    if (Input.GetButtonDown("NextWeapon") || Input.GetAxis("Mouse ScrollWheel") != 0)
                    {
                        if (!LocalPlayer.GetComponent<Controller>().Aim)
                        {
                            WeaponUI.Wheel.SetActive(true);
                            UIAudio.PlayOneShot(WeaponUI.SwitchSFX);
                        }
                    }
                    if (Input.GetButtonDown("Stats"))
                    {
                        MoneyStats.SetActive(true);
                    }
                }

                if (WeaponUI.Wheel.activeSelf)
                {
                    Invoke("RevokedWheel", 1f);
                }
                else
                {
                    CancelInvoke("RevokedWheel");
                }

                if (MoneyStats.activeSelf)
                {
                    Invoke("RevokedMoneyStats", 2f);
                }
                else
                {
                    CancelInvoke("RevokedMoneyStats");
                }

                if (ApartmentSaleUI.activeSelf)
                {
                    Invoke("RevokedAPSaleUI", 10f);
                }
                else
                {
                    CancelInvoke("RevokedAPSaleUI");
                }
            }

            if (AllowCrosshair)
            {
                CrosshairDetect();
            }
            else
            {
                Crosshair.SetActive(false);
            }
        }
    }

    void PlayerHealth() {
        HealthUI.value = LocalPlayer.GetComponent<Health>().health;
    }

    void PlayerAmmo() {
        if (!LocalPlayer.GetComponent<WeaponControl>().CurrentGun)
        {
            AmmoText.text = "Unarmed";
        }
        else {
            AmmoText.text = LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().GunName + " " + LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().AmmoLeft + "/" + (LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().Totalammo * LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().megazine);
        }

    }

    void RecallFunc()
    {
        if (recall)
        {
            LocalPlayer.GetComponent<Health>().DeathScreenUI = DeathScreenUI;
            LocalPlayer.GetComponent<Health>().GameManager = this.GetComponent<GlobalObjects>();
            if (RespawnLocation)
            {
                LocalPlayer.GetComponent<Health>().RespawnPosition = RespawnLocation;
            }
            recall = false;
        }
    }


    void CarWindSFX()
    {
        if (LocalPlayer.GetComponent<Action_Car>().MySit)
        {
            if (LocalPlayer.GetComponent<Action_Car>().MySit.CUC.TypeID != 1)
            {
                WindCarSFX.volume = (LocalPlayer.GetComponent<Action_Car>().MySit.CUC.speed / 100);
                WindCarSFX.volume = Mathf.Clamp(WindCarSFX.volume, 0, 1);

                if (!WindCarSFX.isPlaying)
                {
                    WindCarSFX.Play();
                }
            }
            else {
                if (WindCarSFX.isPlaying)
                {
                    WindCarSFX.Stop();
                }
            }
        }
        else
        {
            if (LocalPlayer.GetComponent<Controller>().Falling && !LocalPlayer.GetComponent<Controller>().InCar && !LocalPlayer.GetComponent<Controller>().Ladder && !LocalPlayer.GetComponent<Controller>().Swim && LocalPlayer.GetComponent<Controller>().AllowSkydive)
            {
                WindCarSFX.volume = (LocalPlayer.GetComponent<Controller>().VerticalVelocity / -100);
                WindCarSFX.volume = Mathf.Clamp(WindCarSFX.volume, 0, 1);

                if (!WindCarSFX.isPlaying)
                {
                    WindCarSFX.Play();
                }
            }
            else
            {
                if (WindCarSFX.isPlaying)
                {
                    WindCarSFX.Stop();
                }
            }
        }
    }

    void CheckPlayerCam() {
        if (LocalPlayer) {
            if (cam.GetComponent<CameraRig>().target != LocalPlayer.transform) {
                cam.GetComponent<CameraRig>().target = LocalPlayer.transform;
                LocalPlayer.GetComponent<Controller>().Mcamera = Mcamera;
                LocalPlayer.GetComponent<Controller>().cam = cam;
            }
        }
    }

    public void InternetTurnOn() {
        InternetUI.SetActive(true);
        InHardMenu = true;
    }

    void CrosshairDetect()
    {
        Crosshair.SetActive(LocalPlayer.GetComponent<Controller>().Aim);

        if (Crosshair.activeSelf)
        {
            if (LocalPlayer.GetComponent<Controller>().InCar == false)
            {
                if (LocalPlayer.GetComponent<WeaponControl>().CurrentGun)
                {
                    Crosshair.GetComponent<Animator>().SetBool("HasScope", LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().hasScope);
                }
                else
                {
                    Crosshair.GetComponent<Animator>().SetBool("HasScope", false);
                }
            }
            else
            {
                Crosshair.GetComponent<Animator>().SetBool("HasScope", false);
            }
        }
        else
        {
            Crosshair.GetComponent<Animator>().SetBool("HasScope", false);
        }


        if (LocalPlayer.GetComponent<Controller>().Aim)
        {
            if (LocalPlayer.GetComponent<WeaponControl>().CurrentGun)
            {
                if (LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().hit.collider)
                {
                    if (ShowGhostCrosshair)
                    {
                        Ray SpawnerRay = new Ray(LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().BulletSpawner.position, LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().BulletSpawner.forward);
                        RaycastHit Bulhit;


                        if (Physics.Raycast(SpawnerRay, out Bulhit))
                        {
                            if (Bulhit.collider != LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().hit.collider)
                            {
                                CrosshairGhostImage.enabled = true;
                                CrosshairGhostImage.transform.position = Camera.main.WorldToScreenPoint(Bulhit.point);
                            }
                            else
                            {
                                CrosshairGhostImage.enabled = false;
                            }
                        }
                        else
                        {
                            CrosshairGhostImage.enabled = false;
                        }
                    }

                    if (LocalPlayer.GetComponent<WeaponControl>().CurrentGun.GetComponent<WeaponInfo>().hit.collider.gameObject.GetComponent<Hitmark>())
                    {
                        CrosshairImage.color = Color.red;
                    }
                    else
                    {
                        CrosshairImage.color = Color.white;
                    }
                }
                else
                {
                    CrosshairImage.color = Color.white;
                    CrosshairGhostImage.enabled = false;
                }
            }
        }
    }

    void WantedLevelMusic()
    {
        if (WantedMusic)
        {
            if (WantedLevel != 0)
            {
                if (!PlayingMP3)
                {
                    if (!MP3PLAYA.isPlaying)
                    {
                        MP3PLAYA.clip = WantedMusic;
                        MP3PLAYA.Play();
                    }
                }
            }
            else
            {
                if (!PlayingMP3)
                {
                    if (MP3PLAYA.isPlaying)
                    {
                        MP3PLAYA.Stop();
                    }
                }
            }
        }
    }

    void WantedLevelUISetup() {
        if (WantedLevel == 0)
        {
            WantedLevelUI.SetActive(false);
        }
        else {
            WantedLevelUI.SetActive(true);
            for (int i = 0; i < Wlevels.Length; i++)
            {
                if (i == WantedLevel)
                {
                    Wlevels[i].SetActive(true);
                }
                else
                {
                    Wlevels[i].SetActive(false);
                }
            }
        }
    }

    void WantedLevelSetup()
    {
        if (!LeaveMeAlone)
        {
            if (notorietyLevel != 0)
            {
                if (notorietyLevel < 2)
                {
                    WantedLevel = 1;
                }
                else
                {
                    if (notorietyLevel < 5)
                    {
                        WantedLevel = 2;
                    }
                    else
                    {
                        if (notorietyLevel < 8)
                        {
                            WantedLevel = 3;
                        }
                        else
                        {
                            if (notorietyLevel < 10)
                            {
                                WantedLevel = 4;
                            }
                            else
                            {
                                if (notorietyLevel < 15)
                                {
                                    WantedLevel = 5;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                WantedLevel = 0;
            }
        }
        else {
            WantedLevel = 0;
        }
    }

    void RevokedWheel() {
        WeaponUI.Wheel.SetActive(false);
        UIAudio.PlayOneShot(WeaponUI.SwitchEndSFX);
    }

    void RevokedMoneyStats()
    {
        MoneyStats.SetActive(false);
        CostUI.gameObject.SetActive(false);
    }

    void RevokedAPSaleUI() {
        ApartmentSaleUI.SetActive(false);
    }

    void MoneyUI() {
        Money = LocalPlayerBank + LocalPlayerCash;
        CashUI.text = "$" + LocalPlayerCash.ToString();
        BankUI.text = "$" + LocalPlayerBank.ToString();
    }

    public void Purchase(int Cost) {
        UIAudio.PlayOneShot(PurchaseSound);
        AudioSource.PlayClipAtPoint(PurchaseSound, transform.position, 1);
        if (LocalPlayerCash > Cost)
        {
            LocalPlayerCash -= Cost;
        }
        else {
            LocalPlayerBank -= (Cost - LocalPlayerCash);
            LocalPlayerCash = 0;
        }
        CostUI.text = "-$" + Cost.ToString();
        CostUI.color = CostTextColor;
        MoneyStats.SetActive(true);
        CostUI.gameObject.SetActive(true);
    }

    public void AddMoney(int Amount) {
        UIAudio.PlayOneShot(PurchaseSound);
        AudioSource.PlayClipAtPoint(PurchaseSound, transform.position, 1);
        LocalPlayerCash += Amount;
        CostUI.text = "+$" + Amount.ToString();
        CostUI.color = EarnTextColor;
        MoneyStats.SetActive(true);
        CostUI.gameObject.SetActive(true);
    }

    void MP3PlayerManager()
    {
        if (MP3Menu.activeSelf)
        {
            for (int i = 0; i < Songs.Length; i++)
            {
                if (i == CurrentMp3Slot)
                {
                    SongTitle.text = Songs[i].SongTitle;
                    SongAuthor.text = Songs[i].SongAuthor;
                    SongAlbum.text = Songs[i].SongAlbum;
                    MP3PLAYA.clip = Songs[i].Track;
                    TrackCover.sprite = Songs[i].AlbumImage;
                }
            }

            MusicLengthSlider.maxValue = MP3PLAYA.clip.length;
            MusicLengthSlider.value = MP3PLAYA.time;
        }
    }

    public void NextMusic() {
        if (CurrentMp3Slot >= 0) {
            CurrentMp3Slot += 1;
        }
    }

    public void BackMusic()
    {
        if (CurrentMp3Slot > 0)
        {
            CurrentMp3Slot -= 1;
        }
    }

    public void PlayMusic() {
        if (MP3PLAYA.isPlaying)
        {
            PlayingMP3 = false;
            MP3PLAYA.Stop();
        }
        else
        {
            PlayingMP3 = true;
            MP3PLAYA.Play();
        }
    }

    void CheckGunIcon() {
        if (LocalPlayer.GetComponent<WeaponControl>().Pistol != null)
        {
            WeaponUI.Gun1.enabled = true;
            WeaponUI.Gun1.sprite = LocalPlayer.GetComponent<WeaponControl>().Pistol.GetComponent<WeaponInfo>().Icon;
        }
        else {
            WeaponUI.Gun1.enabled = false;
            WeaponUI.Gun1.sprite = null;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().Heavy != null)
        {
            WeaponUI.Gun2.enabled = true;
            WeaponUI.Gun2.sprite = LocalPlayer.GetComponent<WeaponControl>().Heavy.GetComponent<WeaponInfo>().Icon;
        }
        else
        {
            WeaponUI.Gun2.enabled = false;
            WeaponUI.Gun2.sprite = null;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().SMG != null)
        {
            WeaponUI.Gun3.enabled = true;
            WeaponUI.Gun3.sprite = LocalPlayer.GetComponent<WeaponControl>().SMG.GetComponent<WeaponInfo>().Icon;
        }
        else
        {
            WeaponUI.Gun3.enabled = false;
            WeaponUI.Gun3.sprite = null;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().Assault != null)
        {
            WeaponUI.Gun4.enabled = true;
            WeaponUI.Gun4.sprite = LocalPlayer.GetComponent<WeaponControl>().Assault.GetComponent<WeaponInfo>().Icon;
        }
        else
        {
            WeaponUI.Gun4.enabled = false;
            WeaponUI.Gun4.sprite = null;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().Sniper != null)
        {
            WeaponUI.Gun5.enabled = true;
            WeaponUI.Gun5.sprite = LocalPlayer.GetComponent<WeaponControl>().Sniper.GetComponent<WeaponInfo>().Icon;
        }
        else
        {
            WeaponUI.Gun5.enabled = false;
            WeaponUI.Gun5.sprite = null;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().Launchers != null)
        {
            WeaponUI.Gun6.enabled = true;
            WeaponUI.Gun6.sprite = LocalPlayer.GetComponent<WeaponControl>().Launchers.GetComponent<WeaponInfo>().Icon;
        }
        else
        {
            WeaponUI.Gun6.enabled = false;
            WeaponUI.Gun6.sprite = null;
        }
    }

    public void TurnOnCamera() {
        LocalPlayer.GetComponent<Action_Phone>().TakingPicture = true;
    }

    public void GoBack() {
        for (int i = 0; i < SidePages.Length; i++)
        {
            SidePages[i].SetActive(false);
        }
        HomePage.SetActive(true);
        MobileInputEvent.SetSelectedGameObject(HomeButton);
    }

    public void GoBackGunStore() {
        for (int i = 0; i < GunStoreSidePages.Length; i++)
        {
            GunStoreSidePages[i].SetActive(false);
        }
        GunStoreHomePage.SetActive(true);
    }

    public void GunStoreButton(GameObject SwitchTo) {
        GunStoreHomePage.SetActive(false);
        SwitchTo.SetActive(true);
    }

    void InteractionMenuFunc() {
        LocalPlayer.GetComponent<Animator>().SetFloat("ActionID", ActionID);
        if (IntMenuUI.activeSelf)
        {

            ActionID = (int)ActionIDSlider.value;
            LocalPlayer.GetComponent<Controller>().ActionID = ActionID;
            LocalPlayer.GetComponent<Action_Phone>().AllowPhone = false;
            if (ActionID == 0)
            {
                CurrentActionText.text = " < Cheer > ";
            }
            if (ActionID == 1)
            {
                CurrentActionText.text = " < Dab > ";
            }
            if (ActionID == 2)
            {
                CurrentActionText.text = " < Mid Fing > ";
            }

        }
        else {
            LocalPlayer.GetComponent<Action_Phone>().AllowPhone = true;
        }

        if (!InHardMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GunStoreHomePage.activeSelf)
                {
                    IntMenuUI.SetActive(false);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    IntMenuUI.SetActive(true);
                }
            }
        }
        else {
            IntMenuUI.SetActive(false);
        }
    }

    public void CheckGunSlot(){
        if (LocalPlayer.GetComponent<WeaponControl>().CurrentSlot == 0)
        {
            WeaponUI.Slot0.sprite = WeaponUI.Selected;
            WeaponUI.WeaponTypeText.text = "Unarmed";
        }
        else {
            WeaponUI.Slot0.sprite = WeaponUI.NotSelected;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().CurrentSlot == 1)
        {
            WeaponUI.Slot1.sprite = WeaponUI.Selected;
            WeaponUI.WeaponTypeText.text = "Pistol";
        }
        else
        {
            WeaponUI.Slot1.sprite = WeaponUI.NotSelected;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().CurrentSlot == 2)
        {
            WeaponUI.Slot2.sprite = WeaponUI.Selected;
            WeaponUI.WeaponTypeText.text = "Shotgun";
        }
        else
        {
            WeaponUI.Slot2.sprite = WeaponUI.NotSelected;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().CurrentSlot == 3)
        {
            WeaponUI.Slot3.sprite = WeaponUI.Selected;
            WeaponUI.WeaponTypeText.text = "SMG";
        }
        else
        {
            WeaponUI.Slot3.sprite = WeaponUI.NotSelected;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().CurrentSlot == 4)
        {
            WeaponUI.Slot4.sprite = WeaponUI.Selected;
            WeaponUI.WeaponTypeText.text = "Assault Rifle";
        }
        else
        {
            WeaponUI.Slot4.sprite = WeaponUI.NotSelected;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().CurrentSlot == 5)
        {
            WeaponUI.Slot5.sprite = WeaponUI.Selected;
            WeaponUI.WeaponTypeText.text = "Sniper Rifle";
        }
        else
        {
            WeaponUI.Slot5.sprite = WeaponUI.NotSelected;
        }
        if (LocalPlayer.GetComponent<WeaponControl>().CurrentSlot == 6)
        {
            WeaponUI.Slot6.sprite = WeaponUI.Selected;
            WeaponUI.WeaponTypeText.text = "Launcher";
        }
        else
        {
            WeaponUI.Slot6.sprite = WeaponUI.NotSelected;
        }
    }

    void AIProgression()
    {
        AIsInScene = GameObject.FindGameObjectsWithTag("AI_Agent");

        if (AIsInScene.Length != 0)
        {
            Dictionary<float, GameObject> distDic = new Dictionary<float, GameObject>();

            foreach (GameObject obj in AIsInScene)
            {
                float dist = Vector3.Distance(gameObject.transform.position, obj.transform.position);

                distDic.Add(dist, obj);
            }

            List<float> distances = distDic.Keys.ToList();

            distances.Sort();

            GameObject furthestObj = distDic[distances[distances.Count - 1]];

            if (AIsInScene.Length > AILimit)
            {
                if (furthestObj.GetComponent<MAS_AIManager>().TemporaryAI)
                {
                    furthestObj.GetComponent<MAS_AIManager>().DestroyAI();
                }
            }
        }


    }
}
