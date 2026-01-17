using UnityEngine;
using System.Collections.Generic;

public class HitParticles : MonoBehaviour
{
    public Transform particleHolder;
    public ParticleSystem flameEffect;
    public ParticleSystem sandHitEffect;
    public ParticleSystem enemyHitEffect;
    public ParticleSystem stoneHitEffect;
    public ParticleSystem hitGlassEffect;
    public ParticleSystem woodHitEffect;
    public ParticleSystem metalHitEffect;
    public ParticleSystem electricShieldHitEffect;
    public int flameEffectsIndex = 0;
    public int enemyHitEffectsIndex = 0;
    public int sandHitEffectsIndex = 0;
    public int woodHitEffectsIndex = 0;
    public int metalHitEffectsIndex = 0;
    public Queue<ParticleSystem> flameParticles;
    public Queue<FireParticle> flameParticlesCs;
    public ParticleSystem[] sandHitEffects;
    public ParticleSystem[] enemyHitEffects;
    public ParticleSystem[] metalHitEffects;
    public ParticleSystem[] woodHitEffects;
    [SerializeField] int size = 3;
    public int flamesSize;

    void Start()
    {
        enemyHitEffects = new ParticleSystem[size];
        metalHitEffects = new ParticleSystem[size];
        woodHitEffects = new ParticleSystem[size];
        sandHitEffects = new ParticleSystem[size];
        flameParticles = new Queue<ParticleSystem>(flamesSize);
        flameParticlesCs = new Queue<FireParticle>(flamesSize);

        for(int i=0;i<size;i++)
        {
            enemyHitEffects[i] = Instantiate(enemyHitEffect,particleHolder.position,Quaternion.identity,particleHolder);
            metalHitEffects[i] = Instantiate(metalHitEffect,particleHolder.position,Quaternion.identity,particleHolder);
            woodHitEffects[i] = Instantiate(woodHitEffect,particleHolder.position,Quaternion.identity,particleHolder);
            sandHitEffects[i] = Instantiate(sandHitEffect,particleHolder.position,Quaternion.identity,particleHolder);
        }
        for(int i=0;i<flamesSize;i++)
        {
            ParticleSystem flameParticleRef = Instantiate(flameEffect,particleHolder.position,Quaternion.identity,particleHolder);
            flameParticles.Enqueue(flameParticleRef);
            flameParticlesCs.Enqueue(flameParticleRef.GetComponent<FireParticle>());
        }
    }
}
