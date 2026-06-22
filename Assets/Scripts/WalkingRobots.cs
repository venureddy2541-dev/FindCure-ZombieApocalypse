using UnityEngine;
using UnityEngine.AI;

public class WalkingRobots : MonoBehaviour
{
    public bool reBirth;
    [SerializeField] int points;
    [SerializeField] LayerMask layers;
    [SerializeField] GameObject obstacleCheckar;
    public GameObject player;
    public AudioSource blastAudioSource;
    [SerializeField] ParticleSystem gun;
    
    Animator anim;
    NavMeshAgent navMesh;
    int health;
    [SerializeField] int healthRef = 100; 
    Vector3 startPosition;
    Quaternion startRotation;
    PlayerHealth playerHealth;
    IsAlive isAlive;
    float Speed;

    public FinalStage manager;

    void Start()
    {
        if(player)
        {
            playerHealth = player.GetComponent<PlayerHealth>();  
            isAlive = player.GetComponent<IsAlive>();
        }

        startPosition = transform.position;
        startRotation = transform.rotation;
        health = healthRef;
        
        anim = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();
        Speed = navMesh.speed;
    }

    void Update()
    {
        if(!isAlive.alive || playerHealth.invisibleState) 
        { 
            anim.SetBool("TargetDead",true);
            var emission = gun.emission; 
            emission.enabled = false; 
            anim.SetBool("Attack",true); 
            navMesh.speed = 0; 
            return;
        }

        float distance = Vector3.Distance(transform.position,player.transform.position);

        if(distance >= navMesh.stoppingDistance)
        {
            Chase();
        }
        else
        {
            obstacleCheckar.transform.LookAt(player.transform);
            RaycastHit hit;
            Physics.Raycast(obstacleCheckar.transform.position,obstacleCheckar.transform.forward,out hit,distance,layers);

            if(hit.collider){ Chase(); }
            else{ Attack(); }
        }
    }

    void Chase()
    {
        navMesh.speed = Speed;
        anim.SetBool("TargetDead",false);
        anim.SetBool("Attack",false);
        anim.SetBool("Chase",true);
        var emission = gun.emission;
        emission.enabled = false;
        navMesh.SetDestination(player.transform.position); 
    }

    void Attack()
    {
        transform.LookAt(player.transform.position);
        navMesh.speed = 0;
        anim.SetBool("TargetDead",false);
        anim.SetBool("Chase",false);
        anim.SetBool("Attack",true);
        var emission = gun.emission;
        emission.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            if(!reBirth) { if(manager != null) { manager.RobotsDeadCount(); } }

            GameManager.gameManager.UpdateCash(points);
            blastAudioSource.Play();
            
            ParticleSystem currentEffect = RequiredParticles.instance.GetspiderRobotBlastParticle();
            currentEffect.transform.position = transform.position;
            currentEffect.transform.rotation = Quaternion.identity;
            currentEffect.Play();

            gameObject.SetActive(false);

            if(reBirth)
            {
                transform.position = startPosition;
                transform.rotation = startRotation;
                health = healthRef;
            }
        }
    }

    public void MasterDead(FinalStage manager)
    {
        reBirth = false;
        this.manager = manager;
    }

    public int Health
    {
        set { health = value; }
    }
}
