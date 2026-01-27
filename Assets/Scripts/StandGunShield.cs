using UnityEngine;
using UnityEngine.UI;

public class StandGunShield : MonoBehaviour
{
    [SerializeField] Slider shiledSlider;
    [SerializeField] GameObject Gun;
    [SerializeField] int gunShieldHealth = 15000;
    IsAlive isAlive;
    int gunShieldHealthRef;
    float zombieStopDistance;

    void Awake()
    {
        isAlive = GetComponent<IsAlive>();
        gunShieldHealthRef = gunShieldHealth;
        shiledSlider.value = gunShieldHealthRef;
    }

    public void GunShieldDamage(int damage)
    {
        gunShieldHealthRef -= damage;
        shiledSlider.value = gunShieldHealthRef;
        if (gunShieldHealthRef <= 0)
        {
            Gun.GetComponent<StandGun>().ChangeEnemyTarget(Gun);
            DeadState();
        }
    }

    public void DeadState()
    {
        gunShieldHealthRef = 0;
        isAlive.alive = false;
        gameObject.SetActive(false);
    }
}
