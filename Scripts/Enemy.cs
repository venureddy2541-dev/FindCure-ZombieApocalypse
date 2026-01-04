using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using StarterAssets;

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
    public float stopValue = 1.2f;

    public bool isProvoked = false;
    public bool dead = false;
    public bool reBirth = false;
    public bool acidAttack = false;
    public bool tempPlayerActive = false;

    Collider colliderForEnemyLook;
    Vector3 dist;
    Vector3 ditectedObject;
    BloodSplit bloodSplit;
    AudioSource zombieSounds;

    public Vector3 startPos;
    public Slider slider;
    public Animator enemyAnimator;
    public GameObject orgPlayer;
    public GameObject player;
    IsAlive isAlive;
    public Collider destCollider;
    public NavMeshAgent navMesh;
    int radius = 10;
    bool CanMove = true;
    [SerializeField] LayerMask layers;
    float newRadius = 0.5f;
    Vector3 pos;
    IsAlive playerIsAlive;

    [SerializeField] GameObject spawner;
    
    [SerializeField] AudioClip screamingSound;
    [SerializeField] AudioClip acidSplitSound;
    [SerializeField] ZombieInvicible[] zombieInvicibleArray;

    public Vector3 latestHitDir;
    public float latestHitForce;
 
    void Awake()
    {
        slider.gameObject.SetActive(false);
        zombieSounds = GetComponent<AudioSource>();
        navMesh = GetComponent<NavMeshAgent>();
        Speed = navMesh.speed;
        angularSpeed = navMesh.angularSpeed;
        slider.maxValue = health;
    }

    void OnEnable()
    {
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

    void Start()
    {
        slider.value = health;
        AudioActivator();
        isActive = true;
        startPos = transform.position;
        healthRef = health;
        bloodSplit = GetComponentInChildren<BloodSplit>();
        isAlive = player.GetComponent<IsAlive>();
        playerIsAlive = orgPlayer.GetComponent<IsAlive>();
        enemyAnimator = GetComponentInChildren<Animator>();
        enemyAnimator.SetFloat("WalkIndex",Random.Range(0,4));
    }

    void Update()
    {
        if(isAlive.alive && playerIsAlive.alive)
        {
            Collider[] col = player.GetComponents<Collider>();
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
        
        if(isProvoked)
        {
            if(!CanMove)
            {
                StopCoroutine(AnimTimeDelay());
                enemyAnimator.SetFloat("WalkIndex",0);
                CanMove = true;
            }

            EnemyProvoked();
        }
        else if(exactDistance<enemyRange)
        {
            CanMove = false;
            isProvoked = true;
        }
        else if(!reBirth)
        {
            if(navMesh.remainingDistance <= navMesh.stoppingDistance && !navMesh.hasPath && navMesh.velocity.sqrMagnitude < 0.01f)
            { 
                enemyAnimator.SetFloat("WalkIndex",0); 
            }

            if(CanMove) { CanMove = false; StartCoroutine(AnimTimeDelay()); }
        }
    }

    IEnumerator AnimTimeDelay()
    {
        navMesh.speed = 0.3f;
        Vector3 newPosition = Random.insideUnitSphere*radius;
        newPosition += transform.position;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(newPosition,out hit,radius,NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if(navMesh.CalculatePath(hit.position,path) && path.status == NavMeshPathStatus.PathComplete)
            {
                pos = hit.position;
                Collider[] col = Physics.OverlapSphere(hit.position,newRadius,layers);

                if(col.Length == 0)
                {
                    int walkIndex = Random.Range(1,4);
                    enemyAnimator.SetFloat("WalkIndex",walkIndex);
                    navMesh.SetDestination(hit.position);
                }  
            }
        }

        int timeIntervel = Random.Range(10,21);
        yield return new WaitForSeconds(timeIntervel);
        CanMove = true;
    }

    void EnemyProvoked()
    {
        if(!dead && isAlive.alive && playerIsAlive.alive)
        {
            if(exactDistance > stopValue + StarterAssetsInputs.starterAssetsInputs.additionalDist || enemyDitected)
            {
                enemyDitected = false;

                if(bloodSplit)
                {
                    if(zombieSounds.clip != acidSplitSound)
                    {
                        zombieSounds.Stop();
                        zombieSounds.clip = acidSplitSound;
                    }
                    SpecialPower();
                }
                else
                {
                    if(zombieSounds.clip != screamingSound)
                    {
                        zombieSounds.Stop();
                        zombieSounds.clip = screamingSound;
                        zombieSounds.Play();
                    }
                    ChaseTarget();
                }
            }

            if(exactDistance <= stopValue + StarterAssetsInputs.starterAssetsInputs.additionalDist)
            {
                if(zombieSounds.clip != screamingSound)
                {
                    zombieSounds.Stop();
                    zombieSounds.clip = screamingSound;
                    zombieSounds.Play();
                }
                AttackTarget();
            }
        }
        else if(bloodSplit)
        {
            bloodSplit.StopAttack();
        }

        if(!isAlive.alive || !playerIsAlive.alive)
        {
            StopEverything();
        }
    }

    void StopEverything()
    {
        navMesh.speed = 0;
        enemyAnimator.SetBool("attack1",false);
        enemyAnimator.SetBool("attack2",false);
        enemyAnimator.SetBool("neckAttack",false);
        enemyAnimator.SetBool("PlayerDead",true);
    }

    void SpecialPower()
    {
        if(exactDistance <= 10 && exactDistance >= 4 && isAlive.alive && playerIsAlive.alive)
        {
            enemyAnimator.SetBool("running", false);
            acidAttack = true;
            navMesh.speed = 0f;
            transform.LookAt(player.transform);
            bloodSplit.transform.LookAt(player.transform);
            bloodSplit.AcidAttack(exactDistance);
        }
        else if(isAlive.alive && playerIsAlive.alive)
        {
            bloodSplit.StopAttack();
            ChaseTarget();
        }
        else
        {
            acidAttack = false;
            bloodSplit.StopAttack();
        }
    }

    void ChaseTarget()
    {
        enemyAnimator.SetBool("attack1", false);
        enemyAnimator.SetBool("attack2", false);
        enemyAnimator.SetBool("neckAttack", false);
        enemyAnimator.SetBool("running",true);
        navMesh.speed = Speed;
        navMesh.angularSpeed = angularSpeed;
        navMesh.SetDestination(player.transform.position);
    }

    void AttackTarget()
    {
        Vector3 rotation = player.transform.position - transform.position;
        rotation.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(rotation);
        transform.rotation = Quaternion.Slerp(transform.rotation,newRotation,Time.deltaTime*zombieRotationSpeed);

        enemyAnimator.SetBool("running", false);
        navMesh.speed = 0;
        navMesh.angularSpeed = 0;
        int zombieAnim = Random.Range(0,3);

        if(zombieAnim == 0)
        {
            enemyAnimator.SetBool("attack1",true);
        }
        else if(zombieAnim == 1)
        {
            enemyAnimator.SetBool("neckAttack",true);
        }
        else
        {
            enemyAnimator.SetBool("attack2",true);
        }
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
                    StopCoroutine(AnimTimeDelay());
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
                int zombieAnim = Random.Range(0,2);
                if(zombieAnim == 0)
                {
                    enemyAnimator.SetBool("crawl",true);   
                }
                else if(zombieAnim == 1)
                {
                    enemyAnimator.SetBool("walk",true);
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
        set { isAlive = value; }
    }
}
