using UnityEngine;

public class OilBarrel : MonoBehaviour
{
    bool isBlasted = false;
    int blastDamage = 500;
    int health = 100;
    [SerializeField] float radius;
    AudioSource blastSounds;
    [SerializeField] AudioClip blastSound;
    [SerializeField] LayerMask objectsLayers;
    [SerializeField] float hitForce;

    public Collider thisCollider;

    void Awake()
    {
        thisCollider = GetComponent<Collider>(); 
    }

    void OnEnable()
    {
        blastSounds = GameObject.FindWithTag("BlastAudio").GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0 && !isBlasted)
        {
            bool gotDamage = true;
            isBlasted = true;
            Collider[] cols = Physics.OverlapSphere(transform.position,radius,objectsLayers);
            foreach(Collider other in cols)
            {
                if(other.CompareTag("Player"))
                {
                    PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                    if(playerHealth)
                    {
                        playerHealth.TakeDamage(Damage(other.gameObject.transform.position));
                    }
                }
                
                if(other.CompareTag("Vehical") && gotDamage)
                {
                    gotDamage = false;
                    other.GetComponent<Car>().CarDamage(Damage(other.gameObject.transform.position));
                }

                if(other.CompareTag("EnemySpawner"))
                {
                    other.GetComponent<EnemySpawner>().DamageTaker(Damage(other.gameObject.transform.position));
                }

                if (other.CompareTag("EnemyMain"))
                {
                    other.GetComponent<Enemy>().TakeDamage(Damage(other.gameObject.transform.position),-(other.transform.position - transform.position),hitForce);
                }
                
                if (other.CompareTag("Wood"))
                {
                    Crate crate = other.GetComponent<Crate>();
                    if (crate)
                    {
                        crate.TakeDamage(Damage(other.gameObject.transform.position));
                    }
                }
            }

            blastSounds.PlayOneShot(blastSound);

            thisCollider.enabled = false;

            ExplodeBarrel eb = RequiredParticles.instance.GetBarralEffect();
            eb.transform.localScale = transform.localScale;
            eb.transform.position = transform.position;
            eb.transform.rotation = transform.rotation;
            eb.Explode();

            Destroy(gameObject);
        }
    }

    int Damage(Vector3 pos)
    {
        int dist = Mathf.RoundToInt(Vector3.Distance(transform.position,pos));
        return (blastDamage/dist);
    }
}
