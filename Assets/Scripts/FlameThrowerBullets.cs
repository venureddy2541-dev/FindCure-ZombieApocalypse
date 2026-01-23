using UnityEngine;
using System.Collections.Generic;

public class FlameThrowerBullets : MonoBehaviour
{
    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    [SerializeField] WeaponType weaponType;
    [SerializeField] ParticleSystem flames;
    ParticleSystem flameThrower;
    [SerializeField] WeaponData weaponData;
    int damage;
    [SerializeField] float hitForce;

    void Start()
    {
        damage = weaponData.weaponDamage;
        flameThrower = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject gb)
    {
        if(gb.CompareTag("Robot"))
        { 
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            SetParticlePos(gb.transform);

            RoboBomb roboBomb = gb.GetComponentInParent<RoboBomb>(); 
            if(roboBomb) { roboBomb.TakeDamage(count*damage); }
            Robot robot = gb.GetComponentInParent<Robot>();
            if(robot) { robot.TakeDamage(count*damage); }
        }

        if(gb.CompareTag("WalkingRobots"))
        {
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            SetParticlePos(gb.transform);

            gb.GetComponent<WalkingRobots>().TakeDamage(count*damage);
        }

        if(gb.CompareTag("Enemy"))
        {
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            SetParticlePos(gb.transform);

            gb.GetComponentInParent<Enemy>().TakeDamage(count*damage,-collisionEvents[0].normal,hitForce);
        }

        if(gb.CompareTag("EnemySpawner"))
        {   
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            SetParticlePos(collisionEvents[0].intersection);

            gb.GetComponent<EnemySpawner>().DamageTaker(count*damage);
        }

        if(gb.CompareTag("Wood"))
        {
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            SetParticlePos(collisionEvents[0].intersection);

            Crate crate = gb.GetComponent<Crate>();
            if(crate)
            {
                crate.TakeDamage(count*damage);
            }
        }

        if(gb.CompareTag("Metal"))
        {   
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            SetParticlePos(collisionEvents[0].intersection);

            OilBarrel oilBarrel = gb.GetComponent<OilBarrel>();
            if(oilBarrel) { oilBarrel.TakeDamage(count*damage); }
        }

        if(gb.CompareTag("SandGround") || gb.CompareTag("TileGround"))
        {
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            SetParticlePos(collisionEvents[0].intersection);
        }
    }

    void SetParticlePos(Transform hitObjectTransform)
    {
        FireParticle currentFireParticle = RequiredParticles.instance.GetFlameHitEffectCs();

        currentFireParticle.SetFollowPos(hitObjectTransform);
        currentFireParticle.Play();
    }

    void SetParticlePos(Vector3 hitObjectPosition)
    {
        FireParticle currentFireParticle = RequiredParticles.instance.GetFlameHitEffectCs();

        currentFireParticle.SetFollowPos(hitObjectPosition);
        currentFireParticle.Play();
    }
}
