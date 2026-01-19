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
    List<ParticleCollisionEvent> collisionEvents;

    [Header("StandGun Hit Particles")]
    [SerializeField] Queue<ParticleSystem> enemyHitParticles;
    [SerializeField] Queue<ParticleSystem> metalHitParticles;  
    [SerializeField] Queue<ParticleSystem> woodHitParticles;  
    [SerializeField] Queue<ParticleSystem> electricHitParticles; 

    [Header("Size Of The Particles")]
    [SerializeField] int size;

    void Start()
    {
        standGunPs = GetComponent<ParticleSystem>();
        for(int i=0;i<size;i++)
        {
            enemyHitParticles.Enqueue(Instantiate(hitEffectsObject.enemyEffect,transform.position,Quaternion.identity,particleHolder));
            metalHitParticles.Enqueue(Instantiate(hitEffectsObject.metalEffect,transform.position,Quaternion.identity,particleHolder));
            woodHitParticles.Enqueue(Instantiate(hitEffectsObject.woodEffect,transform.position,Quaternion.identity,particleHolder));
            electricHitParticles.Enqueue(Instantiate(hitEffectsObject.electricShieldEffect,transform.position,Quaternion.identity,particleHolder));
        }
    }

    void OnParticleCollision(GameObject gb)
    {
        if (gb.CompareTag("Enemy"))
        {
            int collisionEventsCount = standGunPs.GetCollisionEvents(gb,collisionEvents);

            HitAudio(hitEffectsObject.enemySound);

            HitEffect(enemyHitParticles);

            Enemy enemy = gb.GetComponentInParent<Enemy>();
            if(enemy)
            {
                enemy.TakeDamage(standGunbulletDamage*collisionEventsCount,-(gb.transform.position - transform.position),hitForce);
            }
        }
        else if(gb.CompareTag("Metal"))
        {
            HitAudio(hitEffectsObject.metalSound);

            HitEffect(metalHitParticles);
        }
        else if(gb.CompareTag("Wood"))
        {
            HitAudio(hitEffectsObject.woodSound);

            HitEffect(woodHitParticles);
        }
        else if(gb.CompareTag("EnemySpawner"))
        {   
            int collisionEventsCount = standGunPs.GetCollisionEvents(gb,collisionEvents);

            HitAudio(hitEffectsObject.metalSound);

            HitEffect(metalHitParticles);

            gb.GetComponent<EnemySpawner>().DamageTaker(standGunbulletDamage*collisionEventsCount);
        }
        else if(gb.CompareTag("ElectricShield"))
        {
            HitAudio(hitEffectsObject.electricShieldSound);

            HitEffect(electricHitParticles);
        }
    }

    void HitAudio(AudioClip ac)
    {
        weaponAudioSource.PlayOneShot(ac, volume);
    }

    void HitEffect(Queue<ParticleSystem> currentEffectQueue)
    {
        ParticleSystem currentEffect = currentEffectQueue.Dequeue();
        currentEffectQueue.Enqueue(currentEffect);
        currentEffect.transform.position = collisionEvents[0].intersection;
        currentEffect.transform.LookAt(collisionEvents[0].normal);
        currentEffect.Play();
    }
}
