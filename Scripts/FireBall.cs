using UnityEngine;

public class FireBall : MonoBehaviour
{
    AudioSource blastSounds;
    [SerializeField] Transform parent;
    [SerializeField] ParticleSystem blastParticle;
    Rigidbody rb;
    float fireBallDamage = 5000f;
    float radius = 10f;
    [SerializeField] float hitForce;

    void Awake()
    {
        blastSounds = GameObject.FindWithTag("BlastAudio").GetComponent<AudioSource>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision other)
    {
        blastSounds.Play();
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position,radius);

        foreach(Collider coll in collidersInRange)
        {
            if(coll.CompareTag("Metal"))
            {
                OilBarrel oilBarrel = coll.GetComponent<OilBarrel>();
                if(oilBarrel)
                {
                    oilBarrel.TakeDamage(Mathf.RoundToInt(fireBallDamage));
                }
            }

            if(coll.CompareTag("EnemyMain"))
            {
                coll.GetComponent<Enemy>().TakeDamage(Mathf.RoundToInt(fireBallDamage),-(coll.transform.position - transform.position),hitForce);
            }

            if(coll.CompareTag("EnemySpawner"))
            {
                coll.GetComponent<EnemySpawner>().DamageTaker(Mathf.RoundToInt(fireBallDamage));
            }
        }

        blastParticle.gameObject.transform.localScale = transform.localScale;
        Instantiate(blastParticle,transform.position,Quaternion.identity);
        rb.isKinematic = true;
        transform.position = parent.position;
        transform.rotation = parent.rotation;
        gameObject.SetActive(false);
    }   

    public void FireBallDamage(float damage)
    {
        fireBallDamage = damage;
    }
}
