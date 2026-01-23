using UnityEngine;
using System.Collections.Generic;

public class RequiredParticles : MonoBehaviour
{
    public static RequiredParticles instance;

    [SerializeField] WeaponHitEffectsSB weaponHitEffectsSB;

    public Queue<ParticleSystem> sandHitEffects;
    public Queue<ParticleSystem> enemyHitEffects;
    public Queue<ParticleSystem> metalHitEffects;
    public Queue<ParticleSystem> woodHitEffects;
    public Queue<ParticleSystem> stoneHitEffects;
    public Queue<ParticleSystem> electricShieldHitEffects;

    public Queue<ParticleSystem> bombRobotBlastParticles;
    public Queue<ParticleSystem> spiderRoboBlastParticles;
    public Queue<ParticleSystem> spawnersBlastParticles;

    public Queue<ExplodeBarrel> barralEffectsCs;

    public Queue<ExplodeCreate> createEffectsCs;

    public Queue<FireParticle> flameParticlesCs;

    [Header("HitEffects Size")]
    [SerializeField] int size = 3;

    [Header("FlameThrowerParticles Size")]
    public int flamesSize;

    [Header("RobotsBlastParticles Size")]
    public int robotBlastParticlesSize = 10;

    [Header("SpawnersBlastParticles Size")]
    public int spawnersBlastParticlesSize = 2;

    [Header("barralEffectsCs Size")]
    public int barralEffectsCsSize = 3;

    [Header("createEffectsCs Size")]
    public int createEffectsCsSize = 5;

    void Awake()
    {
        instance = this;
    
        enemyHitEffects = new Queue<ParticleSystem>(size);
        metalHitEffects = new Queue<ParticleSystem>(size);
        woodHitEffects = new Queue<ParticleSystem>(size);
        sandHitEffects = new Queue<ParticleSystem>(size);
        stoneHitEffects = new Queue<ParticleSystem>(size);
        electricShieldHitEffects = new Queue<ParticleSystem>(size);
        
        flameParticlesCs = new Queue<FireParticle>(flamesSize);

        bombRobotBlastParticles = new Queue<ParticleSystem>(robotBlastParticlesSize);
        spiderRoboBlastParticles = new Queue<ParticleSystem>(robotBlastParticlesSize);
        spawnersBlastParticles = new Queue<ParticleSystem>(spawnersBlastParticlesSize);

        barralEffectsCs = new Queue<ExplodeBarrel>(barralEffectsCsSize);
        createEffectsCs = new Queue<ExplodeCreate>(createEffectsCsSize);

        for(int i=0;i<size;i++)
        {
            ParticleSystem ps;

            ps = Instantiate(weaponHitEffectsSB.enemyHitEffect,transform);
            enemyHitEffects.Enqueue(ps);
            ps.gameObject.SetActive(false);

            ps = Instantiate(weaponHitEffectsSB.metalHitEffect,transform);
            metalHitEffects.Enqueue(ps);
            ps.gameObject.SetActive(false);

            ps = Instantiate(weaponHitEffectsSB.woodHitEffect,transform);
            woodHitEffects.Enqueue(ps);
            ps.gameObject.SetActive(false);

            ps = Instantiate(weaponHitEffectsSB.sandHitEffect,transform);
            sandHitEffects.Enqueue(ps);
            ps.gameObject.SetActive(false);

            ps = Instantiate(weaponHitEffectsSB.stoneHitEffect,transform);
            stoneHitEffects.Enqueue(ps);
            ps.gameObject.SetActive(false);

            ps = Instantiate(weaponHitEffectsSB.electricShieldHitEffect,transform);
            electricShieldHitEffects.Enqueue(ps);
            ps.gameObject.SetActive(false);

        }

        for(int i=0;i<flamesSize;i++)
        {
            FireParticle fp = Instantiate(weaponHitEffectsSB.flameEffect,transform).GetComponent<FireParticle>();
            flameParticlesCs.Enqueue(fp);
            fp.gameObject.SetActive(false);
        }

        for(int i=0;i<robotBlastParticlesSize;i++)
        {
            ParticleSystem ps;

            ps = Instantiate(weaponHitEffectsSB.bombRobotBlastParticle,transform);
            bombRobotBlastParticles.Enqueue(ps);
            ps.gameObject.SetActive(false);

            ps = Instantiate(weaponHitEffectsSB.spiderRobotBlastParticle,transform);
            spiderRoboBlastParticles.Enqueue(ps);
            ps.gameObject.SetActive(false);
        }

        for(int i=0;i<spawnersBlastParticlesSize;i++)
        {
            ParticleSystem ps = Instantiate(weaponHitEffectsSB.spawnerBlastParticle,transform);
            spawnersBlastParticles.Enqueue(ps);
            ps.gameObject.SetActive(false);
        }

        for(int i=0;i<barralEffectsCsSize;i++)
        {
            ExplodeBarrel eb = Instantiate(weaponHitEffectsSB.barralEffect,transform).GetComponent<ExplodeBarrel>();
            barralEffectsCs.Enqueue(eb);
            eb.gameObject.SetActive(false);
        }

        for(int i=0;i<createEffectsCsSize;i++)
        {
            ExplodeCreate ec = Instantiate(weaponHitEffectsSB.createEffect,transform).GetComponent<ExplodeCreate>();
            createEffectsCs.Enqueue(ec);
            ec.gameObject.SetActive(false);
        }
    }

    public ParticleSystem GetEnemyHitEffect()
    {
        ParticleSystem currentEffect = enemyHitEffects.Dequeue();
        enemyHitEffects.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public ParticleSystem GetMetalHitEffect()
    {
        ParticleSystem currentEffect = metalHitEffects.Dequeue();
        metalHitEffects.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public ParticleSystem GetWoodHitEffect()
    {
        ParticleSystem currentEffect = woodHitEffects.Dequeue();
        woodHitEffects.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public ParticleSystem GetSandHitEffect()
    {
        ParticleSystem currentEffect = sandHitEffects.Dequeue();
        sandHitEffects.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public ParticleSystem GetStoneHitEffect()
    {
        ParticleSystem currentEffect = stoneHitEffects.Dequeue();
        stoneHitEffects.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public ParticleSystem GetElectricShieldHitEffect()
    {
        ParticleSystem currentEffect = electricShieldHitEffects.Dequeue();
        electricShieldHitEffects.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public FireParticle GetFlameHitEffectCs()
    {
        FireParticle currentEffectCs = flameParticlesCs.Dequeue();
        flameParticlesCs.Enqueue(currentEffectCs);

        currentEffectCs.gameObject.SetActive(true);

        return currentEffectCs;
    }

    public ParticleSystem GetRoboBombBlastParticle()
    {
        ParticleSystem currentEffect = bombRobotBlastParticles.Dequeue();
        bombRobotBlastParticles.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public ParticleSystem GetspiderRobotBlastParticle()
    {
        ParticleSystem currentEffect = spiderRoboBlastParticles.Dequeue();
        spiderRoboBlastParticles.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public ParticleSystem GetspawnerBlastParticle()
    {
        ParticleSystem currentEffect = spawnersBlastParticles.Dequeue();
        spawnersBlastParticles.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public ExplodeBarrel GetBarralEffect()
    {
        ExplodeBarrel currentEffect = barralEffectsCs.Dequeue();
        barralEffectsCs.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }

    public ExplodeCreate GetCreateEffect()
    {
        ExplodeCreate currentEffect = createEffectsCs.Dequeue();
        createEffectsCs.Enqueue(currentEffect);

        currentEffect.gameObject.SetActive(true);

        return currentEffect;
    }
}
