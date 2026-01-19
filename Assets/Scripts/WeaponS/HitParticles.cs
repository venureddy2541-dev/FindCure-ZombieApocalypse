using UnityEngine;
using System.Collections.Generic;

public class HitParticles : MonoBehaviour
{
    public Transform particleHolder;

    [Header("Bullet Hit Effects")]
    public ParticleSystem flameEffect;
    public ParticleSystem sandHitEffect;
    public ParticleSystem enemyHitEffect;
    public ParticleSystem stoneHitEffect;
    public ParticleSystem woodHitEffect;
    public ParticleSystem metalHitEffect;
    public ParticleSystem electricShieldHitEffect;

    public Queue<ParticleSystem> flameParticles;
    public Queue<FireParticle> flameParticlesCs;
    public Queue<ParticleSystem> sandHitEffects;
    public Queue<ParticleSystem> enemyHitEffects;
    public Queue<ParticleSystem> metalHitEffects;
    public Queue<ParticleSystem> woodHitEffects;
    public Queue<ParticleSystem> stoneHitEffects;

    [Header("Size Of The Particles")]
    [SerializeField] int size = 3;

    [Header("Size Of The FlameThrower Particles")]
    public int flamesSize;

    void Start()
    {
        enemyHitEffects = new Queue<ParticleSystem>(size);
        metalHitEffects = new Queue<ParticleSystem>(size);
        woodHitEffects = new Queue<ParticleSystem>(size);
        sandHitEffects = new Queue<ParticleSystem>(size);
        stoneHitEffects = new Queue<ParticleSystem>(size);
        flameParticles = new Queue<ParticleSystem>(flamesSize);
        flameParticlesCs = new Queue<FireParticle>(flamesSize);

        for(int i=0;i<size;i++)
        {
            enemyHitEffects.Enqueue(Instantiate(enemyHitEffect,particleHolder.position,Quaternion.identity,particleHolder));
            metalHitEffects.Enqueue(Instantiate(metalHitEffect,particleHolder.position,Quaternion.identity,particleHolder));
            woodHitEffects.Enqueue(Instantiate(woodHitEffect,particleHolder.position,Quaternion.identity,particleHolder));
            sandHitEffects.Enqueue(Instantiate(sandHitEffect,particleHolder.position,Quaternion.identity,particleHolder));
            stoneHitEffects.Enqueue(Instantiate(stoneHitEffect,particleHolder.position,Quaternion.identity,particleHolder));
        }
        for(int i=0;i<flamesSize;i++)
        {
            ParticleSystem flameParticleRef = Instantiate(flameEffect,particleHolder.position,Quaternion.identity,particleHolder);
            flameParticles.Enqueue(flameParticleRef);
            flameParticlesCs.Enqueue(flameParticleRef.GetComponent<FireParticle>());
        }
    }
}
