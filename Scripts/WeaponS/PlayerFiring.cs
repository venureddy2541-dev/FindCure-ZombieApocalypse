using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.UI;

public class PlayerFiring : MonoBehaviour
{
    [SerializeField] Transform particleHolder;
    public GameObject playerWeaponPos;
    bool gamePaused = false;
    [SerializeField] Weapon pW;

    MessageBox messageBox;

    [Header("TimeBomb")]
    [SerializeField] GameObject timeBombObject;
    public bool timeBomb = false;
    public int timeBombCount;
    [SerializeField] AudioSource blastAudioSource;
    [SerializeField] GameObject blastParticle;
    [SerializeField] TMP_Text timeBombText;
    public List<GameObject> bombPlantAreas = new List<GameObject>();

    [Header("Granade Values")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletPos;
    [SerializeField] AudioClip granadeThrowSound;
    [SerializeField] TMP_Text granadeCountText;
    public TMP_Text granadeTimeText;
    Vector3 direction;
    public bool granadeSelected = false;
    bool canThrough = true;
    bool toguleWG = false;
    [SerializeField] float speed = 100f;

    [Header("Cinemachine Components")]
    [SerializeField] CinemachineImpulseSource weaponShake;
    [SerializeField] CinemachineCamera playerCamera;
    public List<Animator> gunAnimater;

    [Header("WeaponHitEffects")]
    public List<ParticleSystem> mazelFlash;
    [SerializeField] ParticleSystem electricShieldHitEffect;
    [SerializeField] ParticleSystem enemyHitEffect;
    [SerializeField] ParticleSystem[] enemyHitEffects;
    int enemyHitEffectsIndex = 0;
    [SerializeField] ParticleSystem enemyHitEffect1;
    [SerializeField] ParticleSystem sandHitEffect;
    [SerializeField] ParticleSystem[] sandHitEffects;
    int sandHitEffectsIndex = 0;
    [SerializeField] ParticleSystem stoneHitEffect;
    [SerializeField] ParticleSystem hitGlassEffect;
    [SerializeField] ParticleSystem woodHitEffect;
    [SerializeField] ParticleSystem[] woodHitEffects;
    int woodHitEffectsIndex = 0;
    [SerializeField] ParticleSystem metalHitEffect;
    [SerializeField] ParticleSystem[] metalHitEffects;
    int metalHitEffectsIndex = 0;
    [SerializeField] ParticleSystem shootGunMetalHitEffect;

    [SerializeField] LayerMask playerHitLayers;
    public GameObject flashLight;
    [SerializeField] GameObject scopeZoomImage1;
    [SerializeField] GameObject scopeZoomImage2;
    public GameObject crossHair;
    [SerializeField] GameObject weaponTexts;

    [Header("WeaponSounds")]
    [SerializeField] AudioSource gunAudioSource;
    [SerializeField] AudioClip electricShieldSound;
    [SerializeField] AudioClip woodHitSound;
    [SerializeField] AudioClip wallHitSound;
    [SerializeField] AudioClip glassHitSound;
    [SerializeField] AudioClip metalHitSound;
    [SerializeField] AudioClip sandHitSound;
    [SerializeField] AudioClip enemyHitSound;
    [SerializeField] AudioClip emptyGunSound;
    [SerializeField] AudioClip collectSound;

    [Header("AmmoTexts")]
    public TMP_Text magText;

    public List<int> storageSize = new List<int>();
    public List<int> magSize = new List<int>();

    int damage;
    float time;
    int granadeCount;
    int tempMag;
    int temp;
    int count = 0;
    int onZoomVal = 1;
    int originalZooom = 40;

    bool isEmpty = false;

    //flashLight On and Off
    bool isOn = false;
    bool granadeThrown = false;
    bool running = false;
    public bool reloading = true;

    //prevents pause click MultipleTimes
    public bool didClick = true;

    //Tougles between weapons and granade
    public bool weaponState = true;
    private bool paused = false;
    public bool reloaded = false;
    public bool fired;
    public bool idle = false;
    public bool willZoom = false;
    public bool shootRate = true;
    public bool canZoom = true;
    public bool cantShoot = false;

    float reloadTime;
    [SerializeField] float volume;
    TimeBomb timeBombRef;
    
    void Awake()
    {
        messageBox = GameObject.FindWithTag("MessageBox").GetComponent<MessageBox>();
    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        enemyHitEffects = new ParticleSystem[5];
        metalHitEffects = new ParticleSystem[5];
        woodHitEffects = new ParticleSystem[5];
        sandHitEffects = new ParticleSystem[5];

        for(int i=0;i<5;i++)
        {
            enemyHitEffects[i] = Instantiate(enemyHitEffect,particleHolder.position,Quaternion.identity,particleHolder);
            metalHitEffects[i] = Instantiate(metalHitEffect,particleHolder.position,Quaternion.identity,particleHolder);
            woodHitEffects[i] = Instantiate(woodHitEffect,particleHolder.position,Quaternion.identity,particleHolder);
            sandHitEffects[i] = Instantiate(sandHitEffect,particleHolder.position,Quaternion.identity,particleHolder);
        }

        timeBombRef = timeBombObject.GetComponent<TimeBomb>();
        timeBombRef.blastAudioSource = blastAudioSource;
        timeBombRef.blastParticle = blastParticle;
        timeBombRef.timerText = timeBombText;
        UpdateGranadeText(granadeCount);
        scopeZoomImage1.SetActive(false);
        scopeZoomImage2.SetActive(false);
        tempMag = magSize[0];
        magText.text = magSize[0].ToString() + "/" + storageSize[0].ToString();
    }

    void Update()
    {
        FlashLight();
        WeaponReload();
    }

    void FlashLight()
    {
        if (Input.GetKeyDown(KeyCode.G) && flashLight != null)
        {
            isOn = !isOn;
            flashLight.SetActive(isOn);
        }
    }

    void OnZoom(InputValue other)
    {
        willZoom = other.isPressed;
        if (willZoom && canZoom && count != 2 && count != 4)
        {
            ZoomIn();
        }
        else
        {
            ZoomOut();
        }
    }

    protected virtual void ZoomIn()
    {
        crossHair.SetActive(false);
        if (count == 1)
        {
            scopeZoomImage1.SetActive(true);
        }
        else if(count == 3)
        {
            scopeZoomImage2.SetActive(true);
        } 
        onZoomVal = 40;
        float zoomDistance = pW.weaponZoom;
        playerCamera.Lens.FieldOfView = zoomDistance;
    }

    protected virtual void ZoomOut()
    {
        crossHair.SetActive(true);
        if (count == 1)
        {
            scopeZoomImage1.SetActive(false);
        }
        else if (count == 3)
        {
            scopeZoomImage2.SetActive(false);
        }
        onZoomVal = 1;
        playerCamera.Lens.FieldOfView = originalZooom;
    }

    void OnGranade(InputValue value)
    {
        if (value.isPressed && toguleWG)
        {
            if(weaponState && fired) { return; }
            granadeSelected = !granadeSelected;
            idle = !granadeSelected;
            canZoom = !granadeSelected;
            weaponState = !granadeSelected;
            cantShoot = !granadeSelected;
        }
    }

    void GranadeExicuted()
    {
        granadeCount--;
        granadeCountText.text = granadeCount.ToString();
        Shoot();
        gunAudioSource.PlayOneShot(granadeThrowSound);
        GameObject gb = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Granade granadeSc = gb.GetComponent<Granade>();
        granadeSc.timeLeft = Mathf.RoundToInt(time);
        granadeSc.GetObject(this);
        Rigidbody rb = gb.GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Impulse);
    }

    IEnumerator GranadeTimer()
    {
        running = true;
        while (time >= 0)
        {
            granadeTimeText.text = time.ToString("F1");
            time -= Time.deltaTime;
            yield return null;
            if(!granadeSelected) { granadeThrown = false; running = false; granadeTimeText.text = null; yield break; }
        }
        if (fired)
        {
            granadeThrown = false;
            GranadeExicuted();
        }
        granadeTimeText.text = null;
        running = false;
    }

    void OnFiring(InputValue other)
    {
        fired = other.isPressed;
        if (idle && magSize[count] > 0)
        {
            if (fired && !reloaded && count != 1 && count != 4 && shootRate)
            {
                OnFire();
            }
            else if (count == 1 || count == 4)
            {
                if (!reloaded)
                {
                    StartCoroutine("Firing");
                }
            }
        }

        if (!weaponState && toguleWG)
        {
            if (fired && granadeCount > 0)
            {
                time = 5f;
                granadeThrown = true;
                if (running) return;
                StartCoroutine(GranadeTimer());
            }

            if (!fired && granadeThrown)
            {
                granadeThrown = false;
                GranadeExicuted();
            }
        }
    }

    IEnumerator Firing()
    {
        while (fired)
        {
            OnFire();
            yield return new WaitForSeconds(pW.fireRate);
            shootRate = true;
        }

        if (!fired)
        {
            gunAudioSource.Stop();
            if (magSize[count] == 0)
            {
                isEmpty = true;
            }
        }
    }

    void OnFire()
    {
        if (magSize[count] > 0)
        {
            shootRate = false;
            weaponShake.GenerateImpulse(new Vector3(0, 0, 1f) * pW.impulseVal * onZoomVal);
            GunAnimation();
            MazelFlash();
            if (count != 4) { Shoot(); }
            magSize[count]--;
            magText.text = magSize[count].ToString() + "/" + storageSize[count].ToString();
            if (magSize[count] == 0)
            {
                fired = false;
                if (count != 1 && count != 4)
                {
                    isEmpty = true;
                }
            }
        }
        else if (storageSize[count] == 0)
        {
            if (!gunAudioSource.isPlaying)
            {
                gunAudioSource.PlayOneShot(emptyGunSound);
            }
            messageBox.PressentMessage("OUT OF AMMO", null);
        }
    }

    void GunAnimation()
    {
        gunAnimater[count].SetTrigger("Shooting");
        gunAudioSource.clip = pW.weaponSound;
        if (count == 4 || count == 1)
        {
            gunAudioSource.loop = true;
            if (!gunAudioSource.isPlaying) { gunAudioSource.Play(); }
        }
        else
        {
            gunAudioSource.loop = false; gunAudioSource.Play();
        }
    }

    void MazelFlash()
    {
        mazelFlash[count].Play();
    }

    void WeaponReload()
    {
        if ((Input.GetKeyDown(KeyCode.R) && !fired && !gamePaused) || (isEmpty && shootRate))
        {
            isEmpty = false;
            GunReload();
        }
    }

    void GunReload()
    {
        temp = tempMag;
        if (magSize[count] == temp) return;

        if (!reloading) return;
        reloading = false;

        if (storageSize[count] > 0)
        {
            gunAudioSource.PlayOneShot(pW.reloadSound);
            reloaded = true;
            StartCoroutine("ReloadTime");
        }
        else
        {
            reloading = true;
            messageBox.PressentMessage("OUT OF AMMO", null);
        }
    }

    IEnumerator ReloadTime()
    {
        yield return new WaitForSeconds(pW.reloadTime);
        if (storageSize[count] >= temp)
        {
            temp -= magSize[count];
        }
        else
        {
            temp = storageSize[count];
        }
        magSize[count] += temp;
        storageSize[count] -= temp;
        magText.text = magSize[count].ToString() + "/" + storageSize[count].ToString();

        reloading = true;
        reloaded = false;
    }

    void OnBombPlant(InputValue value)
    {
        if (timeBombCount > 0)
        {
            timeBomb = value.isPressed;
            if (timeBomb)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Physics.Raycast(ray, out hit, Mathf.Infinity, playerHitLayers, QueryTriggerInteraction.Ignore);

        if (hit.collider && !granadeSelected && !timeBomb)
        {
            Collider rayCastDitection = hit.collider;

            if (rayCastDitection.CompareTag("Enemy"))
            {
                HitAudio(enemyHitSound);
                if (count != 2)
                {
                    if(enemyHitEffectsIndex >= enemyHitEffects.Length) enemyHitEffectsIndex = 0;
                    enemyHitEffects[enemyHitEffectsIndex].gameObject.transform.position = hit.point;
                    enemyHitEffects[enemyHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                    enemyHitEffects[enemyHitEffectsIndex].Play();
                    damage = pW.weaponDamage;
                    enemyHitEffectsIndex++;
                }
                else
                {
                    Instantiate(enemyHitEffect1, hit.point, Quaternion.LookRotation(hit.normal), rayCastDitection.transform);
                    damage = DistByDamage(hit.point);
                }
                rayCastDitection.GetComponentInParent<Enemy>().TakeDamage(damage,-ray.direction,pW.bulletHitForce);
            }

            if (rayCastDitection.CompareTag("EnemySpawner"))
            {
                HitAudio(metalHitSound);
                if(metalHitEffectsIndex >= metalHitEffects.Length) metalHitEffectsIndex = 0;
                metalHitEffects[metalHitEffectsIndex].gameObject.transform.position = hit.point;
                metalHitEffects[metalHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                metalHitEffects[metalHitEffectsIndex].Play();
                if (count != 2)
                {
                    damage = pW.weaponDamage;
                }
                else
                {
                    damage = DistByDamage(hit.point);
                }

                rayCastDitection.GetComponent<EnemySpawner>().DamageTaker(damage);
                metalHitEffectsIndex++;
            }

            if (rayCastDitection.CompareTag("Wood"))
            {
                HitAudio(woodHitSound);
                if(woodHitEffectsIndex >= woodHitEffects.Length) woodHitEffectsIndex = 0;
                woodHitEffects[woodHitEffectsIndex].gameObject.transform.position = hit.point;
                woodHitEffects[woodHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                woodHitEffects[woodHitEffectsIndex].Play();
                Crate crate = rayCastDitection.gameObject.GetComponent<Crate>();
                if (crate)
                {
                    crate.TakeDamage(DamageByWeapon(hit.point));
                }
                woodHitEffectsIndex++;
            }

            if (rayCastDitection.CompareTag("Metal") || rayCastDitection.CompareTag("Vehical"))
            {
                HitAudio(metalHitSound);
                if(metalHitEffectsIndex >= metalHitEffects.Length) metalHitEffectsIndex = 0;
                metalHitEffects[metalHitEffectsIndex].gameObject.transform.position = hit.point;
                metalHitEffects[metalHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                metalHitEffects[metalHitEffectsIndex].Play();
                OilBarrel barrel = rayCastDitection.gameObject.GetComponent<OilBarrel>();
                if (barrel)
                {
                    barrel.TakeDamage(DamageByWeapon(hit.point));
                }
                metalHitEffectsIndex++;
            }

            if (rayCastDitection.CompareTag("TileGround"))
            {
                HitAudio(wallHitSound);
                Instantiate(stoneHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            if (rayCastDitection.CompareTag("Glass"))
            {
                HitAudio(glassHitSound);
                Instantiate(hitGlassEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            if (rayCastDitection.CompareTag("SandGround"))
            {
                HitAudio(sandHitSound);
                if(sandHitEffectsIndex >= sandHitEffects.Length) sandHitEffectsIndex = 0;
                sandHitEffects[sandHitEffectsIndex].gameObject.transform.position = hit.point;
                sandHitEffects[sandHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                sandHitEffects[sandHitEffectsIndex].Play();
                sandHitEffectsIndex++;
            }

            if (rayCastDitection.CompareTag("Robot"))
            {
                HitAudio(metalHitSound);
                if (count != 2)
                {
                    if(metalHitEffectsIndex >= metalHitEffects.Length) metalHitEffectsIndex = 0;
                    metalHitEffects[metalHitEffectsIndex].gameObject.transform.position = hit.point;
                    metalHitEffects[metalHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                    metalHitEffects[metalHitEffectsIndex].Play();
                    damage = pW.weaponDamage;
                    metalHitEffectsIndex++;
                }
                else
                {
                    Instantiate(shootGunMetalHitEffect, hit.point, Quaternion.LookRotation(hit.normal), rayCastDitection.transform);
                    damage = DistByDamage(hit.point);
                }

                RoboBomb roboBomb = rayCastDitection.GetComponentInParent<RoboBomb>();

                if (roboBomb) roboBomb.TakeDamage(damage);
                else rayCastDitection.GetComponentInParent<Robot>().TakeDamage(damage);
            }

            if (rayCastDitection.CompareTag("WalkingRobots"))
            {
                HitAudio(metalHitSound);
                if (count != 2)
                {
                    if(metalHitEffectsIndex >= metalHitEffects.Length) metalHitEffectsIndex = 0;
                    metalHitEffects[metalHitEffectsIndex].gameObject.transform.position = hit.point;
                    metalHitEffects[metalHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                    metalHitEffects[metalHitEffectsIndex].Play();
                    damage = pW.weaponDamage;
                    metalHitEffectsIndex++;
                }
                else
                {
                    Instantiate(shootGunMetalHitEffect, hit.point, Quaternion.LookRotation(hit.normal), rayCastDitection.transform);
                    damage = DistByDamage(hit.point);
                }

                rayCastDitection.GetComponentInParent<WalkingRobots>().TakeDamage(damage);
            }
        }
        else if (hit.collider && granadeSelected)
        {
            direction = hit.point - transform.position;
            if ((direction.x > 30f || direction.x < -30f) || (direction.z > 30f || direction.z < -30f))
            {
                speed = 100f;
            }
            else
            {
                speed = Vector3.Distance(hit.point, transform.position) * 8f;
            }
        }

        if (hit.collider && timeBomb)
        {
            float dist = Vector3.Distance(hit.point, transform.position);

            if (dist <= 5f)
            {
                for (int i = 0; i < bombPlantAreas.Count; i++)
                {
                    if (bombPlantAreas[i].name == hit.collider.name)
                    {
                        timeBombCount--;
                        Instantiate(timeBombObject, hit.point, Quaternion.LookRotation(hit.normal));
                        timeBomb = false;
                        bombPlantAreas.Remove(bombPlantAreas[i]);
                    }
                }
            }
            else
            {
                messageBox.PressentMessage("Go close to the object and press X to place the TimeBomb", null);
            }
        }
    }

    void HitAudio(AudioClip ac)
    {
        gunAudioSource.PlayOneShot(ac, volume);
    }

    int DamageByWeapon(Vector3 hitPoint)
    {
        if (count != 2)
        {
            return pW.weaponDamage;
        }
        else
        {
            return DistByDamage(hitPoint);
        }
    }

    int DistByDamage(Vector3 hitPos)
    {
        float dist = Vector3.Distance(transform.position, hitPos);
        int damage = Mathf.RoundToInt(((pW.weaponDamage * 10f) / dist));
        return damage;
    }

    public void WeaponAssigner(Weapon weaponType, int indexNo)
    {
        pW = weaponType;
        count = indexNo;
        tempMag = weaponType.magSize;

        magText.text = magSize[count].ToString() + "/" + storageSize[count].ToString();
    }

    public void NewWeapon(Animator anim, ParticleSystem mazilFlash)
    {
        gunAnimater.Add(anim);
        mazelFlash.Add(mazilFlash);
    }

    public bool BulletAdder(int ammoAdder, int max, int indexNo)
    {
        if (indexNo < storageSize.Count && storageSize[indexNo] < max)
        {
            gunAudioSource.PlayOneShot(collectSound);
            storageSize[indexNo] += ammoAdder;
            if (storageSize[indexNo] > max)
            {
                storageSize[indexNo] = max;
            }
            magText.text = magSize[count].ToString() + "/" + storageSize[count].ToString();
            return true;
        }

        return false;
    }

    public void WeaponActivator()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cantShoot = true;
        idle = true;
        toguleWG = true;
        weaponTexts.SetActive(true);
    }

    void OnPause(InputValue value)
    {
        if (value.isPressed && didClick)
        {
            Cursor.lockState = CursorLockMode.None;
            gamePaused = true;
            gunAudioSource.Stop();
            gunAudioSource.clip = null;
            StopCoroutine("ReloadTime");
            GetComponent<FirstPersonController>().enabled = false;
            paused = true;
            canZoom = false;
            ZoomOut();
            reloading = true;
            reloaded = false;
            isEmpty = false;
            idle = false;
            didClick = false;
            GameManager.gameManager.PauseMenu("player");
        }
    }

    public void ContinueState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GetComponent<FirstPersonController>().enabled = true;
        paused = false;
        didClick = true;
        if (!weaponState || !cantShoot) return;
        canZoom = true;
        idle = true;
    }

    public void StopShootingOrThrowing()
    {
        crossHair.SetActive(false);
        idle = false;
        canThrough = false;
        toguleWG = false;
    }

    public void ActivateShootingOrThrowing()
    {
        crossHair.SetActive(true);
        toguleWG = true;
        if(cantShoot == true)
        {
            idle = true;
        }
        else
        {
            canThrough = true;
        }
    }

    public bool GamePaused
    {
        get { return paused; }
    }

    public void UpdateGranadeText(int granadeCountRef)
    {
        for(int i = granadeCountRef;i > 0;i--)
        {
            granadeCount++;
        }
        granadeCountText.text = granadeCount.ToString();
    }

    public int GranadeCount
    {
        get { return granadeCount; }
    }
}
