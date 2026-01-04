using UnityEngine;

[CreateAssetMenu(fileName = "HitEffects", menuName = "Scriptable Object/HitEffects")]
public class HitEffects : ScriptableObject
{
    public AudioClip electricShieldSound;
    public AudioClip metalSound;
    public AudioClip woodSound;
    public AudioClip enemySound;
    public AudioClip groundSound;

    public ParticleSystem electricShieldEffect;
    public ParticleSystem metalEffect;
    public ParticleSystem enemyEffect;
    public ParticleSystem woodEffect;
    public ParticleSystem groundEffect;
}
