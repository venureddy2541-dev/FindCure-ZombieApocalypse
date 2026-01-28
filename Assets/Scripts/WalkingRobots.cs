using UnityEngine;
using UnityEngine.AI;

public class WalkingRobots : MonoBehaviour
{
    [SerializeField] int points;
    [SerializeField] LayerMask layers;
    [SerializeField] GameObject obstacleCheckar;
    public GameObject player;
    public AudioSource blastAudioSource;
    [SerializeField] GameObject blastParticles;
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

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        health = healthRef;
        anim = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();
        playerHealth = player.GetComponent<PlayerHealth>();  
        isAlive = player.GetComponent<IsAlive>();
        Speed = navMesh.speed;
    }

    void Update()
    {
        if(!isAlive.alive || playerHealth.invisibleState) 
        { 
            var emission = gun.emission; 
            emission.enabled = false; 
            anim.SetBool("Attack",true); 
            navMesh.speed = 0; 
            return;
        }

        float distance = Vector3.Distance(transform.position,player.transform.position);

        if(distance > navMesh.stoppingDistance)
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
        anim.SetBool("Attack",false);
        var emission = gun.emission;
        emission.enabled = false;
        navMesh.SetDestination(player.transform.position); 
    }

    void Attack()
    {
        transform.LookAt(player.transform.position);
        navMesh.speed = 0;
        anim.SetBool("Attack",true);
        var emission = gun.emission;
        emission.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            GameManager.gameManager.UpdateCash(points);
            blastAudioSource.Play();
            
            ParticleSystem currentEffect = RequiredParticles.instance.GetspiderRobotBlastParticle();
            currentEffect.transform.position = transform.position;
            currentEffect.transform.rotation = Quaternion.identity;
            currentEffect.Play();

            gameObject.SetActive(false);
            transform.position = startPosition;
            transform.rotation = startRotation;
            health = healthRef;
        }
    }

    public int Health
    {
        set { health = value; }
    }
}
