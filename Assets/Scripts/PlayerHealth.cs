using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using StarterAssets;
using System.Linq;
using System;

public class PlayerHealth : MonoBehaviour
{
    public static event Action<PlayerState> PlayerDead;
    [SerializeField] GameObject playerUIComponenets;
    [SerializeField] PlayerInput playerInputSystem;
    [SerializeField] EnemyAttackTransition enemyAttackTransition;
    [SerializeField] EnemySpawner[] enemySpawners;
    public Slider slider;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text healthKitText;

    [SerializeField] Volume volume;
    Vignette vignette;

    [SerializeField] AudioSource requiredAudios;
    
    [SerializeField] GameObject playerUIComponents;
    [SerializeField] GameObject crossHair;
    [SerializeField] GameObject playerDeathText;
    [SerializeField] CinemachineCamera playerCam;
    [SerializeField] CinemachineCamera deathCam;

    [SerializeField] Car car;
    PlayerManager playerManager;
    WeaponHandle weaponHandle;
    StarterAssetsInputs starterAssetsInputs;

    [SerializeField] AudioClip playerDeadAudio;
    [SerializeField] AudioClip playerBreathAudio;
    [SerializeField] AudioClip playerGotHitSound;
    [SerializeField] AudioClip healAudio;

    public int playerHealth = 100;
    public int playerHealthRef;
    public int minHealth = 20;
    [SerializeField] float speed = 1.2f;

    [SerializeField] GameObject playerBody;
    float coolDownTime = 25f;
    float invisibleTime = 5f;
    int healthKitCount;
    [SerializeField] int helathInc = 40;
    float zombieStopDistance = 1f;
    public bool lowHealth = true;
    public bool loopControler = false;
    float tempPlayerTimer;

    [SerializeField] Image invisibilImg;
    bool isInvicible = true;
    public bool invisibleState = false;

    RectTransform crossHairRectTrans;
    Vector2 crossHairOrgPos;

    public IsAlive isAlive;

    [SerializeField] float explosionForce;
    [SerializeField] float explosionRadius;

    void Awake()
    {
        crossHairRectTrans = crossHair.GetComponent<RectTransform>();
        crossHairOrgPos = crossHairRectTrans.anchoredPosition;

        playerManager = GetComponent<PlayerManager>();
        playerDeathText = GameObject.FindWithTag("DeadMenu");
        isAlive = GetComponent<IsAlive>();

        playerHealthRef = playerHealth;
        healthText.text = playerHealthRef.ToString();
        slider.value = playerHealthRef;
    }

    void OnDisable()
    {
        PlayerDead?.Invoke(PlayerState.InActive);
    }

    void OnDestroy()
    {
        PlayerDead?.Invoke(PlayerState.Dead);
    }
    
    void Start()
    {
        UpdateHealthText(healthKitCount); 
        
        if(volume.profile.TryGet(out vignette))

        weaponHandle = GetComponent<WeaponHandle>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }
    
    void OnInvisibile(InputValue value)
    {
        if (value.isPressed && !playerManager.idle && isInvicible && !playerManager.GamePaused)
        {
            isAlive.alive = false;
            invisibleState = true;
            weaponHandle.ToTransparent();
            isInvicible = false;
            playerManager.IdleState(true);
            int playerLayer = LayerMask.NameToLayer("Player");
            int enemyLayer = LayerMask.NameToLayer("PlayerHit");
            Physics.IgnoreLayerCollision(playerLayer,enemyLayer,true);
            StartCoroutine(InvisibilityManager(false));                     
        }
    }

    IEnumerator InvisibilityManager(bool coolDownTimeLeft)
    {
        if(!coolDownTimeLeft)
        {
            tempPlayerTimer = 0f;
            loopControler = true;

            while (tempPlayerTimer <= invisibleTime)
            {
                invisibilImg.fillAmount = 1-(tempPlayerTimer/invisibleTime);
                tempPlayerTimer += Time.deltaTime;
                yield return null;
            }

            isAlive.alive = true;
            invisibleState = false;
            loopControler = false;
            weaponHandle.ToOpaque();
            playerManager.IdleState(false);
            int playerLayer = LayerMask.NameToLayer("Player");
            int enemyLayer = LayerMask.NameToLayer("PlayerHit");
            Physics.IgnoreLayerCollision(playerLayer,enemyLayer,false);
            tempPlayerTimer = coolDownTime;
        }

        while(tempPlayerTimer >= 0f)
        {
            invisibilImg.fillAmount = 1-(tempPlayerTimer/coolDownTime);
            tempPlayerTimer -= Time.deltaTime;
            yield return null;
        }
        isInvicible = true;
    }
    
    public void TakeDamage(int damage)
    {
        if(!isAlive.alive) { return; }
        if(lowHealth){ StartCoroutine("GotHit"); }
        HealthConditions(damage);
    }
    
    IEnumerator GotHit()
    {
        float gotHitVal = UnityEngine.Random.Range(0f,0.4f);
        vignette.intensity.value = gotHitVal;

        yield return new WaitForSeconds(0.1f);
        vignette.intensity.value = 0;
    }

    public void HealthConditions(int damage)
    {
        playerHealthRef -= damage;
        playerHealthRef = (playerHealthRef < 0)? 0 : playerHealthRef;
        slider.value = playerHealthRef;
        healthText.text = playerHealthRef.ToString();

        if (playerHealthRef <= minHealth && playerHealthRef > 0 && lowHealth)
        {
            HealthState(false);
        }

        if (playerHealthRef > minHealth && !lowHealth)
        {
            HealthState(true);
        }
        
        if (playerHealthRef <= 0)
        {
            Dead();
        }
    }

    void Dead()
    {
        isAlive.alive = false;

        playerManager.StopAll();

        MessageBox.messageBox.CompleteClear();
        playerUIComponents.SetActive(false);
        crossHair.SetActive(false);
        vignette.active = false;
        GameManager.gameManager.DeadMenu();
        deathCam.transform.parent = null;
        deathCam.Priority = 20;
        playerBody.transform.parent = null;
        playerBody.SetActive(true);
        
        ThrowWeapons();
        
        requiredAudios.Stop();
        requiredAudios.PlayOneShot(playerDeadAudio);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(false);
    }

    void ThrowWeapons()
    {
        foreach (GameObject gb in weaponHandle.Weapons)
        {
            gb.transform.parent = null;
            Rigidbody rb = gb.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            Animator animator = gb.GetComponent<Animator>();
            if(animator) { animator.enabled = false; }
            gb.SetActive(true);
            rb.AddExplosionForce(explosionForce,transform.position,explosionRadius,UnityEngine.Random.Range(0f,2f),ForceMode.Impulse);
        }
    }

    public void HealthState(bool currentState)
    {
        lowHealth = currentState;
        vignette.active = !currentState;

        if(!currentState)
        {
            requiredAudios.clip = playerBreathAudio;
            requiredAudios.Play();
            StartCoroutine("Blinking");
            playerManager.canZoom = false;
            playerManager.SetWeaponScrolling(!currentState);
            playerManager.SetScope(currentState); 
        }
        else
        {
            requiredAudios.clip = null;
            StopCoroutine("Blinking");
            playerManager.canZoom = true;
        }
    }

    IEnumerator Blinking()
    {
        while(true)
        {
            float lerpValRef = Mathf.Sin(Time.time*speed);
            float lerpVal = (lerpValRef + 1)/2;
            vignette.intensity.value = lerpVal;
            yield return new WaitForEndOfFrame();
        }
    }

    void OnHealth(InputValue value)
    {
        if(value.isPressed && healthKitCount > 0 && playerHealthRef < playerHealth && !playerManager.GamePaused)
        {
            requiredAudios.PlayOneShot(healAudio);
            healthKitCount--;
            healthKitText.text = "COUNT : " + healthKitCount; 
            int healthRef = playerHealthRef + helathInc;
            int health = (healthRef > playerHealth)? playerHealth - playerHealthRef : helathInc;
            HealthConditions(-health);
        }
    }

    void OnParticleCollision(GameObject gb)
    {
        TakeDamage(1);
    }

    public void UpdateHealthText(int healthKitCountRef)
    {
        /*for(int i = healthKitCountRef;i > 0;i--)
        {
            healthKitCount++;
        }*/
        healthKitCount += healthKitCountRef;
        healthKitText.text = "COUNT : "+ healthKitCount;
    }

    public int HealthKitCount
    {
        get { return healthKitCount; }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("LaserBeam"))
        {
            TakeDamage(playerHealthRef);
        }
    }

    public void ActivateDriveMode()
    {   
        HealthState(true);
        playerInputSystem.enabled = false;
        playerUIComponents.SetActive(false);
        playerCam.Priority = 0;
        if(enemySpawners.All(x => x != null)) { enemyAttackTransition.ChangingObject(enemySpawners,EnemyTarget.car,car.gameObject,zombieStopDistance); }
        gameObject.SetActive(false);
    }

    public void ActivateNormalMode()
    {
        playerManager.granadeTimeText.text = null;
        playerInputSystem.enabled = true;
        playerUIComponenets.SetActive(true);
        playerManager.ToggleShootingOrThrowing(FireStateEnum.CanFire);
        playerCam.Priority = 20;
        if (playerHealth <= minHealth)
        {
            HealthState(false);
        }
        gameObject.SetActive(true);
        StartCoroutine(InvisibilityManager(true)); 
        crossHair.gameObject.SetActive(true);
        crossHairRectTrans.anchoredPosition = crossHairOrgPos;
    }

    public void ActivateStandGunMode()
    {
        HealthState(true);
        playerInputSystem.enabled = false;
        playerUIComponents.SetActive(false);
        playerCam.Priority = 0;
        crossHair.SetActive(false);
    }

    public void AssignCar(Car car)
    {
        this.car = car;
    }
}
