using UnityEngine;

public class DamageManager : MonoBehaviour
{
    [SerializeField] int enemyHitDamage = 25;
    public EnemyTarget enemyTarget;
    AudioSource audioSource;
    [SerializeField] AudioClip playerHitSound;
    [SerializeField] AudioClip otherHitSound;

    PlayerHealth playerHealth;
    StandGunShield standGunShield;
    StandGun standGun;
    Car car;
    string target;

    void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void AnimtionEvent()
    {
        switch(target) 
        {
            case  "Player" :
                
                HitAudios(playerHitSound);
                playerHealth.TakeDamage(enemyHitDamage);
                break;

            case "StandGun" :

                HitAudios(otherHitSound);
                standGun.GunDamage(enemyHitDamage);
                break;

            case "Car" :

                HitAudios(otherHitSound);
                car.CarDamage(enemyHitDamage);
                break;

            case "Shield" :

                HitAudios(otherHitSound);
                standGunShield.GunShieldDamage(enemyHitDamage);
                break;
        }
    }

    public void ChangeTarget(GameObject currentTarget)
    {
        target = currentTarget.name;
        if(target == "Player")
        {
            playerHealth = currentTarget.GetComponent<PlayerHealth>();
        }
        else if(target == "StandGun")
        {
            standGun = currentTarget.GetComponent<StandGun>();
        }
        else if(target == "Shield")
        {
            standGunShield = currentTarget.GetComponent<StandGunShield>();
        }
        else if(target == "Car")
        {
            car = currentTarget.GetComponent<Car>();
        }
    }

    void HitAudios(AudioClip ac)
    {
        audioSource.PlayOneShot(ac);
    }
}
