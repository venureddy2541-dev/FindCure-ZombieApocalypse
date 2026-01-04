using UnityEngine;

public class DamageManager : MonoBehaviour
{
    [SerializeField] int enemyHitDamage = 25;
    public string aventName;
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
        aventName = "player";

        standGunShield = FindFirstObjectByType<StandGunShield>();
        standGun = FindFirstObjectByType<StandGun>();
        car = FindFirstObjectByType<Car>();
    }

    public void AnimtionEvent()
    {
        switch(aventName) 
        {
            case "player" :
                
                playerHealth = FindFirstObjectByType<PlayerHealth>();
                if(playerHealth)
                {
                    HitAudios(playerHitSound);
                    playerHealth.TakeDamage(enemyHitDamage);
                }
                break;

            case "gun" :

                if(standGun)
                {
                    HitAudios(otherHitSound);
                    standGun.GunDamage(enemyHitDamage);
                }
                break;

            case "car" :

                if(car)
                {
                    HitAudios(otherHitSound);
                    car.CarDamage(enemyHitDamage);
                }
                break;

            case "gunShield" :

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
