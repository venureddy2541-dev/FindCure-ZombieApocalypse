using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    Enemy enemy;
    [SerializeField] Collider[] colliders;
    [SerializeField] Rigidbody[] rb;
    [SerializeField] AudioSource enemyAudios;
    [SerializeField] AudioClip enemyHitByVehicalSound;
    bool isAlive = true;
    [SerializeField] float hitForce;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void Chase()
    {
        enemy.ChasePlayer();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Vehical") && isAlive  && other.gameObject.GetComponentInParent<Car>().speedRef >= 1.5f)
        {
            enemy.TakeDamage(enemy.healthRef,Vector3.zero,hitForce);
            other.GetComponent<Car>().CarDamage(10);
            isAlive = false;
            enemyAudios.PlayOneShot(enemyHitByVehicalSound);
        }
    }

    public void RegdolActivation()
    {
        enemy.navMesh.speed = 0;
        enemy.navMesh.angularSpeed = 0;
        gameObject.GetComponent<Animator>().enabled = false;
        foreach(Rigidbody newRb in rb)
        {
            newRb.isKinematic = false;
            newRb.AddForce(-enemy.latestHitDir*enemy.latestHitForce,ForceMode.Impulse);
        }
        
        enemy.dead = true;
        Invoke("EnemyDead",2f);
    }

    void EnemyDead()
    {
        enemy.EnemyDeactiveState();
    }

    public void ResetEverything()
    {
        if(enemy.reBirth)
        {
            isAlive = true;

            /*foreach(Collider col in colliders)
            {
                col.enabled = false;
            }

            foreach(Rigidbody newRb in rb)
            {
                newRb.linearVelocity = Vector3.zero;
                newRb.angularVelocity = Vector3.zero;
            }

            foreach(Collider col in colliders)
            {
                col.enabled = true;
            }*/

            gameObject.GetComponent<Animator>().enabled = true;
            foreach(Rigidbody newRb in rb)
            {
                newRb.isKinematic = true;
            }
            transform.localRotation = Quaternion.identity;
        }
    }
}
