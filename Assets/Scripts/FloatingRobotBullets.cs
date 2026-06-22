using UnityEngine;

public class FloatingRobotBullets : MonoBehaviour
{
    [SerializeField] int damage;
    PlayerHealth playerHealth;
    
    void OnParticleCollision(GameObject gb)
    {
        if(gb.CompareTag("Player"))
        {
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            else
            {
                playerHealth = gb.GetComponent<PlayerHealth>();
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
