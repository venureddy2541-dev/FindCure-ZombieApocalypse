using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Robot : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] int points;
    [SerializeField] LayerMask layers;
    [SerializeField] GameObject obstacleCheckar;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerLookPos;
    [SerializeField] ParticleSystem[] bullets;
    AudioSource audioSource;
    [SerializeField] AudioClip firingSound;
    [SerializeField] AudioClip movingSound;
    NavMeshAgent navMesh;

    AudioSource blastSound;

    [SerializeField] float rotateSpeed = 10f;
    float chaseDist = 20f;
    [SerializeField] int health = 1000;
    float Speed;
    Animator anim;
    PlayerHealth playerHealth;
    IsAlive isAlive;

    void Awake()
    {
        slider.maxValue = health;
        audioSource = GetComponent<AudioSource>();
        blastSound = GameObject.FindWithTag("BlastAudio").GetComponent<AudioSource>();
    } 

    void Start()
    {
        slider.value = health;
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
            ChaseSound();
            anim.SetBool("idle",true);
            foreach (ParticleSystem ps in bullets)
            {
                var emission = ps.emission; 
                emission.enabled = false;
            }
            return;
        }

        float dist = Vector3.Distance(transform.position,player.transform.position);

        if(dist >= chaseDist) 
        { 
            Chase(); 
        }
        else 
        { 
            obstacleCheckar.transform.LookAt(player.transform);
            RaycastHit hit;
            Physics.Raycast(obstacleCheckar.transform.position,obstacleCheckar.transform.forward,out hit,dist,layers);

            if(hit.collider){ Chase(); } else{ Attack(); }
        }
    }

    void Chase()
    {
        ChaseSound();
        navMesh.speed = Speed;
        anim.SetBool("idle",true);
        foreach (ParticleSystem ps in bullets)
        {
            var emission = ps.emission; 
            emission.enabled = false;
        }
        navMesh.SetDestination(player.transform.position);
    }

    void Attack()
    {
        if(audioSource.clip != firingSound)
        {
            audioSource.minDistance = 1;
            audioSource.Stop();
            audioSource.clip = firingSound;
            audioSource.Play();
        }
        Vector3 rotatePos = (player.transform.position - transform.position);
        Quaternion finalRotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(rotatePos),Time.deltaTime*rotateSpeed);
        transform.rotation = finalRotation;
        navMesh.speed = 0;
        anim.SetBool("idle",false);
        foreach (ParticleSystem ps in bullets)
        {
            ps.transform.rotation = finalRotation;
            var emission = ps.emission; 
            emission.enabled = true;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        slider.value = health;

        if(health <= 0)
        {
            slider.gameObject.SetActive(false);
            GameManager.gameManager.UpdateCash(points);
            blastSound.Play();
            GetComponentInParent<FinalStage>().NormalRobotsDeadCount();

            ParticleSystem currentEffect = RequiredParticles.instance.GetspawnerBlastParticle();
            currentEffect.transform.position = transform.position + new Vector3(0,1f,0);
            currentEffect.transform.rotation = transform.rotation;
            currentEffect.Play();

            Destroy(gameObject);
        }
    }

    void ChaseSound()
    {
        if(audioSource.clip != movingSound)
        {
            audioSource.minDistance = 0.03f;
            audioSource.Stop();
            audioSource.clip = movingSound;
            audioSource.Play();
        }
    }

    public int Health
    {
        set { health = value; }
    }
}
