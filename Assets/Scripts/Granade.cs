using UnityEngine;

public class Granade : MonoBehaviour
{
    AudioSource blastAudioSource;
    [SerializeField] ParticleSystem blastParticles;
    PlayerFiring player;
    int granadeRadius = 10;
    int damage = 500;
    public int timeLeft;
    [SerializeField] float hitForce;

    void Awake()
    {
        blastAudioSource = GameObject.FindWithTag("BlastAudio").GetComponent<AudioSource>();
    }

    void Start()
    {
        timeLeft += 1;
        Invoke("GranadeBlast",timeLeft);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,granadeRadius);
    }

    void GranadeBlast()
    {
        Instantiate(blastParticles,transform.position,Quaternion.identity);
        blastAudioSource.Play();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position,granadeRadius);
        foreach(Collider other in hitColliders)
        {
            if(other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if(playerHealth)
                {
                    playerHealth.TakeDamage(Damage(other.gameObject.transform.position));
                }
            }

            if(other.CompareTag("EnemyMain"))
            {
                other.GetComponent<Enemy>().TakeDamage(Damage(other.gameObject.transform.position),-(other.transform.position - transform.position),hitForce);
            }

            if(other.CompareTag("Wood"))
            {
                Crate crate = other.GetComponent<Crate>();
                if(crate)
                {
                    crate.TakeDamage(Damage(other.gameObject.transform.position));
                }
            }

            if(other.CompareTag("Metal"))
            {
                OilBarrel oilBarrel = other.GetComponent<OilBarrel>();
                if(oilBarrel)
                {
                    oilBarrel.TakeDamage(Damage(other.gameObject.transform.position));
                }
            }

            if(other.CompareTag("EnemySpawner"))
            {   
                other.GetComponent<EnemySpawner>().DamageTaker(damage);
            }

            if(other.CompareTag("Robot")) 
            { 
                RoboBomb roboBomb = other.GetComponentInParent<RoboBomb>(); 
                if(roboBomb) { roboBomb.TakeDamage(damage); }
                Robot robot = other.GetComponentInParent<Robot>();
                if(robot) { robot.TakeDamage(damage); }
            }

            if(other.CompareTag("WalkingRobots"))
            {
                other.GetComponent<WalkingRobots>().TakeDamage(damage);
            }

        }
        Destroy(gameObject);
    }

    int Damage(Vector3 pos)
    {
        int dist = Mathf.RoundToInt(Vector3.Distance(transform.position,pos));
        dist = (dist == 0)? 1: dist;
        return (damage/dist*1);
    }

    public void GetObject(PlayerFiring playerScript)
    {
        player = playerScript;
    }
}
