using UnityEngine;
using System.Collections.Generic;

public class FlameThrowerBullets : MonoBehaviour
{
    [SerializeField] ParticleSystem flames;
    ParticleSystem flameThrower;
    int damage = 10;
    [SerializeField] float hitForce;

    void Start()
    {
        flameThrower = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject gb)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int count = flameThrower.GetCollisionEvents(gb,collisionEvents);
        for(int i = 0;i<count;i++)
        {
            Instantiate(flames,collisionEvents[i].intersection,Quaternion.LookRotation(collisionEvents[i].normal));
        }

        if(gb.CompareTag("Robot")) 
        { 
            RoboBomb roboBomb = gb.GetComponentInParent<RoboBomb>(); 
            if(roboBomb) { roboBomb.TakeDamage(damage); }
            Robot robot = gb.GetComponentInParent<Robot>();
            if(robot) { robot.TakeDamage(damage); }
        }

        if(gb.CompareTag("WalkingRobots"))
        {
            gb.GetComponent<WalkingRobots>().TakeDamage(damage);
        }

        if(gb.CompareTag("Enemy"))
        {
            gb.GetComponentInParent<Enemy>().TakeDamage(damage,-collisionEvents[0].normal,hitForce);
        }

        if(gb.CompareTag("EnemySpawner"))
        {   
            gb.GetComponent<EnemySpawner>().DamageTaker(damage);
        }

        if(gb.CompareTag("Wood"))
        {
            Crate crate = gb.GetComponent<Crate>();
            if(crate)
            {
                crate.TakeDamage(damage);
            }
        }

        if(gb.CompareTag("Metal"))
        {   
            OilBarrel oilBarrel = gb.GetComponent<OilBarrel>();
            if(oilBarrel) { oilBarrel.TakeDamage(damage); }
        }
    }
}
