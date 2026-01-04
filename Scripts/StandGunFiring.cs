using UnityEngine;

public class StandGunFiring : MonoBehaviour
{

    Enemy enemy;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    
    void OnParticleCollision(GameObject gb)
    {
        if(gb.CompareTag("StandGun"))
        {
            //enemy.TakeDamage(standGunbulletDamage);
        }
    }
}
