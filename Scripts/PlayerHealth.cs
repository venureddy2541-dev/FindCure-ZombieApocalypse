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

public class PlayerHealth : MonoBehaviour
{
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
    PlayerFiring playerFiring;
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
    float weaponSplitUpSpeed;
    float weaponSplitFrontSpeed;
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
    MessageBox messageBox;

    RectTransform crossHairRectTrans;
    Vector2 crossHairOrgPos;

    public IsAlive isAlive;

    [SerializeField] float explosionForce;
    [SerializeField] float explosionRadius;

    void Awake()
    {
        crossHairRectTrans = crossHair.GetComponent<RectTransform>();
        crossHairOrgPos = crossHairRectTrans.anchoredPosition;

        playerFiring = GetComponent<PlayerFiring>();
        playerDeathText = GameObject.FindWithTag("DeadMenu");
        messageBox = GameObject.FindWithTag("MessageBox").GetComponent<MessageBox>();
        isAlive = GetComponent<IsAlive>();

        playerHealthRef = playerHealth;
        healthText.text = playerHealthRef.ToString();
        slider.value = playerHealthRef;
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
        if (value.isPressed && isInvicible && !playerFiring.GamePaused)
        {
            isAlive.alive = false;
            invisibleState = true;
            weaponHandle.ToTransparent();
            isInvicible = false;
            playerFiring.idle = false;
            int playerLayer = LayerMask.NameToLayer("Player");
            int enemyLayer = LayerMask.NameToLayer("PlayerHit");
            Physics.IgnoreLayerCollision(playerLayer,enemyLayer,true);
            PlayerInvicibleState(null);
            StartCoroutine(InvisibilityManager(false));                         
        }
    }

    public void PlayerInvicibleState(GameObject currentObject)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies)
        {   
            if(enemy.isProvoked && !enemy.dead)
            {
                if(enemy.navMesh.speed != 0f || enemy.acidAttack)
                {
                    EnemyIdleState(enemy);
                }
                else
                {
                    enemy.enemyAnimator.SetBool("PlayerDead",false);
                    enemy.navMesh.speed = enemy.Speed;
                }
            }
            enemy.player = currentObject;
        }
    }

    void EnemyIdleState(Enemy enemy)
    {
        enemy.navMesh.speed = 0;
        enemy.enemyAnimator.SetBool("attack1",false);
        enemy.enemyAnimator.SetBool("attack2",false);
        enemy.enemyAnimator.SetBool("neckAttack",false);
        enemy.enemyAnimator.SetBool("PlayerDead",true);
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
            if(playerFiring.cantShoot == true)
            {
                playerFiring.idle = true;
            }
            int playerLayer = LayerMask.NameToLayer("Player");
            int enemyLayer = LayerMask.NameToLayer("PlayerHit");
            Physics.IgnoreLayerCollision(playerLayer,enemyLayer,false);
            PlayerInvicibleState(this.gameObject);
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
        float gotHitVal = Random.Range(0f,0.4f);
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
            playerFiring.ZoomOut();
            LowHealth();
        }

        if (playerHealthRef > minHealth && !lowHealth)
        {
            GreaterThenMinHealth();
        }
        
        if (playerHealthRef <= 0)
        {
            isAlive.alive = false;
            messageBox.CompleteClear();
            playerFiring.ZoomOut();
            playerUIComponents.SetActive(false);
            crossHair.SetActive(false);
            vignette.active = false;
            requiredAudios.Stop();
            requiredAudios.PlayOneShot(playerDeadAudio);
            GameManager.gameManager.DeadMenu();
            playerFiring.canZoom = false;
            deathCam.transform.parent = null;
            deathCam.Priority = 20;
            playerBody.transform.parent = null;
            playerBody.SetActive(true);
            foreach (GameObject gb in weaponHandle.Weapons)
            {
                gb.transform.parent = null;
                Rigidbody rb = gb.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                Animator animator = gb.GetComponent<Animator>();
                if(animator) { animator.enabled = false; }
                gb.SetActive(true);
                float weaponSplitUpSpeed = Random.Range(0f, 5f);
                float weaponSplitFrontSpeed = Random.Range(0f, 3f);
                float weaponSplitLRSpeed = Random.Range(0, 2f);
                rb.AddExplosionForce(explosionForce,transform.position,explosionRadius,Random.Range(0f,2f),ForceMode.Impulse);
            }
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gameObject.SetActive(false);
        }
    }

    public void LowHealth()
    {
        lowHealth = false;
        playerFiring.canZoom = false;
        vignette.active = true;
        requiredAudios.clip = playerBreathAudio;
        requiredAudios.Play();
        StartCoroutine("Blinking");
    }

    public void GreaterThenMinHealth()
    {
        requiredAudios.clip = null;
        StopCoroutine("Blinking");
        playerFiring.canZoom = true;
        lowHealth = true;
        vignette.active = false;
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
        if(value.isPressed && healthKitCount > 0 && playerHealthRef < playerHealth && !playerFiring.GamePaused)
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
        for(int i = healthKitCountRef;i > 0;i--)
        {
            healthKitCount++;
        }
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
        GreaterThenMinHealth();
        playerInputSystem.enabled = false;
        playerUIComponents.SetActive(false);
        playerCam.Priority = 0;
        if(enemySpawners.All(x => x != null)) { enemyAttackTransition.ChangingObject(enemySpawners,"car",car.gameObject,zombieStopDistance); }
        gameObject.SetActive(false);
    }

    public void ActivateNormalMode()
    {
        playerFiring.granadeTimeText.text = null;
        playerInputSystem.enabled = true;
        playerUIComponenets.SetActive(true);
        playerFiring.ActivateShootingOrThrowing();
        playerCam.Priority = 20;
        if (playerHealth <= minHealth)
        {
            LowHealth();
        }
        gameObject.SetActive(true);
        StartCoroutine(InvisibilityManager(true)); 
        crossHair.gameObject.SetActive(true);
        crossHairRectTrans.anchoredPosition = crossHairOrgPos;
    }

    public void ActivateStandGunMode()
    {
        GreaterThenMinHealth();
        playerInputSystem.enabled = false;
        playerUIComponents.SetActive(false);
        playerCam.Priority = 0;
        crossHair.SetActive(false);
    }

    public void AssignCar(Car car)
    {
        car = car;
    }
}
