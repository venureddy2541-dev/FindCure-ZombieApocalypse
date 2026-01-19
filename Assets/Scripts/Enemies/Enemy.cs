using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using StarterAssets;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] int points;
    public int healthRef;
    [SerializeField] int health = 100;

    bool isActive = false;
    bool enemyDitected = false;

    public float enemyRange = 20f; 
    public float exactDistance;
    [SerializeField] int zombieRotationSpeed;
    public float Speed;
    float angularSpeed;
    float stopValue = 1.2f;
    public float stopValueRef;

    public bool isProvoked = false;
    public bool dead = false;
    public bool reBirth = false;
    public bool tempPlayerActive = false;

    Collider colliderForEnemyLook;
    Vector3 dist;
    Vector3 ditectedObject;
    
    AudioSource zombieSounds;

    public Vector3 startPos;
    public Slider slider;
    public Animator enemyAnimator;
    
    public Collider destCollider;
    public NavMeshAgent navMesh;
    int radius = 10;
    bool CanMove = true;
    bool newPath = true;
    [SerializeField] LayerMask layers;
    public float newRadius = 0.25f;
    Vector3 pos;
    public GameObject player;
    public IsAlive isPlayerAlive;
    public GameObject playerMountedObject;
    public IsAlive isPlayerMountedAlive;

    [SerializeField] GameObject spawner;
    
    [SerializeField] AudioClip screamingSound;
    [SerializeField] ZombieInvicible[] zombieInvicibleArray;

    public Vector3 latestHitDir;
    public float latestHitForce;
    bool isStopped = false;
    public bool canAttack = true;
 
    protected virtual void Awake()
    {
        slider.gameObject.SetActive(false);
        zombieSounds = GetComponent<AudioSource>();
        navMesh = GetComponent<NavMeshAgent>();
        Speed = navMesh.speed;
        angularSpeed = navMesh.angularSpeed;
        slider.maxValue = health;
    }

    protected virtual void OnEnable()
    {
        stopValueRef = stopValue; 
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
        slider.value = health;
        AudioActivator();
        isActive = true;
        startPos = transform.position;
        healthRef = health;
        isPlayerAlive = player.GetComponent<IsAlive>();
        isPlayerMountedAlive = playerMountedObject.GetComponent<IsAlive>();
        enemyAnimator = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        if(!dead && isPlayerMountedAlive.alive && isPlayerAlive.alive)
        {
            FindInRangeOrNot();

            if(exactDistance<enemyRange)
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

    void FindInRangeOrNot()
    {
        Collider[] col = playerMountedObject.GetComponents<Collider>();
        exactDistance = 0f;
        float maxDist = Mathf.Infinity;
        foreach(Collider newCol in col)
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
                Collider[] col = Physics.OverlapSphere(hit.position,newRadius,layers);
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
        if(exactDistance > stopValueRef + StarterAssetsInputs.starterAssetsInputs.additionalDist || enemyDitected)
        {
            enemyDitected = false;
            EnemyAudio();
            Behaviour();
        }

        if(exactDistance <= stopValueRef + StarterAssetsInputs.starterAssetsInputs.additionalDist)
        {
            EnemyAudio();
            AttackTarget();
        }
    }

    protected virtual void Behaviour()
    {
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
        canAttack = true;
        enemyAnimator.SetFloat("AttackType",0f);
        enemyAnimator.SetBool("running",true);
        navMesh.speed = Speed;
        navMesh.angularSpeed = angularSpeed;
        navMesh.SetDestination(playerMountedObject.transform.position);
    }

    void AttackTarget()
    {
        Vector3 rotation = playerMountedObject.transform.position - transform.position;
        rotation.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(rotation);
        transform.rotation = Quaternion.Slerp(transform.rotation,newRotation,Time.deltaTime*zombieRotationSpeed);

        enemyAnimator.SetBool("running", false);
        navMesh.speed = 0;
        navMesh.angularSpeed = 0;
        if(canAttack)
        {
            canAttack = false;
            enemyAnimator.SetFloat("AttackType",CurrentAttackType());
        }
    }

    protected virtual float CurrentAttackType()
    {
        return Random.Range(1f,3f);
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

            healthRef -= damage;
            slider.value = healthRef;
            if(healthRef <= 0)
            {
                dead = true;
                if(!reBirth) { GameManager.gameManager.UpdateCash(points); }
                ZombieDeadState();
                gameObject.GetComponent<Collider>().isTrigger = true;
                slider.gameObject.SetActive(false);
                Waves wave = transform.parent.parent.GetComponent<Waves>();
                if(wave)
                {
                    wave.Counting();
                }
                GetComponentInChildren<EnemyAttack>().RegdolActivation();
            }
            else if(healthRef >= 10 && healthRef <= 20)
            {
                if(!enemyAnimator.GetBool("crawl"))
                {
                    stopValueRef = stopValueRef - 0.8f;
                    enemyAnimator.SetBool("crawl",true);
                }
            }
        }
    }

    IEnumerator HealthHider()
    {
        if(slider.gameObject.activeInHierarchy) yield return null;
        yield return new WaitForSeconds(1f);
        slider.gameObject.SetActive(false);
    }

    public void ZombieDeadState()
    {
        zombieSounds.Stop();
    }

    public void EnemyDeactiveState()
    {
        foreach(ZombieInvicible zombieInvicible in zombieInvicibleArray)
        {
            zombieInvicible.OpaqueToTransparent();
        }
    }

    public void ResetEverything()
    {
        if (reBirth)
        {
            gameObject.GetComponent<Collider>().isTrigger = false;
            navMesh.speed = Speed;
            navMesh.angularSpeed = angularSpeed;
            dead = false;
            enemyDitected = false;
            isProvoked = false;
            transform.position = startPos;
            slider.value = health;
            healthRef = health; 
            foreach(ZombieInvicible zombieInvicible in zombieInvicibleArray)
            {
                zombieInvicible.TransparentToOpaque();
            }
        }
    }

    public void ChasePlayer()
    {
        isProvoked = true;
        CanMove = true;
        enemyAnimator.SetBool("provoked",false);
    }

    public int Health
    {
        set { health = value; }
    }

    public IsAlive AliveChanger
    {
        set { isPlayerMountedAlive = value; }
    }

    /*public void IfActiveSetIdle(GameObject currrentPlayerObject)
    {
        Debug.Log("Triggered");
        if(!dead)
        {   
            Debug.Log("inside");
            ToggleEnemyAttack();
        }
        player = currrentPlayerObject;
    }*/

    /*protected virtual void ToggleEnemyAttack()
    {
        if(!enemyAnimator.GetBool("PlayerDead") && isProvoked)
        {
            Debug.Log("hello");
            StopEverything();
        }
        else if(isProvoked)
        {
            Debug.Log("hi");
            enemyAnimator.SetBool("PlayerDead",false);
            navMesh.speed = Speed;
        }
    }*/
}
