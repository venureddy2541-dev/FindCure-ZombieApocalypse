using UnityEngine;
using System.Collections;

public class BloodSplit : MonoBehaviour
{
    Enemy enemy;
    PlayerHealth playerHealth;
    [SerializeField] Transform[] acidBloodBags;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem acidBlood;
    [SerializeField] AudioSource audioSource;

    float value = 0f;
    bool isSpliting = true;
    bool isAttacking = true;

    void Start()
    {
        acidBlood = GetComponent<ParticleSystem>();
        enemy = GetComponentInParent<Enemy>(); 
        playerHealth = FindFirstObjectByType<PlayerHealth>();      
    }

    public void AcidAttack(float dist)
    {
        acidBlood.startSpeed = dist;
        if(!isSpliting) return;
        StartCoroutine(AcidSplitTime());
    }

    IEnumerator AcidSplitTime()
    {
        isSpliting = false;
        while(!isSpliting && isAttacking)
        {
            animator.SetBool("AcidAttack",true);
            acidBlood.Play();
            audioSource.Play();
            while(value < 3f)
            {
                value += Time.deltaTime;
                float particleCount = (1-(value/3f));
                var emission = acidBlood.emission;
                emission.rateOverTime = particleCount*200;
                ValueLerping(value);
                yield return null;
                if(isSpliting) break;
            }
            animator.SetBool("AcidAttack",false);
            
            isAttacking = false;
            if(value >= 3f)
            {
                audioSource.Stop();
                acidBlood.Stop();
                while(value > 0f)
                {
                    value -= Time.deltaTime;
                    ValueLerping(value);
                    yield return null;
                }
            } 
            isAttacking = true;
            yield return null;
        }
    }

    void ValueLerping(float value)
    {
        float acidValue = (0.5f - (value/6f));
        foreach (Transform item in acidBloodBags)
        {
            item.localScale = new Vector3(acidValue,acidValue,acidValue);
        }
    }

    public void StopAttack()
    {
        isSpliting = true;
        acidBlood.Stop();
        animator.SetBool("AcidAttack",false);
    }

    void OnParticleCollision(GameObject gb)
    {
        if(gb.CompareTag("Player"))
        {
            gb.GetComponent<PlayerHealth>().TakeDamage(1);
        }
        
        if(gb.CompareTag("Vehical"))
        {
            gb.GetComponent<Car>().CarDamage(1);
        }
        
        if(gb.CompareTag("StandGunShield"))
        {
            gb.GetComponent<StandGunShield>().GunShieldDamage(1);
        }

        if(gb.CompareTag("StandGun"))
        {
            gb.GetComponent<StandGun>().GunDamage(1);
        }
    }
}
