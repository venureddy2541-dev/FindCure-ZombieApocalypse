using UnityEngine;
using System.Collections.Generic;

public class EnemyHitParticles : MonoBehaviour
{
    [SerializeField] WeaponAudioClipsSB weaponAudioClipsSB;
    [SerializeField] AudioSource weaponAudioSource;

    int standGunbulletDamage = 50;
    ParticleSystem standGunPs;
    public float volume;
    [SerializeField] float hitForce;
    [SerializeField] List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void Start()
    {
        standGunPs = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject gb)
    {
        if (gb.CompareTag("Enemy"))
        {
            int collisionEventsCount = standGunPs.GetCollisionEvents(gb,collisionEvents);

            HitAudio(weaponAudioClipsSB.enemyHitSound);

            HitEffect(RequiredParticles.instance.GetEnemyHitEffect());

            Enemy enemy = gb.GetComponentInParent<Enemy>();
            if(enemy)
            {
                enemy.TakeDamage(standGunbulletDamage*collisionEventsCount,-(gb.transform.position - transform.position),hitForce);
            }
        }
        else if(gb.CompareTag("Metal"))
        {
            int collisionEventsCount = standGunPs.GetCollisionEvents(gb,collisionEvents);

            HitAudio(weaponAudioClipsSB.metalHitSound);

            HitEffect(RequiredParticles.instance.GetMetalHitEffect());
        }
        else if(gb.CompareTag("EnemySpawner"))
        {   
            int collisionEventsCount = standGunPs.GetCollisionEvents(gb,collisionEvents);

            HitAudio(weaponAudioClipsSB.metalHitSound);

            HitEffect(RequiredParticles.instance.GetMetalHitEffect());

            gb.GetComponent<EnemySpawner>().DamageTaker(standGunbulletDamage*collisionEventsCount);
        }
        else if(gb.CompareTag("ElectricShield"))
        {
            int collisionEventsCount = standGunPs.GetCollisionEvents(gb,collisionEvents);

            HitAudio(weaponAudioClipsSB.electricShieldHitSound);

            HitEffect(RequiredParticles.instance.GetElectricShieldHitEffect());
        }
    }

    void HitAudio(AudioClip ac)
    {
        weaponAudioSource.PlayOneShot(ac, volume);
    }

    void HitEffect(ParticleSystem currentEffect)
    {
        currentEffect.transform.position = collisionEvents[0].intersection;
        currentEffect.transform.LookAt(collisionEvents[0].normal);
        currentEffect.Play();
    }
}
