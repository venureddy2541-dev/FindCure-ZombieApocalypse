using UnityEngine;
using System.Collections.Generic;

public class FlameThrowerBullets : MonoBehaviour
{
    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    [SerializeField] WeaponType weaponType;
    HitParticles particles;
    [SerializeField] ParticleSystem flames;
    ParticleSystem flameThrower;
    [SerializeField] WeaponData weaponData;
    int damage;
    [SerializeField] float hitForce;

    void Start()
    {
        particles = weaponType.particles;
        damage = weaponData.weaponDamage;
        flameThrower = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject gb)
    {
        if(gb.CompareTag("Robot"))
        { 
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            ParticleSystem current = particles.flameParticles.Dequeue();
            particles.flameParticles.Enqueue(current);

            FireParticle currentFireParticle = particles.flameParticlesCs.Dequeue();
            particles.flameParticlesCs.Enqueue(currentFireParticle);

            current.gameObject.SetActive(true);
            currentFireParticle.SetFollowPos(gb.transform);
            current.Play();

            RoboBomb roboBomb = gb.GetComponentInParent<RoboBomb>(); 
            if(roboBomb) { roboBomb.TakeDamage(count*damage); }
            Robot robot = gb.GetComponentInParent<Robot>();
            if(robot) { robot.TakeDamage(count*damage); }
        }

        if(gb.CompareTag("WalkingRobots"))
        {
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            ParticleSystem current = particles.flameParticles.Dequeue();
            particles.flameParticles.Enqueue(current);

            FireParticle currentFireParticle = particles.flameParticlesCs.Dequeue();
            particles.flameParticlesCs.Enqueue(currentFireParticle);

            current.gameObject.SetActive(true);
            currentFireParticle.SetFollowPos(gb.transform);
            current.Play();

            gb.GetComponent<WalkingRobots>().TakeDamage(count*damage);
        }

        if(gb.CompareTag("Enemy"))
        {
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            ParticleSystem current = particles.flameParticles.Dequeue();
            particles.flameParticles.Enqueue(current);

            FireParticle currentFireParticle = particles.flameParticlesCs.Dequeue();
            particles.flameParticlesCs.Enqueue(currentFireParticle);

            current.gameObject.SetActive(true);
            currentFireParticle.SetFollowPos(gb.transform);
            current.Play();

            gb.GetComponentInParent<Enemy>().TakeDamage(count*damage,-collisionEvents[0].normal,hitForce);
        }

        if(gb.CompareTag("EnemySpawner"))
        {   
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            ParticleSystem current = particles.flameParticles.Dequeue();
            particles.flameParticles.Enqueue(current);

            FireParticle currentFireParticle = particles.flameParticlesCs.Dequeue();
            particles.flameParticlesCs.Enqueue(currentFireParticle);

            current.gameObject.SetActive(true);
            currentFireParticle.SetFollowPos(collisionEvents[0].intersection);
            current.Play();

            gb.GetComponent<EnemySpawner>().DamageTaker(count*damage);
        }

        if(gb.CompareTag("Wood"))
        {
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            ParticleSystem current = particles.flameParticles.Dequeue();
            particles.flameParticles.Enqueue(current);

            FireParticle currentFireParticle = particles.flameParticlesCs.Dequeue();
            particles.flameParticlesCs.Enqueue(currentFireParticle);

            current.gameObject.SetActive(true);
            currentFireParticle.SetFollowPos(collisionEvents[0].intersection);
            current.Play();

            Crate crate = gb.GetComponent<Crate>();
            if(crate)
            {
                crate.TakeDamage(count*damage);
            }
        }

        if(gb.CompareTag("Metal"))
        {   
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            ParticleSystem current = particles.flameParticles.Dequeue();
            particles.flameParticles.Enqueue(current);

            FireParticle currentFireParticle = particles.flameParticlesCs.Dequeue();
            particles.flameParticlesCs.Enqueue(currentFireParticle);

            current.gameObject.SetActive(true);
            currentFireParticle.SetFollowPos(collisionEvents[0].intersection);
            current.Play();

            OilBarrel oilBarrel = gb.GetComponent<OilBarrel>();
            if(oilBarrel) { oilBarrel.TakeDamage(count*damage); }
        }

        if(gb.CompareTag("SandGround") || gb.CompareTag("TileGround"))
        {
            int count = flameThrower.GetCollisionEvents(gb,collisionEvents);

            ParticleSystem current = particles.flameParticles.Dequeue();
            particles.flameParticles.Enqueue(current);

            FireParticle currentFireParticle = particles.flameParticlesCs.Dequeue();
            particles.flameParticlesCs.Enqueue(currentFireParticle);

            current.gameObject.SetActive(true);
            currentFireParticle.SetFollowPos(collisionEvents[0].intersection);
            current.Play();
        }
    }
}
