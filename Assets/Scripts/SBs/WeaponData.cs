using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData" , menuName = "Scriptable Object/WeaponData")]
public class WeaponData : ScriptableObject
{
    public float fireRate;
    public float reloadTime;
    public int weaponZoom;
    public int magSize;
    public int maxAmmo;
    public int weaponDamage;
    public float bulletHitForce;
    public float range;
    public float bulletSpread;
    public float onScopeBulletSpread;
    public float xRecoil;
    public float yRecoil;
    public AudioClip reloadSound;
    public AudioClip weaponSound;
    public AudioClip emptyGunSound;
}
