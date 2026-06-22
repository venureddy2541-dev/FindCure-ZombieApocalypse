using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using StarterAssets;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public Waves level;
    bool inAttackRange = false;
    public ZombieData zombieData;
    int health;

    bool isActive = false;
    bool enemyDitected = false;

    public float exactDistance;
    [SerializeField] int zombieRotationSpeed;
    public float Speed;
    float angularSpeed;
    public float stopValueRef;

    public bool isProvoked = false;
    public bool dead = false;
    public bool reBirth = false;

    Collider colliderForEnemyLook;
    Vector3 dist;
    Vector3 ditectedObject;
    
    AudioSource zombieSounds;

    public Slider slider;
    public Animator enemyAnimator;
    
    public Collider destCollider;
    public NavMeshAgent navMesh;
    int radius = 10;
    bool CanMove = true;
    bool newPath = true;
    [SerializeField] LayerMask layers;
    float randomWalkPointObstacleCheckRadius = 0.25f;
    Vector3 pos;
    DamageManager damageManager;
    public GameObject player;
    public IsAlive isPlayerAlive;
    public GameObject playerMountedObject;
    public IsAlive isPlayerMountedAlive;

    [SerializeField] GameObject spawner;
    
    [SerializeField] AudioClip screamingSound;
    [SerializeField] ZombieInvicible zombieInvicible;

    public Vector3 latestHitDir;
    public float latestHitForce;
    bool isStopped = false;
    public bool canAttack = true;
    [SerializeField] int hitDamageToVehical;
    [SerializeField] AudioClip enemyHitByVehicalSound;
    Collider[] cols;
    Collider col;
    Rigidbody rb;
    EnemyAttack enemyAttack;
 
    protected virtual void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        damageManager = GetComponentInChildren<DamageManager>();
        enemyAttack = GetComponentInChildren<EnemyAttack>();

        slider.gameObject.SetActive(false);
        zombieSounds = GetComponent<AudioSource>();
        navMesh = GetComponent<NavMeshAgent>();
        Speed = navMesh.speed;
        angularSpeed = navMesh.angularSpeed;

        slider.maxValue = zombieData.health;
        slider.value = zombieData.health;
        health = zombieData.health;
    }

    protected virtual void OnEnable()
    {
        stopValueRef = zombieData.stopValue; 
        if (isActive)
        {
            AudioActivator();
        }
    }

    void AudioActivator()
    {
        zombieSounds.clip = screamingSound;
        zombieSounds.Play();
    }

    protected virtual void Start()
    {
        AudioActivator();
        isActive = true;
        enemyAnimator = GetComponentInChildren<Animator>();
        if(player != null)
        {
            AssiginPlayerData(player,playerMountedObject);
        }
    }

    protected virtual void Update()
    {
        if(isPlayerAlive == null || isPlayerMountedAlive == null) return;

        if(!dead && isPlayerMountedAlive.alive && isPlayerAlive.alive)
        {
            FindInRangeOrNot();

            if(exactDistance<zombieData.provokeRange)
            {
                if(isStopped){ isStopped = false; }
                if(enemyAnimator.GetBool("PlayerDead")) { enemyAnimator.SetBool("PlayerDead",false); }
                isProvoked = true;
            }
        }
        else if(isProvoked || !isStopped)
        {
            isStopped = true;
            isProvoked = false;
            StopEverything();
        }

        Provoked();
        RandomWalk();
    }

    public void AssiginPlayerData(GameObject playerRef,GameObject playerMountedObjectRef)
    {
        player = playerRef;
        playerMountedObject = playerMountedObjectRef;
        isPlayerAlive = (playerRef)? playerRef.GetComponent<IsAlive>() : null;
        isPlayerMountedAlive = (playerMountedObjectRef)? playerMountedObjectRef.GetComponent<IsAlive>() : null;
        cols = playerMountedObjectRef.GetComponents<Collider>();
        damageManager.ChangeTarget(playerMountedObjectRef);
    }

    void FindInRangeOrNot()
    {
        exactDistance = 0f;
        float maxDist = Mathf.Infinity;
        foreach(Collider newCol in cols)
        {
            dist = newCol.ClosestPoint(transform.position);
            float distance = Vector3.Distance(transform.position,dist);
            if(distance < maxDist)
            {
                maxDist = distance;
                ditectedObject = dist;
                exactDistance = maxDist;
            }
        }
    }

    void Provoked()
    {
        if(isProvoked)
        {
            if(!CanMove)
            {
                StopAllCoroutines();
                enemyAnimator.SetFloat("WalkIndex",0);
                CanMove = true;
            }

            EnemyProvoked();
        }
    }

    void RandomWalk()
    {
        if(!dead && !isProvoked)
        {
            if(CanMove)
            {
                CanMove = false;
                StartCoroutine(FindPath());
            }
        }
    }

    IEnumerator FindPath()
    {
        bool hasPath = false;

        while(!hasPath)
        {
            hasPath = FindAndSetPosition();
            yield return null;
        }

        while(navMesh.remainingDistance >= navMesh.stoppingDistance && navMesh.hasPath)
        {
            yield return null;
        }

        enemyAnimator.SetFloat("WalkIndex",0);
        navMesh.SetDestination(transform.position);
        navMesh.speed = 0;
        yield return new WaitForSeconds(Random.Range(5,6));
        CanMove = true;
    }

    bool FindAndSetPosition()
    {
        Vector3 newPosition = Random.insideUnitSphere*radius;
        newPosition += transform.position;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(newPosition,out hit,radius,NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if(navMesh.CalculatePath(hit.position,path) && path.status == NavMeshPathStatus.PathComplete)
            {
                Collider[] col = Physics.OverlapSphere(hit.position,randomWalkPointObstacleCheckRadius,layers);
                if(col.Length == 0) 
                { 
                    navMesh.speed = 0.3f;
                    navMesh.angularSpeed = angularSpeed;
                    enemyAnimator.SetFloat("WalkIndex",Random.Range(1,4));
                    navMesh.SetDestination(hit.position);
                    newPath = false;
                    return true;
                }
            }
        }

        return false;
    }

    void EnemyProvoked()
    {
        if(exactDistance > stopValueRef || enemyDitected)
        {
            enemyDitected = false;
            EnemyAudio();
            Behaviour();
        }

        if(exactDistance <= stopValueRef)
        {
            EnemyAudio();
            AttackRange();
        }
    }

    protected virtual void Behaviour()
    {
        if(!enemyAnimator.GetBool("running")) { enemyAnimator.SetFloat("RunIndex",CurrentChaseType()); }
        ChaseTarget();
    }

    void EnemyAudio()
    {
        if(zombieSounds.clip != screamingSound)
        {
            zombieSounds.Stop();
            zombieSounds.clip = screamingSound;
            zombieSounds.Play();
        }
    }

    protected virtual void StopEverything()
    {
        enemyAnimator.SetFloat("AttackType",0f);
        enemyAnimator.SetBool("PlayerDead",true);
    }

    public void ChaseTarget()
    {
        enemyAnimator.SetBool("running",true);
        navMesh.speed = Speed;
        navMesh.angularSpeed = angularSpeed;
        navMesh.SetDestination(playerMountedObject.transform.position);
        float offset = 0f;//StarterAssetsInputs.starterAssetsInputs.additionalDist;

        if(exactDistance < stopValueRef + 0.3f + offset && exactDistance > stopValueRef + offset)
        {
            inAttackRange = true;
            EnemyAudio();
            Attack();
        }
        else
        {
            canAttack = true;
            enemyAnimator.SetFloat("AttackType",0f);
            inAttackRange = false;
        }
    }

    int CurrentChaseType()
    {
        return Random.Range(0,4);
    }

    void AttackRange()
    {
        enemyAnimator.SetBool("running", false);
        navMesh.speed = 0;
        navMesh.angularSpeed = 0;

        Attack();
    }

    void Attack()
    {
        Vector3 rotation = playerMountedObject.transform.position - transform.position;
        rotation.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(rotation);
        transform.rotation = Quaternion.Slerp(transform.rotation,newRotation,Time.deltaTime*zombieRotationSpeed);
        if(canAttack)
        {
            canAttack = false;
            enemyAnimator.SetFloat("AttackType",CurrentAttackType());
        }
    }

    protected virtual int CurrentAttackType()
    {
        return Random.Range(1,3);
    }

    public void ChangeMountedObject(GameObject targetRef,IsAlive isALiveRef,float stopDist,Collider[] colsRef)
    {
        playerMountedObject = targetRef;
        isPlayerMountedAlive = isALiveRef;
        cols = colsRef;
        stopValueRef = stopDist;
    }

    public void ChangeTarget()
    {
        damageManager.ChangeTarget(playerMountedObject);
    }

    public void TakeDamage(int damage , Vector3 hitDirection , float hitForce)
    {
        if(!dead)
        {
            latestHitDir = hitDirection;
            latestHitForce = hitForce;
            slider.gameObject.SetActive(true);
            StartCoroutine(HealthHider());
            if(!isProvoked)
            {
                if(!CanMove)
                {
                    StopAllCoroutines();
                    enemyAnimator.SetFloat("WalkIndex",0);
                }
                enemyDitected = true;
                enemyAnimator.SetBool("provoked",true);
            }

            health -= damage;
            slider.value = health;
            if(health <= 0)
            {
                dead = true;
                if(!reBirth) { GameManager.gameManager.UpdateCash(zombieData.points); }
                zombieSounds.Stop();

                col.isTrigger = true;
                navMesh.enabled = false;
                slider.gameObject.SetActive(false);
                    
                if(level) level.Counting();
                
                enemyAttack.RegdolActivation();
            }
            else if(health >= 10 && health <= 20)
            {
                Crawl();
            }
        }
    }

    public void HitByVehical(int damage , Vector3 hitDirection , float hitForce)
    {
        if(!dead)
        {
            TakeDamage(damage,hitDirection,hitForce);
            zombieSounds.PlayOneShot(enemyHitByVehicalSound);
        }
    }

    protected virtual void Crawl()
    {
        if(!enemyAnimator.GetBool("crawl"))
        {
            stopValueRef = stopValueRef/2f;
            enemyAnimator.SetBool("crawl",true);
        }
    }

    IEnumerator HealthHider()
    {
        if(slider.gameObject.activeInHierarchy) yield return null;
        yield return new WaitForSeconds(1f);
        slider.gameObject.SetActive(false);
    }

    public void EnemyDeactiveState()
    {
        zombieInvicible.OpaqueToTransparent();
    }

    public void ForceReset()
    {
        zombieInvicible.ForceReset();
    }

    public void ResetEverything()
    {
        dead = false;
        enemyDitected = false;
        isProvoked = false;
        
        col.isTrigger = false;
        navMesh.enabled = true;

        navMesh.speed = Speed;
        navMesh.angularSpeed = angularSpeed;
        slider.value = zombieData.health;
        health = zombieData.health; 

        zombieInvicible.TransparentToOpaque();
    }

    public void ChasePlayer()
    {
        isProvoked = true;
        CanMove = true;
        enemyAnimator.SetBool("provoked",false);
    }

    public int Health
    {
        set { zombieData.health = value; }
        get { return health; }
    }
}
