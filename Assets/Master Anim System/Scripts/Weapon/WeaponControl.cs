using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    Animator anim;
    AnimatorStateInfo stateInfo;
    IKControl IK_CTRL;
    Controller CTRL_s;
    Action_Phone Phone_SCRPT;
    [HideInInspector] public GlobalObjects Global_Script;
    Health health;
    [Range(0, 6)]
    public int CurrentSlot;
    public GameObject CurrentGun;

    [Header("Guns In Slot")]
    public GameObject Pistol;
    public GameObject Heavy;
    public GameObject SMG;
    public GameObject Assault;
    public GameObject Sniper;
    public GameObject Launchers;

    [Header("Guns In Slot")]
    public Transform HandPos;
    public Transform RifleAimPos;
    public Transform PistolAimPos;
    public bool BlockedAim;

    [Header("GeneralSetting")]
    public Transform BlockedPos;
    public Transform LookObject;

    [Header("FiringSetting")]
    public LayerMask CastLayer; // casting layers / detecting layers
    public float AudioVolume = 1;
    float damage;
    public GameObject hitEffect;
    public GameObject ExplosionEffect;
    public float hitEffectDestroyTime = 1;
    public string hitEffectTag = "HitBox";
    public GameObject missEffect;
    public GameObject BrickEffect;
    public GameObject MetalEffect;
    public GameObject woodEffect;
    public GameObject waterEffect;
    public GameObject glassEffect;
    public GameObject dirtEffect;
    public float missEffectDestroyTime = 1;
    public float timeToDestroyAfterHitting = 0.5f;


    [Header("Player Information")]
    public int KilledTotal;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        IK_CTRL = GetComponent<IKControl>();
        CTRL_s = GetComponent<Controller>();
        Phone_SCRPT = GetComponent<Action_Phone>();
        health = GetComponent<Health>();
        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponDetector();
        if (!CTRL_s.InCover)
        {
            WeaponIKSetup();
        }
        else
        {
            if (Phone_SCRPT.isPhoneOn)
            {
                IK_CTRL.ikActive = true;
                IK_CTRL.lookObj = Phone_SCRPT.PhoneIKTarget;
                anim.SetFloat("SprintStyle", 0, 0.2f, Time.deltaTime);
                if (!CTRL_s.InCar)
                {
                    IK_CTRL.leftHandObj = null;
                    IK_CTRL.rightHandObj = Phone_SCRPT.PhoneIKTarget;
                }
                else
                {
                    if (GetComponent<Action_Car>().MySit.CUC.IKTargetLeft != null)
                    {
                        IK_CTRL.leftHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetLeft;
                    }
                    else
                    {
                        IK_CTRL.leftHandObj = null;
                    }
                    if (GetComponent<Action_Car>().MySit.CUC.IKTargetRight != null)
                    {
                        IK_CTRL.rightHandObj = Phone_SCRPT.PhoneIKTarget;
                    }
                    else
                    {
                        IK_CTRL.rightHandObj = Phone_SCRPT.PhoneIKTarget;
                    }
                }
            }
            else
            {
                IK_CTRL.lookObj = null;
                anim.SetFloat("SprintStyle", 0, 0.2f, Time.deltaTime);
                if (!CTRL_s.InCar)
                {
                    IK_CTRL.ikActive = false;
                    IK_CTRL.leftHandObj = null;
                    IK_CTRL.rightHandObj = null;
                }
                else
                {
                    if (GetComponent<Action_Car>().MySit.CUC.IKTargetLeft != null)
                    {
                        IK_CTRL.ikActive = true;
                        IK_CTRL.leftHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetLeft;
                    }
                    else
                    {
                        IK_CTRL.ikActive = false;
                        IK_CTRL.leftHandObj = null;
                    }
                    if (GetComponent<Action_Car>().MySit.CUC.IKTargetRight != null)
                    {
                        IK_CTRL.ikActive = true;
                        IK_CTRL.rightHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetRight;
                    }
                    else
                    {
                        IK_CTRL.ikActive = false;
                        IK_CTRL.rightHandObj = null;
                    }
                }
            }
            if (CurrentGun != null)
            {
                if (CTRL_s.Aim)
                {
                    FireDetect(1);
                    IK_CTRL.lookObj = LookObject;
                    if (CurrentSlot != 1 && CurrentSlot != 3)
                    {
                        IK_CTRL.rightHandObj = RifleAimPos;
                    }
                    else
                    {
                        IK_CTRL.rightHandObj = PistolAimPos;
                    }
                    stateInfo = anim.GetCurrentAnimatorStateInfo(1);
                    if (!stateInfo.IsName("Reload"))
                    {
                        IK_CTRL.ikActive = true;
                    }
                    else
                    {
                        IK_CTRL.ikActive = false;
                    }
                    if (!CTRL_s.InCar)
                    {
                        IK_CTRL.leftHandObj = CurrentGun.GetComponent<WeaponInfo>().HandTarget;
                    }
                    else
                    {
                        if (GetComponent<Action_Car>().MySit.CUC.IKTargetLeft != null)
                        {
                            IK_CTRL.leftHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetLeft;
                        }
                        else
                        {
                            IK_CTRL.leftHandObj = null;
                        }
                    }
                }
                else
                {
                    FireDetect(0);
                    CurrentGun.transform.parent = HandPos.parent;
                    CurrentGun.transform.localPosition = HandPos.localPosition;
                    CurrentGun.transform.localRotation = HandPos.localRotation;
                    CurrentGun.GetComponent<WeaponInfo>().GunUser = this.gameObject;
                }
            }
        }
        GunRecoil();
    }

    public void SwitchWeapon(int changeValue)
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("AimLocomot"))
        {
            if (changeValue < 0)
            {
                if (CurrentSlot == 0)
                {
                    changeValue = 6;
                }
            }
            CurrentSlot = CurrentSlot + changeValue;
        }
    }

    void WeaponIKSetup()
    {
        if (health.alive)
        {
            if (CurrentGun != null)
            {
                FireDetect(1);
                if (CTRL_s.Aim)
                {
                    IK_CTRL.lookObj = LookObject;
                    if (CurrentSlot != 1 && CurrentSlot != 3)
                    {
                        IK_CTRL.rightHandObj = RifleAimPos;
                    }
                    else
                    {
                        IK_CTRL.rightHandObj = PistolAimPos;
                    }
                    stateInfo = anim.GetCurrentAnimatorStateInfo(1);
                    if (!stateInfo.IsName("Reload"))
                    {
                        IK_CTRL.ikActive = true;
                    }
                    else
                    {
                        IK_CTRL.ikActive = false;
                    }
                    if (!CTRL_s.InCar)
                    {
                        IK_CTRL.leftHandObj = CurrentGun.GetComponent<WeaponInfo>().HandTarget;
                    }
                    else
                    {
                        if (GetComponent<Action_Car>().MySit.CUC.IKTargetLeft != null)
                        {
                            IK_CTRL.leftHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetLeft;
                        }
                        else
                        {
                            IK_CTRL.leftHandObj = null;
                        }
                    }
                }
                else
                {
                    IK_CTRL.lookObj = null;
                    if (CurrentSlot != 1 && CurrentSlot != 3)
                    {
                        stateInfo = anim.GetCurrentAnimatorStateInfo(1);
                        if (!stateInfo.IsName("Reload"))
                        {
                            IK_CTRL.ikActive = true;
                        }
                        else
                        {
                            IK_CTRL.ikActive = false;
                        }
                        if (!CTRL_s.InCar)
                        {
                            IK_CTRL.rightHandObj = null;
                            IK_CTRL.leftHandObj = CurrentGun.GetComponent<WeaponInfo>().HandTarget;
                        }
                        else
                        {
                            if (GetComponent<Action_Car>().MySit.CUC.IKTargetLeft != null)
                            {
                                IK_CTRL.leftHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetLeft;
                            }
                            else
                            {
                                IK_CTRL.leftHandObj = null;
                            }
                            if (GetComponent<Action_Car>().MySit.CUC.IKTargetRight != null)
                            {
                                IK_CTRL.rightHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetRight;
                            }
                            else
                            {
                                IK_CTRL.rightHandObj = null;
                            }
                        }
                        anim.SetFloat("SprintStyle", 1, 0.2f, Time.deltaTime);

                    }
                    else
                    {
                        anim.SetFloat("SprintStyle", 0, 0.2f, Time.deltaTime);
                        if (!CTRL_s.InCar)
                        {
                            IK_CTRL.ikActive = false;
                            IK_CTRL.leftHandObj = null;
                            IK_CTRL.rightHandObj = null;
                        }
                        else
                        {
                            if (GetComponent<Action_Car>().MySit.CUC.IKTargetLeft != null)
                            {
                                IK_CTRL.ikActive = true;
                                IK_CTRL.leftHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetLeft;
                            }
                            else
                            {
                                IK_CTRL.ikActive = false;
                                IK_CTRL.leftHandObj = null;
                            }
                            if (GetComponent<Action_Car>().MySit.CUC.IKTargetRight != null)
                            {
                                IK_CTRL.ikActive = true;
                                IK_CTRL.rightHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetRight;
                            }
                            else
                            {
                                IK_CTRL.ikActive = false;
                                IK_CTRL.rightHandObj = null;
                            }
                        }
                    }
                }

                CurrentGun.transform.parent = HandPos.parent;
                CurrentGun.transform.localPosition = HandPos.localPosition;
                CurrentGun.transform.localRotation = HandPos.localRotation;
                CurrentGun.GetComponent<WeaponInfo>().GunUser = this.gameObject;
            }
            else
            {
                //IF Holding PHONE IK ON, IF NOT NO IK
                if (Phone_SCRPT.isPhoneOn)
                {
                    IK_CTRL.ikActive = true;
                    IK_CTRL.lookObj = Phone_SCRPT.PhoneIKTarget;
                    anim.SetFloat("SprintStyle", 0, 0.2f, Time.deltaTime);
                    if (!CTRL_s.InCar)
                    {
                        IK_CTRL.leftHandObj = null;
                        IK_CTRL.rightHandObj = Phone_SCRPT.PhoneIKTarget;
                    }
                    else
                    {
                        if (GetComponent<Action_Car>().MySit.CUC.IKTargetLeft != null)
                        {
                            IK_CTRL.leftHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetLeft;
                        }
                        else
                        {
                            IK_CTRL.leftHandObj = null;
                        }
                        if (GetComponent<Action_Car>().MySit.CUC.IKTargetRight != null)
                        {
                            IK_CTRL.rightHandObj = Phone_SCRPT.PhoneIKTarget;
                        }
                        else
                        {
                            IK_CTRL.rightHandObj = Phone_SCRPT.PhoneIKTarget;
                        }
                    }
                }
                else
                {
                    IK_CTRL.lookObj = null;
                    anim.SetFloat("SprintStyle", 0, 0.2f, Time.deltaTime);
                    if (!CTRL_s.InCar)
                    {
                        IK_CTRL.ikActive = false;
                        IK_CTRL.leftHandObj = null;
                        IK_CTRL.rightHandObj = null;
                    }
                    else
                    {
                        if (GetComponent<Action_Car>().MySit.CUC.IKTargetLeft != null)
                        {
                            IK_CTRL.ikActive = true;
                            IK_CTRL.leftHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetLeft;
                        }
                        else
                        {
                            IK_CTRL.ikActive = false;
                            IK_CTRL.leftHandObj = null;
                        }
                        if (GetComponent<Action_Car>().MySit.CUC.IKTargetRight != null)
                        {
                            IK_CTRL.ikActive = true;
                            IK_CTRL.rightHandObj = GetComponent<Action_Car>().MySit.CUC.IKTargetRight;
                        }
                        else
                        {
                            IK_CTRL.ikActive = false;
                            IK_CTRL.rightHandObj = null;
                        }
                    }
                }
            }
        }
        else
        {
            IK_CTRL.ikActive = false;
            IK_CTRL.leftHandObj = null;
            IK_CTRL.rightHandObj = null;
            IK_CTRL.lookObj = null;
        }
    }

    void WeaponDetector()
    {
        if (CurrentSlot > 6)
        {
            CurrentSlot = 0;
        }
        // ---- Weapon Enable and Disable when Equipped or not ----
        if (CurrentSlot == 0)
        {
            CurrentGun = null;
        }

        if (CurrentSlot == 1)
        {
            if (Pistol != null)
            {
                CurrentGun = Pistol;
                Pistol.SetActive(true);
            }
            else
            {
                CurrentGun = null;
            }
        }
        else
        {
            if (Pistol != null)
            {
                Pistol.SetActive(false);
            }
        }

        if (CurrentSlot == 2)
        {
            if (Heavy != null)
            {
                CurrentGun = Heavy;
                Heavy.SetActive(true);
            }
            else
            {
                CurrentGun = null;
            }
        }
        else
        {
            if (Heavy != null)
            {
                Heavy.SetActive(false);
            }
        }

        if (CurrentSlot == 3)
        {
            if (SMG != null)
            {
                CurrentGun = SMG;
                SMG.SetActive(true);
            }
            else
            {
                CurrentGun = null;
            }
        }
        else
        {
            if (SMG != null)
            {
                SMG.SetActive(false);
            }
        }

        if (CurrentSlot == 4)
        {
            if (Assault != null)
            {
                CurrentGun = Assault;
                Assault.SetActive(true);
            }
            else
            {
                CurrentGun = null;
            }
        }
        else
        {
            if (Assault != null)
            {
                Assault.SetActive(false);
            }
        }

        if (CurrentSlot == 5)
        {
            if (Sniper != null)
            {
                CurrentGun = Sniper;
                Sniper.SetActive(true);
            }
            else
            {
                CurrentGun = null;
            }
        }
        else
        {
            if (Sniper != null)
            {
                Sniper.SetActive(false);
            }
        }

        if (CurrentSlot == 6)
        {
            if (Launchers != null)
            {
                CurrentGun = Launchers;
                Launchers.SetActive(true);
            }
            else
            {
                CurrentGun = null;
            }
        }
        else
        {
            if (Launchers != null)
            {
                Launchers.SetActive(false);
            }
        }

        // ---- that ends here ----


        if (CurrentSlot != 0)
        {
            if (CurrentGun == null)
            {
                CurrentSlot += 1;
            }
        }

    }

    void FireDetect(int LayerID)
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(LayerID);
        if (stateInfo.IsName("Shoot"))
        {
            if (Time.time > CurrentGun.GetComponent<WeaponInfo>().nextFire)
            {
                if (CurrentGun.GetComponent<WeaponInfo>().AmmoLeft > 0)
                {
                    firing();
                    CurrentGun.GetComponent<WeaponInfo>().currentRecoilZPos -= CurrentGun.GetComponent<WeaponInfo>().recoilAmount;
                    CurrentGun.GetComponent<WeaponInfo>().BulletSpray();
                    Global_Script.cam.GetComponent<CameraRig>().ShakeCamera(0.01f, 0.1f);
                }
                else
                {
                    CurrentGun.GetComponent<WeaponInfo>().currentRecoilZPos = Mathf.SmoothDamp(CurrentGun.GetComponent<WeaponInfo>().currentRecoilZPos, 0, ref CurrentGun.GetComponent<WeaponInfo>().currentRecoilZPosV, CurrentGun.GetComponent<WeaponInfo>().recoilRecoverTime);
                }
                if (CurrentGun.GetComponent<WeaponInfo>().AmmoLeft > 0)
                {
                    CurrentGun.GetComponent<WeaponInfo>().nextFire = Time.time + CurrentGun.GetComponent<WeaponInfo>().fireRate;
                    AudioSource.PlayClipAtPoint(CurrentGun.GetComponent<WeaponInfo>().gunShot, transform.position, AudioVolume);
                    CurrentGun.GetComponent<WeaponInfo>().AmmoLeft--;
                }
                else
                {
                    CurrentGun.GetComponent<WeaponInfo>().nextFire = Time.time + CurrentGun.GetComponent<WeaponInfo>().fireRate;
                    AudioSource.PlayClipAtPoint(CurrentGun.GetComponent<WeaponInfo>().EmptyShot, transform.position, 1);
                }
            }
        }
        else
        {
            CurrentGun.GetComponent<WeaponInfo>().XRotation = 0;
            CurrentGun.GetComponent<WeaponInfo>().YRotation = 0;
        }
        CurrentGun.GetComponent<WeaponInfo>().currentRecoilZPos = Mathf.SmoothDamp(CurrentGun.GetComponent<WeaponInfo>().currentRecoilZPos, 0, ref CurrentGun.GetComponent<WeaponInfo>().currentRecoilZPosV, CurrentGun.GetComponent<WeaponInfo>().recoilRecoverTime);
    }

    void GunRecoil()
    {
        if (CurrentGun != null)
        {
            RifleAimPos.transform.localPosition = new Vector3(0, 0, CurrentGun.GetComponent<WeaponInfo>().currentRecoilZPos);
            PistolAimPos.transform.localPosition = new Vector3(0, 0, CurrentGun.GetComponent<WeaponInfo>().currentRecoilZPos);
        }
    }

    void firing()
    {
        CurrentGun.GetComponent<WeaponInfo>().Muzzle.Emit(1);
        if (CurrentGun.GetComponent<WeaponInfo>().Trail)
        {
            CurrentGun.GetComponent<WeaponInfo>().Trail.Emit(1);
        }
        if (CurrentSlot != 6)
        {
            CurrentGun.GetComponent<WeaponInfo>().EjectBullet.Emit(1);
        }
        Ray bullet = new Ray(CurrentGun.GetComponent<WeaponInfo>().bulletspawn.position, CurrentGun.GetComponent<WeaponInfo>().bulletspawn.forward);
        RaycastHit hit;
        if (Physics.Raycast(bullet, out hit, 1000, CastLayer))
        {
            if (CurrentSlot != 6)
            {
                // if (hit.collider.gameObject.GetComponent<CarHealth>()) {
                // damage = CurrentGun.GetComponent<WeaponInfo>().Damage;
                //  hit.collider.gameObject.GetComponent<CarHealth>().Takedamage(damage);
                //}

                if (hit.collider.gameObject.GetComponent<VehiclManager>())
                {
                    for (int i = 0; i < hit.collider.gameObject.GetComponent<VehiclManager>().Doors.Length; i++)
                    {
                        if (hit.collider.gameObject.GetComponent<VehiclManager>().Doors[i].GetComponent<DoorInteraction>().User)
                        {
                            if (hit.collider.gameObject.GetComponent<VehiclManager>().Doors[i].GetComponent<DoorInteraction>().User.GetComponent<Health>().alive)
                            {
                                health.LastShooter = hit.collider.gameObject.GetComponent<VehiclManager>().Doors[i].GetComponent<DoorInteraction>().User;
                                damage = CurrentGun.GetComponent<WeaponInfo>().Damage;

                                hit.collider.gameObject.GetComponent<VehiclManager>().Doors[i].GetComponent<DoorInteraction>().User.GetComponent<Health>().Takedamage(damage * (0.5f), this.gameObject);
                                if (Global_Script.notorietyLevel == 0)
                                {
                                    Global_Script.notorietyLevel += 1;
                                }
                            }
                        }
                    }
                }

                if (hit.collider.gameObject.GetComponent<Hitmark>())
                {
                    if (hit.collider.gameObject.GetComponent<Hitmark>().HealthScript.gameObject != this.gameObject)
                    {
                        GameObject currentHitEffect = (GameObject)(Instantiate(hitEffect, hit.point, this.transform.rotation));
                        currentHitEffect.transform.SetParent(hit.transform);
                        if (hit.collider.gameObject.GetComponent<Hitmark>().HealthScript.alive)
                        {
                            //Increase Wanted Level
                            if (Global_Script.notorietyLevel == 0)
                            {
                                Global_Script.notorietyLevel += 1;
                            }

                            //Apply Damages
                            health.LastShooter = hit.collider.gameObject.GetComponent<Hitmark>().HealthScript.gameObject;
                            damage = CurrentGun.GetComponent<WeaponInfo>().Damage;
                            hit.collider.gameObject.GetComponent<Hitmark>().HealthScript.Takedamage(damage + hit.collider.gameObject.GetComponent<Hitmark>().DamageMultiplier, this.gameObject);

                            if ((hit.collider.gameObject.GetComponent<Hitmark>().HealthScript.health) <= (damage + hit.collider.gameObject.GetComponent<Hitmark>().DamageMultiplier))
                            {
                                // killed Status Update
                                Global_Script.Crosshair.GetComponent<Animator>().SetTrigger("KMark");
                                Global_Script.notorietyLevel += 1;
                                KilledTotal += 1;
                            }
                        }
                    }
                }
                else
                {
                    if (hit.collider.gameObject.GetComponent<Rigidbody>())
                    {
                        if (!hit.collider.gameObject.GetComponent<Rigidbody>().isKinematic)
                        {
                            Vector3 dir = hit.transform.position - transform.position;
                            dir = dir.normalized;

                            hit.rigidbody.AddForce(dir * 500);
                        }
                    }

                    if (!hit.collider.GetComponent<SurfaceType>())
                    {
                        GameObject currentMissEffect = (GameObject)(Instantiate(missEffect, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal)));
                        currentMissEffect.transform.SetParent(hit.transform);
                        GameObject.Destroy(currentMissEffect, missEffectDestroyTime);
                    }
                    else
                    {
                        if (hit.collider.GetComponent<SurfaceType>().TypeOfSurface == Surface.brick)
                        {
                            GameObject currentMissEffect = (GameObject)(Instantiate(BrickEffect, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal)));
                            currentMissEffect.transform.SetParent(hit.transform);
                            GameObject.Destroy(currentMissEffect, missEffectDestroyTime);
                        }
                        if (hit.collider.GetComponent<SurfaceType>().TypeOfSurface == Surface.metal)
                        {
                            GameObject currentMissEffect = (GameObject)(Instantiate(MetalEffect, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal)));
                            currentMissEffect.transform.SetParent(hit.transform);
                            GameObject.Destroy(currentMissEffect, missEffectDestroyTime);
                        }
                        if (hit.collider.GetComponent<SurfaceType>().TypeOfSurface == Surface.wood)
                        {
                            GameObject currentMissEffect = (GameObject)(Instantiate(woodEffect, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal)));
                            currentMissEffect.transform.SetParent(hit.transform);
                            GameObject.Destroy(currentMissEffect, missEffectDestroyTime);
                        }
                        if (hit.collider.GetComponent<SurfaceType>().TypeOfSurface == Surface.water)
                        {
                            GameObject currentMissEffect = (GameObject)(Instantiate(waterEffect, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal)));
                            currentMissEffect.transform.SetParent(hit.transform);
                            GameObject.Destroy(currentMissEffect, missEffectDestroyTime);
                        }
                        if (hit.collider.GetComponent<SurfaceType>().TypeOfSurface == Surface.glass)
                        {
                            GameObject currentMissEffect = (GameObject)(Instantiate(glassEffect, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal)));
                            currentMissEffect.transform.SetParent(hit.transform);
                            GameObject.Destroy(currentMissEffect, missEffectDestroyTime);
                        }
                        if (hit.collider.GetComponent<SurfaceType>().TypeOfSurface == Surface.dirt)
                        {
                            GameObject currentMissEffect = (GameObject)(Instantiate(dirtEffect, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal)));
                            currentMissEffect.transform.SetParent(hit.transform);
                            GameObject.Destroy(currentMissEffect, missEffectDestroyTime);
                        }
                    }
                }

            }
            else
            {
                GameObject currentMissEffect = (GameObject)(Instantiate(ExplosionEffect, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal)));
                GameObject.Destroy(currentMissEffect, 3);
            }
        }
    }
}
