using UnityEngine;

[CreateAssetMenu(fileName = "WeaponHitEffects", menuName = "Scriptable Object/WeaponHitEffects")]
public class WeaponHitEffectsSB : ScriptableObject
{
    public ParticleSystem flameEffect;
    public ParticleSystem sandHitEffect;
    public ParticleSystem enemyHitEffect;
    public ParticleSystem stoneHitEffect;
    public ParticleSystem woodHitEffect;
    public ParticleSystem metalHitEffect;
    public ParticleSystem electricShieldHitEffect;
    public ParticleSystem bombRobotBlastParticle;
    public ParticleSystem spiderRobotBlastParticle;
    public ParticleSystem spawnerBlastParticle;
    public GameObject barralEffect;
    public GameObject createEffect;
}
