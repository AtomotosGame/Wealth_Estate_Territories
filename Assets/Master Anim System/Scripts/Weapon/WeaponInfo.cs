using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo : MonoBehaviour {
    [Range(0,6)]
    public int SlotID;
    public string GunName;
    public bool hasScope = false;
    public bool TempGun;


    [Tooltip("Muzzle Particle")]
    public ParticleSystem Muzzle;
    public ParticleSystem Trail;
    [Tooltip("Ammo Shell Eject Effect")]
    public ParticleSystem EjectBullet;
    public BoxCollider GunCollider;
    public GameObject GunTrigger;
    public Sprite Icon;


    [Header("IK Stuff")]
    public Transform HandTarget;
    public LayerMask DetectLayer;

    [Header("General Properties")]
    public GameObject GunUser;
    public float Damage;
    public Transform bulletspawn;
    public Transform BulletSpawner;
    //public GameObject Icon;
    public int AmmoLeft = 30;
    public int Totalammo = 30;
    public int megazine;
    public AudioClip gunShot;
    public AudioClip EmptyShot;
    bool Setreload;
    public float nextFire = 0.0f;
    public float fireRate = 0.7f;
    public float point = 30;
    [Tooltip("If the weapon has any")]
    public GameObject MagazineOBJ;
    // Use this for initialization

    [Header("Recoil Properties")]
    public float recoilAmount;
    public float recoilRecoverTime;
    [HideInInspector] public float currentRecoilZPos;
    [HideInInspector] public float currentRecoilZPosV;

    [Header("Bullet Spread Properties")]
    public float MinSpray;
    public float MaxSpray;
    [HideInInspector] public float XRotation;
    [HideInInspector] public float YRotation;
    Vector3 crosshairPoint;

    // Hitting Ray
    [HideInInspector] public RaycastHit hit;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        Setted();
        if (GunUser != null)
        {
            bulletspawn.localRotation = Quaternion.Euler(XRotation, YRotation, 0);
            if (AmmoLeft > 0)
            {
                Setreload = true;
            }

            //ReloadOption
            if (megazine > 0)
            {
                if (GunUser.GetComponent<Controller>())
                {
                    if (AmmoLeft <= 0 || Input.GetButtonDown("Reload"))
                    {
                        if (AmmoLeft != Totalammo)
                        {
                            if (GunUser.GetComponent<Animator>().GetFloat("BZWep") < 0.05f)
                            {
                                if (Setreload)
                                {
                                    GunUser.GetComponent<Animator>().SetTrigger("Reload");
                                    Setreload = false;
                                    Invoke("Reload", 1.5f);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (AmmoLeft <= 0)
                    {
                        if (AmmoLeft != Totalammo)
                        {
                            if (GunUser.GetComponent<Animator>().GetFloat("BZWep") < 0.05f)
                            {
                                if (Setreload)
                                {
                                    GunUser.GetComponent<Animator>().SetTrigger("Reload");
                                    Setreload = false;
                                    Invoke("Reload", 1.5f);
                                }
                            }
                        }
                    }
                }
            }
        }
        else {
            if (TempGun)
            {
                if ((transform.position - Camera.main.transform.position).sqrMagnitude > 50 * 50)
                {
                    DestroyGun();
                }
            }
        }
    }

    public void ReloadFunction() {
        GunUser.GetComponent<Animator>().SetTrigger("Reload");
        Setreload = false;
        Invoke("Reload", 1.5f);
    }

    void Setted() {
        if (GunUser != null)
        {
            GunCollider.enabled = false;
            GunTrigger.SetActive(false);
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else {
            GunCollider.enabled = true;
            GunTrigger.SetActive(true);
            GetComponent<Rigidbody>().isKinematic = false;
        }

        if (MagazineOBJ != null) {
            if (AmmoLeft == 0)
            {
                MagazineOBJ.SetActive(false);
            }
            else {
                MagazineOBJ.SetActive(true);
            }
        }
    }

    void FixedUpdate()
    {
        if (GunUser != null)
        {
            if (GunUser.GetComponent<Controller>())
            {
                Ray ray = new Ray(GunUser.GetComponent<Controller>().Mcamera.transform.position + (GunUser.GetComponent<Controller>().Mcamera.transform.forward * 3f), GunUser.GetComponent<Controller>().Mcamera.transform.forward);

                if (Physics.Raycast(ray, out hit))
                {
                    if (!hit.collider.gameObject.GetComponent<Hitmark>())
                    {
                        crosshairPoint = hit.point;
                    }
                    else
                    {
                        if (hit.collider.gameObject.GetComponent<Hitmark>().HealthScript.gameObject != GunUser)
                        {
                            crosshairPoint = hit.point;

                        }
                    }

                }
                else
                {
                    crosshairPoint = ray.GetPoint(100);
                }
                BulletSpawner.LookAt(crosshairPoint);
            }
            if (GunUser.GetComponent<MASAIBrain>()) {
                if (GunUser.GetComponent<MASAIBrain>().Target) {
                    Vector3 ReTarget = new Vector3(GunUser.GetComponent<MASAIBrain>().Target.position.x, GunUser.GetComponent<MASAIBrain>().Target.position.y + 1, GunUser.GetComponent<MASAIBrain>().Target.position.z);
                    //BulletSpawner.locarotation = Quaternion.LookRotation(transform.position - ReTarget);
                     BulletSpawner.LookAt(ReTarget);
                }
            }
        }
    }

    public void BulletSpray()
    {
        if (GunUser.GetComponent<MASAIBrain>())
        {
            XRotation += (Random.value - 0.5f) * Mathf.Lerp((MinSpray * 7), (MaxSpray * 7), 1f);
            YRotation += (Random.value - 0.5f) * Mathf.Lerp((MinSpray * 7), (MaxSpray * 7), 1f);
        }
        else
        {
            XRotation += (Random.value - 0.5f) * Mathf.Lerp(MinSpray, MaxSpray, 1f);
            YRotation += (Random.value - 0.5f) * Mathf.Lerp(MinSpray, MaxSpray, 1f);
        }
    }

    void Reload()
    {
        AmmoLeft = Totalammo;
        megazine -= 1;
    }
    
    public void Restock() {
        megazine = 7;
    }

    public void DestroyGun()
    {
        GameObject.Destroy(gameObject);
    }
}
