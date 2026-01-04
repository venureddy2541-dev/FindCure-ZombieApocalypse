using UnityEngine;

public class FloatingRobotBullets : MonoBehaviour
{
    [SerializeField] int damage;
    
    void OnParticleCollision(GameObject gb)
    {
        if(gb.CompareTag("Player"))
        {
            gb.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
