using UnityEngine;

public class OilBarrel : MonoBehaviour
{
    bool isBlasted = false;
    int blastDamage = 500;
    int health = 100;
    [SerializeField] float radius;
    AudioSource blastSounds;
    [SerializeField] AudioClip blastSound;
    [SerializeField] GameObject barrel;
    [SerializeField] GameObject blastParticles;
    [SerializeField] GameObject flameParticles;
    [SerializeField] GameObject fireParticles;
    [SerializeField] LayerMask layers;
    [SerializeField] LayerMask objectsLayers;
    [SerializeField] float hitForce;

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
            Physics.Raycast(transform.position,Vector3.down,out RaycastHit hit,Mathf.Infinity,layers);
            if(hit.collider.CompareTag("SandGround"))
            {
                Instantiate(flameParticles,hit.point,Quaternion.identity);
            }
            Instantiate(blastParticles,transform.position,transform.rotation);
            GameObject newbarrel = Instantiate(barrel,transform.position,transform.rotation);
            foreach(Transform child in newbarrel.transform)
            {
                Instantiate(fireParticles,child.transform);
            }
            newbarrel.transform.localScale = transform.localScale;
            newbarrel.GetComponent<ExplodeBarrel>().Explode();
            gameObject.SetActive(false);
        }
    }

    int Damage(Vector3 pos)
    {
        int dist = Mathf.RoundToInt(Vector3.Distance(transform.position,pos));
        return (blastDamage/dist);
    }
}
