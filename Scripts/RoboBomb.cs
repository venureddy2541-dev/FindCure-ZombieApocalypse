using UnityEngine;
using UnityEngine.AI;

public class RoboBomb : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] int points;
    public GameObject player;
    public AudioSource blastAudioSource;

    NavMeshAgent navMesh;
    Vector3 startPosition;
    Quaternion startRotation;

    [SerializeField] ParticleSystem blastParticleSystem;

    [SerializeField] float radius = 8f;

    [SerializeField] float rotateSpeed = 10f;
    float chaseDist = 2f;
    [SerializeField] int health = 100;
    int healthRef;
    [SerializeField] int damage = 10;
    Animator anim;
    float Speed;
    PlayerHealth playerHealth;
    IsAlive isAlive;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if(player) 
        {  
            playerHealth = player.GetComponent<PlayerHealth>();
            isAlive = player.GetComponent<IsAlive>();
        }

        healthRef = health;
        startPosition = transform.position;
        startRotation = transform.rotation;
        anim = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();
        Speed = navMesh.speed;
    }

    void Update()
    {
        if(!isAlive.alive || playerHealth.invisibleState)
        {
            navMesh.speed = 0;
            return;
        }

        float dist = Vector3.Distance(transform.position,player.transform.position);
        if(dist > chaseDist)
        {
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            navMesh.speed = Speed;
            navMesh.SetDestination(player.transform.position);
            Vector3 rotatePos = (player.transform.position - transform.position);
            Quaternion finalRotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(rotatePos),Time.deltaTime*rotateSpeed);
            transform.rotation = finalRotation;
        }
        else
        {
            audioSource.Stop();
            Blast();
        }
    }

    void Blast()
    {
        Collider[] collsInRange = Physics.OverlapSphere(transform.position,radius);
        foreach(Collider col in collsInRange)
        {
            if(col.CompareTag("Player"))
            {
                PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();

                if(playerHealth) playerHealth.TakeDamage(damage);
            }
        }

        blastAudioSource.Play();
        Instantiate(blastParticleSystem,transform.position + new Vector3(0,1f,0),Quaternion.identity);
        gameObject.SetActive(false);
        transform.position = startPosition;
        transform.rotation = startRotation;
        health = healthRef;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            GameManager.gameManager.UpdateCash(points);
            Blast();
        }
    }

    public int Health
    {
        set { health = value; }
    }
}
