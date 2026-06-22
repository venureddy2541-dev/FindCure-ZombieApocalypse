using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAudioClips", menuName = "Scriptable Object/WeaponAudioClips")]
public class WeaponAudioClipsSB : ScriptableObject
{
    public AudioClip electricShieldHitSound;
    public AudioClip woodHitSound;
    public AudioClip wallHitSound;
    public AudioClip glassHitSound;
    public AudioClip metalHitSound;
    public AudioClip sandHitSound;
    public AudioClip enemyHitSound;
    public AudioClip emptyGunSound;
    public AudioClip collectSound;
}
