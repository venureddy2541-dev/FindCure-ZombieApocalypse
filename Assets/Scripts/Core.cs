using UnityEngine;
using System.Collections.Generic;

public class CoreScript : MonoBehaviour
{
    EnemySpawner enemySpawner;

    [SerializeField] ParticleSystem standGunPS;
    int standGunbulletDamage = 20;
    [SerializeField] ParticleSystem metalHitEffect;

    void OnParticleCollision(GameObject other)
    {
        enemySpawner = GetComponentInParent<EnemySpawner>();
        if(enemySpawner != null)
        {
            if(other.CompareTag("StandGun"))
            {
                List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
                int collisionEventsCount = standGunPS.GetCollisionEvents(gameObject,collisionEvents);
                for(int i = 0;i<collisionEventsCount;i++)
                {
                    Vector3 contactPoint = collisionEvents[i].intersection;
                    Vector3 normal = collisionEvents[i].normal;
                    Instantiate(metalHitEffect,contactPoint,Quaternion.LookRotation(normal));
                }
                enemySpawner.DamageTaker(standGunbulletDamage);
            }
        }
    }
}
