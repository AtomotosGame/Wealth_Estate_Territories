using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MindState { 
    Friendly,
    Neutral,
    Enemy
}

public enum MoveState {
    Idle,
    Walking,
    Running
}

public enum GenderV {
    Male,
    Female
}

public enum Job
{
    None,
    Vender,
    hobo
}

public class MASAIBrain : MonoBehaviour {
    public Animator anim;
    AnimatorStateInfo stateInfo;
    Health Health_Script;
    public AICarAction AI_CarAction;
    [Header("AI Details")]
    public Transform WayPoint;
    public Transform FollowTarget;
    public Transform Target;
    public GenderV Gender;
    public MindState AIType;
    public Job AIJob;
    public MoveState AIMovementState;
    GlobalObjects Global_Script;
    [Header("Movement Details")]
    public float wanderRadius;
    public float wanderTimer;
    public float ShootFromRange = 10;
    public float moveSpeed;
    public float RunSpeed;
    public float KeepDistance = 2f;
    public float ParkourID;
    public bool Parkour;
    public bool InCar;
    public bool inJob;
    public int JobID;

    [Header("Inventory")]
    public Transform Gun;
    public Transform HandPosition;
    public bool hasRifle;

    [Header("Movement State")]
    public bool walking;
    public bool running;
    public bool Aim;
    public bool Attack;
    public bool Falling;
    public bool AlreadyInCar;
    private Transform target;
    private NavMeshAgent agent;
    private float timer;
    Vector3 newPos;


    [Header("FiringSetting")]
    [Range(0, 1)]
    public float FiringRate;
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

    [Header("Parkour Editor")]
    public bool PHigh;
    public bool PLow;
    public bool PStep;
    public bool PDense;
    public bool PJump;
    public Transform ParkourDetector;
    public Transform ParkourDetectorTop;
    public Transform ParkourDetectorDense;
    public Transform ParkourDetectorStep;
    public Transform ParkourDetectorJump;
    public float prkrdtector = 0.4f;

    // Match Track
    Vector3 m_Target = new Vector3();
    private Vector3 dir;
    Transform ClosestDoor;

    Ray VaultRay;
    RaycastHit VaultHit;
    Ray ClimbRay;
    RaycastHit ClimbHit;
    Ray DenseRay;
    RaycastHit DenseHit;
    Ray StepRay;
    RaycastHit StepHit;
    Ray Coverray;
    RaycastHit Coverhit;

    private const float m_VaultMatchTargetStart = 0.06f;
    private const float m_VaultMatchTargetStop = 0.47f;
    private const float m_ClimbMatchTargetStart = 0.18f;
    private const float m_ClimbMatchTargetStop = 0.35f;
    private const float m_StepMatchTargetStart = 0.05f;
    private const float m_StepMatchTargetStop = 0.30f;
    private const float m_CoverMatchTargetStart = 0.05f;
    private const float m_CoverMatchTargetStop = 0.50f;


    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        if (Gun) {
            Gun.GetComponent<WeaponInfo>().GunUser = this.gameObject;
        }
        if (AlreadyInCar) {
            anim.SetTrigger("AlreadyInCar");
        }

        Global_Script = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalObjects>();
        Health_Script = GetComponent<Health>();
    }
	
	// Update is called once per frame
	void Update () {
        AIBrain();
        AssignSpeed();
        AssignStates();
        GunSetup();
        Retargetation();
        ParkourSystem();
        ParkourDetect();
        ProcessMatchTarget();
        GroundedCheck();
        AIAimable();
    }

    void AIBrain()
    {
        if (Health_Script.alive)
        {
            //Decide to get in car or not 
            if (!GetComponent<AILaw>())
            {
                if (FollowTarget)
                {
                    if (AIType == MindState.Friendly)
                    {
                        if ((transform.position - FollowTarget.transform.position).sqrMagnitude < (KeepDistance * KeepDistance) * 10)
                        {
                            if (FollowTarget.GetComponent<Controller>())
                            {
                                if (FollowTarget.GetComponent<Controller>().InCar)
                                {
                                    AI_CarAction.GetIn = true;

                                }
                                else
                                {
                                    AI_CarAction.GetIn = false;
                                }
                            }
                            if (FollowTarget.GetComponent<MASAIBrain>())
                            {
                                if (FollowTarget.GetComponent<MASAIBrain>().InCar)
                                {
                                    AI_CarAction.GetIn = true;
                                }
                                else
                                {
                                    AI_CarAction.GetIn = false;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Target)
                {
                    if ((transform.position - Target.transform.position).sqrMagnitude < (KeepDistance * KeepDistance) * 10)
                    {
                        AI_CarAction.GetIn = false;
                    }
                    else {
                        AI_CarAction.GetIn = true;
                    }

                }
            }

            // Target Set
            if (Target)
            {
                if (Target.GetComponent<Health>())
                {
                    if (Target.GetComponent<Health>().alive)
                    {
                        if (FollowTarget)
                        {
                            if ((transform.position - Target.transform.position).sqrMagnitude < (KeepDistance * KeepDistance) * ShootFromRange)
                            {
                                if (Gun)
                                {
                                    Aim = true;
                                    Attack = true;
                                    if ((transform.position - FollowTarget.transform.position).sqrMagnitude < (KeepDistance * KeepDistance) * 3)
                                    {
                                        if (!InCar)
                                        {
                                            Vector3 lookDir = Target.position - this.transform.position;
                                            Quaternion lookRot = Quaternion.LookRotation(lookDir);
                                            lookRot.x = 0;
                                            lookRot.z = 0;
                                            Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 5f);
                                            transform.rotation = newRotation;
                                        }
                                    }
                                }
                                else
                                {
                                    Aim = false;
                                    Attack = false;
                                }
                            }
                            else
                            {
                                Aim = false;
                                Attack = false;
                            }
                        }
                        else
                        {
                            FollowTarget = Target;
                        }
                    }
                    else
                    {
                        Target = null;
                        Aim = false;
                        Attack = false;
                    }
                }
                else
                {
                    if ((transform.position - Target.transform.position).sqrMagnitude < (KeepDistance * KeepDistance) * 10)
                    {
                        if (Gun)
                        {
                            Aim = true;
                            Attack = true;
                            if ((transform.position - FollowTarget.transform.position).sqrMagnitude < (KeepDistance * KeepDistance) * 3)
                            {
                                if (!InCar)
                                {
                                    Vector3 lookDir = Target.position - this.transform.position;
                                    Quaternion lookRot = Quaternion.LookRotation(lookDir);
                                    lookRot.x = 0;
                                    lookRot.z = 0;
                                    Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 5f);
                                    transform.rotation = newRotation;
                                }
                            }
                        }
                    }
                }
            }


            // Movement Destination
            if (agent.enabled)
            {
                if (AIJob == Job.None)
                {
                    inJob = false;
                    JobID = 0;
                    if (!FollowTarget)
                    {
                        if (!WayPoint)
                        {

                            if (Health_Script.LastShooter == null)
                            {
                                Freeroam();
                            }
                            else
                            {
                                if (!Gun)
                                {
                                    if (Health_Script.LastShooter.GetComponent<Health>().alive)
                                    {

                                        RunAway();
                                    }
                                    else
                                    {
                                        Health_Script.LastShooter = null;
                                    }
                                }
                                else {
                                    FollowTarget = Health_Script.LastShooter.transform;
                                }
                            }
                        }
                        else
                        {
                            if (Health_Script.LastShooter == null)
                            {
                                agent.SetDestination(WayPoint.transform.position);
                                agent.stoppingDistance = 0;


                                if ((transform.position - WayPoint.transform.position).sqrMagnitude < (KeepDistance))
                                {
                                    AIMovementState = MoveState.Idle;
                                    if (WayPoint.GetComponent<WayPointDistribution>())
                                    {
                                        WayPoint.GetComponent<WayPointDistribution>().SetDestination(Random.Range(0, 5));
                                        WayPoint = WayPoint.GetComponent<WayPointDistribution>().NextWayPoint;
                                    }
                                }
                                else
                                {
                                    if (!Health_Script.LastShooter)
                                    {
                                        AIMovementState = MoveState.Walking;
                                    }
                                    else
                                    {
                                        AIMovementState = MoveState.Running;
                                    }
                                }
                            }
                            else
                            {
                                if (!Gun)
                                {
                                    if (Health_Script.LastShooter.GetComponent<Health>().alive)
                                    {

                                        RunAway();
                                    }
                                    else
                                    {
                                        Health_Script.LastShooter = null;
                                    }
                                }
                                else
                                {
                                    WayPoint = null;
                                    FollowTarget = Health_Script.LastShooter.transform;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (FollowTarget.GetComponent<Health>().alive)
                        {
                            if (AIType == MindState.Friendly || AIType == MindState.Neutral)
                            {
                                if (!AI_CarAction.GetIn)
                                {
                                    
                                    agent.SetDestination(FollowTarget.transform.position);
                                    agent.stoppingDistance = (KeepDistance) * 0.5f;

                                    if ((transform.position - FollowTarget.transform.position).sqrMagnitude < (KeepDistance * KeepDistance))
                                    {
                                        if ((transform.position - FollowTarget.transform.position).sqrMagnitude < (KeepDistance))
                                        {
                                            AIMovementState = MoveState.Idle;
                                        }
                                        else
                                        {
                                            AIMovementState = MoveState.Walking;
                                        }
                                    }
                                    else
                                    {
                                        AIMovementState = MoveState.Running;
                                    }
                                }
                                else
                                {
                                    if (!AI_CarAction.MySit)
                                    {
                                        EnterNearestDoor();
                                    }
                                }
                            }
                            if (AIType == MindState.Enemy)
                            {
                                if (Gun)
                                {
                                    agent.SetDestination(FollowTarget.transform.position);
                                    agent.stoppingDistance = 1f;

                                    if ((transform.position - FollowTarget.transform.position).sqrMagnitude < (KeepDistance * KeepDistance) * 3)
                                    {
                                        if ((transform.position - FollowTarget.transform.position).sqrMagnitude < (KeepDistance))
                                        {
                                            AIMovementState = MoveState.Idle;
                                        }
                                        else
                                        {
                                            AIMovementState = MoveState.Walking;
                                        }
                                    }
                                    else
                                    {
                                        AIMovementState = MoveState.Running;
                                    }
                                }
                                else
                                {
                                    agent.SetDestination(FollowTarget.transform.position);
                                    agent.stoppingDistance = 1f;

                                    if ((transform.position - FollowTarget.transform.position).sqrMagnitude < (KeepDistance * KeepDistance))
                                    {
                                        if ((transform.position - FollowTarget.transform.position).sqrMagnitude < (KeepDistance))
                                        {
                                            AIMovementState = MoveState.Idle;
                                        }
                                        else
                                        {
                                            AIMovementState = MoveState.Walking;
                                        }
                                    }
                                    else
                                    {
                                        AIMovementState = MoveState.Running;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Freeroam();
                            FollowTarget = null;
                        }
                    }
                }
                else {
                    inJob = true;
                    if (AIJob == Job.Vender)
                    {
                        JobID = 1;
                    }


                    //Disable job if distracted
                    if (Health_Script.LastShooter)
                    {
                        AIJob = Job.None;
                    }
                }
            }
            else
            {
                Aim = false;
                Attack = false;
            }
        }
        else
        {
            if (Gun != null)
            {
                Gun.transform.parent = null;
                Gun.GetComponent<WeaponInfo>().GunUser = null;
                Gun = null;
            }
        }
    }

    void AIAimable()
    {
        if (Global_Script.LocalPlayer)
        {
            if (AIType != MindState.Friendly)
            {
                if ((transform.position - Global_Script.LocalPlayer.transform.position).sqrMagnitude < 5 * 5)
                {
                    Global_Script.LocalPlayer.GetComponent<Controller>().AutoTarget = this.transform;
                }
                else
                {
                    if (Global_Script.LocalPlayer.GetComponent<Controller>().AutoTarget)
                    {
                        if (Global_Script.LocalPlayer.GetComponent<Controller>().AutoTarget == this.transform)
                        {
                            Global_Script.LocalPlayer.GetComponent<Controller>().AutoTarget = null;
                        }
                    }
                }
            }
        }
    }

    void Freeroam() {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        if (transform.position != newPos)
        {
            AIMovementState = MoveState.Walking;
        }
        else
        {
            AIMovementState = MoveState.Idle;
        }
    }

    void EnterNearestDoor()
    {
        if (FollowTarget.GetComponent<Action_Car>())
        {
            if (FollowTarget.GetComponent<Action_Car>().MySit)
            {
                float distance = Mathf.Infinity;

                foreach (Transform door in FollowTarget.GetComponent<Action_Car>().MySit.GetComponent<DoorInteraction>().CUC.Doors)
                {
                    Vector3 diff = (door.transform.position - this.transform.position);
                    float curDistance = diff.sqrMagnitude;
                    if (!door.GetComponent<DoorInteraction>().User)
                    {
                        if (curDistance < distance)
                        {
                            ClosestDoor = door;
                            distance = curDistance;
                            agent.SetDestination(door.position);
                            agent.stoppingDistance = 0;
                        }
                    }
                    else
                    {
                        Freeroam();
                    }
                }
            }
        }
        if (FollowTarget.GetComponent<AICarAction>())
        {
            if (FollowTarget.GetComponent<AICarAction>().MySit)
            {
                float distance = Mathf.Infinity;

                foreach (Transform door in FollowTarget.GetComponent<AICarAction>().MySit.GetComponent<DoorInteraction>().CUC.Doors)
                {
                    Vector3 diff = (door.transform.position - this.transform.position);
                    float curDistance = diff.sqrMagnitude;
                    if (!door.GetComponent<DoorInteraction>().User)
                    {
                        if (curDistance < distance)
                        {
                            ClosestDoor = door;
                            distance = curDistance;
                            agent.SetDestination(door.position);
                            agent.stoppingDistance = 0;
                        }
                    }
                    else
                    {
                        Freeroam();
                    }
                }
            }
        }
    }

    void RunAway()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        if (transform.position != newPos)
        {
            AIMovementState = MoveState.Running;
        }
        else
        {
            AIMovementState = MoveState.Idle;
        }
    }

    void Retargetation()
    {
        if (Gun)
        {
            if (Health_Script.LastShooter)
            {
                if (Health_Script.LastShooter.GetComponent<Health>().alive)
                {
                    if (Health_Script.LastShooter.GetComponent<MASAIBrain>())
                    {
                        if (Health_Script.LastShooter.GetComponent<MASAIBrain>().AIType != AIType)
                        {
                            Target = Health_Script.LastShooter.transform;
                        }
                    }
                    else {
                        Target = Health_Script.LastShooter.transform;
                    }
                }
                else
                {
                    Health_Script.LastShooter = null;
                }
            }

            if (AIType == MindState.Friendly)
            {
                if (FollowTarget)
                {
                    if (!Target)
                    {
                        if (FollowTarget.GetComponent<Health>().LastShooter)
                        {
                            if (FollowTarget.GetComponent<Health>().LastShooter.GetComponent<Health>().alive)
                            {
                                if (FollowTarget.GetComponent<Health>().LastShooter != this.gameObject)
                                {
                                    Target = FollowTarget.GetComponent<Health>().LastShooter.transform;
                                }
                            }
                            else
                            {
                                Target = null;
                            }
                        }
                    }
                }
            }
        }
    }

    void GunSetup()
    {
        if (Health_Script.alive)
        {
            if (Gun)
            {
                if (Gun.GetComponent<WeaponInfo>().SlotID != 0 && Gun.GetComponent<WeaponInfo>().SlotID != 1 && Gun.GetComponent<WeaponInfo>().SlotID != 3)
                {
                    hasRifle = true;
                    FireDetect("Shoot Rifle");
                }
                else
                {
                    hasRifle = false;
                    FireDetect("Shoot Pistol");
                }

                Gun.parent = HandPosition;

                if (Gun.transform.position != HandPosition.position)
                {
                    Gun.rotation = HandPosition.rotation;
                    Gun.position = HandPosition.position;
                }
            }
        }
    }

    void AssignSpeed() {
        if (Health_Script.alive)
        {
            if (anim.GetFloat("ParkourState") > 0)
            {
                agent.speed = 1.2f;
                running = false;
                walking = false;
            }
            else {
                if (AIMovementState == MoveState.Idle)
                {
                    agent.speed = 0;
                    running = false;
                    walking = false;
                }
                if (AIMovementState == MoveState.Walking)
                {
                    walking = true;
                    running = false;
                    stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                    if (stateInfo.IsName("Walk"))
                    {
                        agent.speed = moveSpeed;
                    }
                    else {
                        agent.speed = 0;
                    }
                }
                if (AIMovementState == MoveState.Running)
                {
                    walking = false;
                    running = true;
                    stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                    if (stateInfo.IsName("Run"))
                    {
                        agent.speed = RunSpeed;
                    }
                    else {
                        agent.speed = 0;
                    }
                }
            }

        }
        else {
            agent.speed = 0;
            running = false;
            walking = false;
        }
    }

    void AssignStates() {
        anim.SetFloat("Speed", agent.speed);
        anim.SetBool("Walk", walking);
        anim.SetBool("Run", running);
        anim.SetBool("hasRifle", hasRifle);
        anim.SetBool("Aim", Aim);
        anim.SetBool("Fire", Attack);
        anim.SetBool("Jump", Parkour);
        anim.SetFloat("JumpID", ParkourID);
        anim.SetBool("Fall", Falling);
        anim.SetBool("InCar", InCar);
        anim.SetBool("DoorOpened", AI_CarAction.DoorOpened);
        anim.SetFloat("SeatSide", AI_CarAction.SeatSide);
        anim.SetFloat("SeatID", AI_CarAction.SeatID);
        anim.SetBool("InJob", inJob);
        anim.SetFloat("JobID", JobID);
        if (Gender == GenderV.Male)
        {
            anim.SetFloat("Gender", 0);
        }
        else {
            anim.SetFloat("Gender", 1);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    void FireDetect(string StateName)
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(1);
        if (stateInfo.IsName(StateName))
        {
            if ((Time.time*FiringRate) > Gun.GetComponent<WeaponInfo>().nextFire)
            {
                if (Gun.GetComponent<WeaponInfo>().AmmoLeft > 0)
                {
                    firing();
                    Gun.GetComponent<WeaponInfo>().currentRecoilZPos -= Gun.GetComponent<WeaponInfo>().recoilAmount;
                    Gun.GetComponent<WeaponInfo>().BulletSpray();
                }
                else
                {
                    Gun.GetComponent<WeaponInfo>().currentRecoilZPos = Mathf.SmoothDamp(Gun.GetComponent<WeaponInfo>().currentRecoilZPos, 0, ref Gun.GetComponent<WeaponInfo>().currentRecoilZPosV, Gun.GetComponent<WeaponInfo>().recoilRecoverTime);
                }
                if (Gun.GetComponent<WeaponInfo>().AmmoLeft > 0)
                {
                    Gun.GetComponent<WeaponInfo>().nextFire = Time.time + Gun.GetComponent<WeaponInfo>().fireRate;
                    AudioSource.PlayClipAtPoint(Gun.GetComponent<WeaponInfo>().gunShot, transform.position, AudioVolume);
                    Gun.GetComponent<WeaponInfo>().AmmoLeft--;
                }
                else
                {
                    Gun.GetComponent<WeaponInfo>().nextFire = Time.time + Gun.GetComponent<WeaponInfo>().fireRate;
                    AudioSource.PlayClipAtPoint(Gun.GetComponent<WeaponInfo>().EmptyShot, transform.position, 1);
                }
            }
        }
        else
        {
        }
        Gun.GetComponent<WeaponInfo>().currentRecoilZPos = Mathf.SmoothDamp(Gun.GetComponent<WeaponInfo>().currentRecoilZPos, 0, ref Gun.GetComponent<WeaponInfo>().currentRecoilZPosV, Gun.GetComponent<WeaponInfo>().recoilRecoverTime);
    }

    void firing()
    {
        Gun.GetComponent<WeaponInfo>().Muzzle.Emit(1);
        if (Gun.GetComponent<WeaponInfo>().Trail)
        {
            Gun.GetComponent<WeaponInfo>().Trail.Emit(1);
        }

        if (Gun.GetComponent<WeaponInfo>().SlotID != 6)
        {
            Gun.GetComponent<WeaponInfo>().EjectBullet.Emit(1);
        }
        Ray bullet = new Ray(Gun.GetComponent<WeaponInfo>().bulletspawn.position, Gun.GetComponent<WeaponInfo>().bulletspawn.forward);
        RaycastHit hit;
        if (Physics.Raycast(bullet, out hit, 1000, CastLayer))
        {
            if (hit.collider.gameObject.GetComponent<VehiclManager>())
            {
                for (int i = 0; i < hit.collider.gameObject.GetComponent<VehiclManager>().Doors.Length; i++)
                {
                    if (hit.collider.gameObject.GetComponent<VehiclManager>().Doors[i].GetComponent<DoorInteraction>().User)
                    {
                        if (hit.collider.gameObject.GetComponent<VehiclManager>().Doors[i].GetComponent<DoorInteraction>().User.GetComponent<Health>().alive)
                        {
                            damage = Gun.GetComponent<WeaponInfo>().Damage;

                            hit.collider.gameObject.GetComponent<VehiclManager>().Doors[i].GetComponent<DoorInteraction>().User.GetComponent<Health>().Takedamage(damage * (0.5f), this.gameObject);
                        }
                    }
                }
            }

            if (Gun.GetComponent<WeaponInfo>().SlotID != 6)
            {
                if (hit.collider.gameObject.GetComponent<Hitmark>())
                {
                    if (hit.collider.gameObject.GetComponent<Hitmark>().HealthScript.gameObject != this.gameObject)
                    {
                        GameObject currentHitEffect = (GameObject)(Instantiate(hitEffect, hit.point, this.transform.rotation));
                        currentHitEffect.transform.SetParent(hit.transform);
                        if (hit.collider.gameObject.GetComponent<Hitmark>().HealthScript.alive)
                        {
                            damage = Gun.GetComponent<WeaponInfo>().Damage * 0.5f;
                            hit.collider.gameObject.GetComponent<Hitmark>().HealthScript.Takedamage((damage) , this.gameObject);
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

    void GroundedCheck()
    {
        Ray PJumpRay = new Ray(ParkourDetectorJump.position, ParkourDetectorJump.forward);
        RaycastHit hitJump;


        if (Physics.Raycast(PJumpRay, out hitJump, 1.2f))
        {
            PJump = true;

        }
        else
        {
            PJump = false;
        }

        Falling = !PJump;
    }

    public void ParkourDetect()
    {
        VaultRay = new Ray(ParkourDetector.position, ParkourDetector.forward);
        ClimbRay = new Ray(ParkourDetectorTop.position, ParkourDetectorTop.forward);
        DenseRay = new Ray(ParkourDetectorDense.position, ParkourDetectorDense.forward);
        StepRay = new Ray(ParkourDetectorStep.position, ParkourDetectorStep.forward);
        dir = transform.TransformDirection(Vector3.forward);


        if (Physics.Raycast(VaultRay, out VaultHit, prkrdtector))
        {
            if (VaultHit.collider.gameObject.tag == "Parkour")
            {
                PLow = true;
            }
            else
            {
                PLow = false;
            }
        }
        else
        {
            PLow = false;
        }

        if (Physics.Raycast(ClimbRay, out ClimbHit, prkrdtector))
        {
            if (ClimbHit.collider.gameObject.tag == "Parkour")
            {
                PHigh = true;
            }
            else
            {
                PHigh = false;
            }
        }
        else
        {
            PHigh = false;
        }

        if (Physics.Raycast(DenseRay, out DenseHit, prkrdtector))
        {
            if (DenseHit.collider.gameObject.tag == "Parkour")
            {
                PDense = true;
            }
            else
            {
                PDense = false;
            }
        }
        else
        {
            PDense = false;
        }

        if (Physics.Raycast(StepRay, out StepHit, prkrdtector))
        {
            if (StepHit.collider.gameObject.tag == "Parkour")
            {
                PStep = true;
            }
            else
            {
                PStep = false;
            }
        }
        else
        {
            PStep = false;
        }
    }

    void ParkourSystem()
    {
        if (anim.GetFloat("ParkourState") == 0)
        {
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Run"))
            {
                if (PLow == true && PHigh == false && PStep == true)
                {
                    m_Target = VaultHit.point;
                    m_Target.y = (VaultHit.collider.bounds.extents.y) + (VaultHit.transform.position.y);
                    if (PDense == true)
                    {
                        ParkourID = 3;
                    }
                    else
                    {
                        ParkourID = 2;
                    }
                    Parkour = true;
                }
                if (PLow == true && PHigh == false && PStep == false)
                {
                    m_Target = VaultHit.point;
                    m_Target.y = (VaultHit.collider.bounds.extents.y) + (VaultHit.transform.position.y);
                    if (PDense == true)
                    {
                        ParkourID = 3;
                    }
                    else
                    {
                        ParkourID = 2;
                    }
                    Parkour = true;
                }
                if (PLow == false && PHigh == false && PStep == true)
                {
                    m_Target = StepHit.point;
                    m_Target.y = (StepHit.collider.bounds.extents.y) + (StepHit.transform.position.y);
                    ParkourID = 1;
                    Parkour = true;
                }
                if (PLow == true && PHigh == true && PStep == true)
                {
                    m_Target = ClimbHit.point;
                    m_Target.y = (ClimbHit.collider.bounds.extents.y) + (ClimbHit.transform.position.y) + 0.1f;
                    ParkourID = 4;
                    Parkour = true;
                }
                if (PLow == false && PHigh == true && PStep == false)
                {
                    m_Target = ClimbHit.point;
                    m_Target.y = (ClimbHit.collider.bounds.extents.y) + (ClimbHit.transform.position.y) + 0.1f;
                    ParkourID = 4;
                    Parkour = true;
                }

            }
            else
            {
                Parkour = false;
            }
        }
        else
        {
            Aim = false;
            Parkour = false;
        }
    }

    void ProcessMatchTarget()
    {
        if (anim.IsInTransition(0))
            return;

        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Jump_Action"))
        {
            if (ParkourID == 2)
            {
                anim.MatchTarget(m_Target, new Quaternion(), AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), m_VaultMatchTargetStart, m_VaultMatchTargetStop);
            }
            if (ParkourID == 3)
            {
                anim.MatchTarget(m_Target, new Quaternion(), AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), m_VaultMatchTargetStart, m_VaultMatchTargetStop);
            }
            if (ParkourID == 1)
            {
                anim.MatchTarget(m_Target, new Quaternion(), AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0), m_StepMatchTargetStart, m_StepMatchTargetStop);
            }
            if (ParkourID == 4)
            {
                anim.MatchTarget(m_Target, new Quaternion(), AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), m_ClimbMatchTargetStart, m_ClimbMatchTargetStop);
            }
        }
    }
}
