using UnityEngine;

public class DamageManager : MonoBehaviour
{
    //public EnemyTarget initialEnemyTarget;
    [SerializeField] int enemyHitDamage = 25;
    public EnemyTarget enemyTarget;
    AudioSource audioSource;
    [SerializeField] AudioClip playerHitSound;
    [SerializeField] AudioClip otherHitSound;

    PlayerHealth playerHealth;
    StandGunShield standGunShield;
    StandGun standGun;
    Car car;

    void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();

        standGunShield = FindFirstObjectByType<StandGunShield>();
        standGun = FindFirstObjectByType<StandGun>();
        car = FindFirstObjectByType<Car>();
    }

    public void AnimtionEvent()
    {
        switch(enemyTarget) 
        {
            case  EnemyTarget.player :
                
                playerHealth = FindFirstObjectByType<PlayerHealth>();
                if(playerHealth)
                {
                    HitAudios(playerHitSound);
                    playerHealth.TakeDamage(enemyHitDamage);
                }
                break;

            case EnemyTarget.gun :

                if(standGun)
                {
                    HitAudios(otherHitSound);
                    standGun.GunDamage(enemyHitDamage);
                }
                break;

            case EnemyTarget.car :

                if(car)
                {
                    HitAudios(otherHitSound);
                    car.CarDamage(enemyHitDamage);
                }
                break;

            case EnemyTarget.gunShield :

                if(standGunShield)
                {
                    HitAudios(otherHitSound);
                    standGunShield.GunShieldDamage(enemyHitDamage);
                }
                break;
        }
    }

    void HitAudios(AudioClip ac)
    {
        audioSource.PlayOneShot(ac);
    }
}
