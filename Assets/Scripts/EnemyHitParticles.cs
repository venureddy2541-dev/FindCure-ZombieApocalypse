using UnityEngine;
using System.Collections.Generic;

public class EnemyHitParticles : MonoBehaviour
{
    [SerializeField] Transform particleHolder;
    [SerializeField] HitEffects hitEffectsObject;
    [SerializeField] AudioSource weaponAudioSource;

    int standGunbulletDamage = 50;

    ParticleSystem standGunPs;

    public float volume;

    [SerializeField] float hitForce;

    [SerializeField] List<ParticleSystem> enemyHitParticles = new List<ParticleSystem>();
    [SerializeField] List<ParticleSystem> metalHitParticles = new List<ParticleSystem>();  
    [SerializeField] List<ParticleSystem> woodHitParticles = new List<ParticleSystem>();  
    [SerializeField] List<ParticleSystem> electricHitParticles = new List<ParticleSystem>(); 
    int enemyHitParticlesIndex = 0;
    int metalHitParticlesIndex = 0;
    int woodHitParticlesIndex = 0;
    int electricHitParticlesIndex = 0;

    void Start()
    {
        standGunPs = GetComponent<ParticleSystem>();
        for(int i=0;i<10;i++)
        {
            enemyHitParticles.Add(Instantiate(hitEffectsObject.enemyEffect,transform.position,Quaternion.identity,particleHolder));
            metalHitParticles.Add(Instantiate(hitEffectsObject.metalEffect,transform.position,Quaternion.identity,particleHolder));
            woodHitParticles.Add(Instantiate(hitEffectsObject.woodEffect,transform.position,Quaternion.identity,particleHolder));
            electricHitParticles.Add(Instantiate(hitEffectsObject.electricShieldEffect,transform.position,Quaternion.identity,particleHolder));
        }
    }

    void OnParticleCollision(GameObject gb)
    {
        if (gb.CompareTag("Enemy"))
        {
            HitAudio(hitEffectsObject.enemySound);
            if(enemyHitParticlesIndex >= enemyHitParticles.Count) enemyHitParticlesIndex = 0;
            HitEffect(gb,enemyHitParticles[enemyHitParticlesIndex]);

            Enemy enemy = gb.GetComponentInParent<Enemy>();
            if(enemy)
            {
                enemy.TakeDamage(standGunbulletDamage,-(gb.transform.position - transform.position),hitForce);
            }
            enemyHitParticlesIndex++;
        }
        /*else if(gb.CompareTag("TileGround"))
        {
            HitAudio(hitEffectsObject.groundSound);
            if(groundHitParticlesIndex >= groundHitParticles.Count) groundHitParticlesIndex = 0;
            HitEffect(gb,metalHitParticles[groundHitParticlesIndex]);
            groundHitParticlesIndex++;
        }*/
        else if(gb.CompareTag("Metal"))
        {
            HitAudio(hitEffectsObject.metalSound);
            if(metalHitParticlesIndex >= metalHitParticles.Count) metalHitParticlesIndex = 0;
            HitEffect(gb,metalHitParticles[metalHitParticlesIndex]);
            metalHitParticlesIndex++;
        }
        else if(gb.CompareTag("Wood"))
        {
            HitAudio(hitEffectsObject.woodSound);
            if(woodHitParticlesIndex >= woodHitParticles.Count) woodHitParticlesIndex = 0;
            HitEffect(gb,woodHitParticles[woodHitParticlesIndex]);
            woodHitParticlesIndex++;
        }
        else if(gb.CompareTag("EnemySpawner"))
        {   
            HitAudio(hitEffectsObject.metalSound);
            if(metalHitParticlesIndex >= enemyHitParticles.Count) metalHitParticlesIndex = 0;
            HitEffect(gb,metalHitParticles[metalHitParticlesIndex]);
            metalHitParticlesIndex++;

            gb.GetComponent<EnemySpawner>().DamageTaker(standGunbulletDamage);
        }
        else if(gb.CompareTag("ElectricShield"))
        {
            HitAudio(hitEffectsObject.electricShieldSound);
            if(electricHitParticlesIndex >= electricHitParticles.Count) electricHitParticlesIndex = 0;
            HitEffect(gb,electricHitParticles[electricHitParticlesIndex]);
            electricHitParticlesIndex++;
        }
    }

    void HitAudio(AudioClip ac)
    {
        weaponAudioSource.PlayOneShot(ac, volume);
    }

    void HitEffect(GameObject gb , ParticleSystem ps)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int collisionEventsCount = standGunPs.GetCollisionEvents(gb,collisionEvents);
        for(int i = 0;i<collisionEventsCount;i++)
        {
            Vector3 contactPoint = collisionEvents[i].intersection;
            Vector3 normal = collisionEvents[i].normal;
            ps.gameObject.transform.position = contactPoint;
            ps.gameObject.transform.LookAt(normal);
            ps.Play();
        }
    }
}
