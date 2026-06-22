using UnityEngine;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using System.Collections.Generic;
using StarterAssets;

public class WeaponType : MonoBehaviour
{
    PlayerManager playerManager;
    RecoilManager recoilManager;

    public SensyType sensyType;
    public Transform AdsPoint;
    public AnimatorOverrideController overrideController;
    [SerializeField] WeaponAudioClipsSB weaponAudioClipsSB;
    [SerializeField] private Animator animator;
    public ParticleSystem mazilFlash;
    public LayerMask playerHitLayers;

    [Header("Cinemachine Components")]
    public CinemachineCamera playerCamera;

    public int magSize;
    public int storageSize;
    public WeaponData weaponData;

    [Header("WeaponSounds")]
    public AudioSource gunAudioSource;

    [Header("AmmoTexts")]
    [SerializeField] private TMP_Text magText;
    public TMP_Text MagText { get { return magText; } }

    int damage;
    float time;
    int granadeCount;
    int temp;
    int originalZooom = 40;

    public bool reloading = false;

    //Tougles between weapons and granade
    public bool weaponState = true;
    public bool reloaded = false;
    public bool fired;
    public bool idle = false;
    public bool shootRate = true;
    public bool canZoom = true;
    float reloadTime;
    public float volume;
    RaycastHit hit;
    Ray ray;

    float spreadValue;

    void OnEnable()
    {
        shootRate = true;
    }

    void OnDisable()
    {
        if(animator) animator.ResetTrigger("Reload");
    }

    protected virtual void Start()
    {
        spreadValue = weaponData.bulletSpread;
        magSize = weaponData.magSize;
        UpdateWeaponData();
    }

    public virtual void UpdateWeaponData()
    {
        if(magText) magText.text = magSize.ToString()+"/"+storageSize.ToString();
        gunAudioSource.Stop();
        gunAudioSource.clip = weaponData.weaponSound;
    }

    public virtual bool CanZoom()
    {
        if(!playerCamera) return false;
        return true;
    }

    public virtual bool Zoom(bool zoomState)
    {
        ZoomInAndOut(zoomState);
        return zoomState;
    }

    void ZoomInAndOut(bool zoomState)
    {
        if(zoomState)
        {
            spreadValue = weaponData.onScopeBulletSpread;
            playerCamera.Lens.FieldOfView = weaponData.weaponZoom;
        }
        else
        {
            spreadValue = weaponData.bulletSpread;
            playerCamera.Lens.FieldOfView = originalZooom;
        }
    }

    public void AutoReload()
    {
        if(magSize == 0 && storageSize > 0)
        {
            if(ToggleWeaponReload(true)) playerManager.DisableScope();
        }
    }
    
    public virtual void Fire(bool fired)
    {
        this.fired = fired;
        if (fired && !reloaded && shootRate)
        {
            OnFire();
            StartCoroutine("FireRate");
        }
    }

    protected virtual void OnFire()
    {
        if (magSize > 0)
        { 
            magSize--;
            if(magText) magText.text = magSize.ToString() + "/" + storageSize.ToString();

            shootRate = false;

            WeaponAnimation();

            WeaponSound();

            WeaponRecoil();

            MazilFlash();

            ShootRay();

            if(magSize == 0)
            { 
                fired = false;
                AutoReload();
            }
        }
        else
        {
            gunAudioSource.PlayOneShot(weaponAudioClipsSB.emptyGunSound);
        }

        if(magSize == 0 && storageSize == 0)
        {
            if(MessageBox.messageBox) MessageBox.messageBox.PressentMessage("OUT OF AMMO", null);
        }
    }

    protected virtual void WeaponAnimation()
    {
        //Override this for weapon Fire animations
        animator.CrossFadeInFixedTime("Fire",0.05f,0,0f);
    }

    protected virtual void WeaponSound()
    {
        gunAudioSource.Play(); 
    }

    void MazilFlash()
    {
        mazilFlash.Play();
    }

    void WeaponRecoil()
    {
        recoilManager.TriggerRecoil();
    }

    IEnumerator FireRate()
    {
        yield return new WaitForSeconds(weaponData.fireRate);
        shootRate = true;
    }

    public virtual bool ToggleWeaponReload(bool canReload)
    {
        if(canReload)
        {
            if (weaponData.magSize == magSize) return false;
            if (storageSize > 0)
            {
                GunReload();
                return true;
            }

            if(MessageBox.messageBox) MessageBox.messageBox.PressentMessage("OUT OF AMMO", null);
            return false;
        }

        gunAudioSource.Stop();
        StopCoroutine(ReloadTime());
        playerManager.ResetBlockedConstraints();
        reloading = false;
        reloaded = false;
        return false;
    }

    void GunReload()
    {
        if (!reloading)
        {
            reloading = true;
            reloaded = true;
            animator.SetTrigger("Reload");
            StartCoroutine(ReloadTime());
        }
    }

    IEnumerator ReloadTime()
    {
        while(!shootRate)
        {
            yield return null;
        }

        gunAudioSource.clip = weaponData.reloadSound;
        gunAudioSource.Play();

        yield return new WaitForSeconds(weaponData.reloadTime);

        temp = weaponData.magSize - magSize;
        if (storageSize >= temp)
        {
            magSize += temp;
            storageSize -= temp;
        }
        else
        {
            magSize += storageSize;
            storageSize = 0;
        }
        
        magText.text = magSize.ToString() + "/" + storageSize.ToString();

        gunAudioSource.Stop();
        gunAudioSource.clip = weaponData.weaponSound;
        reloading = false;
        reloaded = false;
        playerManager.ResetBlockedConstraints();
    }

    void ShootRay()
    {
        InitiateShoot(Camera.main.transform.position,Camera.main.transform.forward);
    }

    protected virtual void InitiateShoot(Vector3 startPos,Vector3 direction)
    {
        FireBullet(startPos,direction);
        HitObject();
    }

    void FireBullet(Vector3 startPos,Vector3 direction)
    {
        ray = new Ray(startPos,direction + RandomSpreadValue());
        Physics.Raycast(ray, out hit, weaponData.range, playerHitLayers, QueryTriggerInteraction.Ignore);
    }

    Vector3 RandomSpreadValue()
    {
        float upSplit = Random.Range(-spreadValue,spreadValue);
        float leftSplit = Random.Range(-spreadValue,spreadValue);
        Vector3 splitDirc = transform.right*upSplit + transform.up*leftSplit;
        return splitDirc;
    }

    void HitObject()
    {
        if (hit.collider && RequiredParticles.instance)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                HitAudio(weaponAudioClipsSB.enemyHitSound);

                HitEffect(RequiredParticles.instance.GetEnemyHitEffect());

                hit.collider.GetComponentInParent<Enemy>().TakeDamage(weaponData.weaponDamage,-ray.direction,weaponData.bulletHitForce);
            }

            if (hit.collider.CompareTag("EnemySpawner"))
            {
                HitAudio(weaponAudioClipsSB.metalHitSound);

                HitEffect(RequiredParticles.instance.GetMetalHitEffect());

                hit.collider.GetComponent<EnemySpawner>().DamageTaker(weaponData.weaponDamage);
            }

            if (hit.collider.CompareTag("Wood"))
            {
                HitAudio(weaponAudioClipsSB.woodHitSound);

                HitEffect(RequiredParticles.instance.GetWoodHitEffect());

                Crate crate = hit.collider.gameObject.GetComponent<Crate>();
                if (crate)
                {
                    crate.TakeDamage(weaponData.weaponDamage);
                }
            }

            if (hit.collider.CompareTag("Metal") || hit.collider.CompareTag("Vehical"))
            {
                HitAudio(weaponAudioClipsSB.metalHitSound);

                HitEffect(RequiredParticles.instance.GetMetalHitEffect());

                OilBarrel barrel = hit.collider.gameObject.GetComponent<OilBarrel>();
                if (barrel)
                {
                    barrel.TakeDamage(weaponData.weaponDamage);
                }
            }

            if (hit.collider.CompareTag("TileGround"))
            {
                HitAudio(weaponAudioClipsSB.wallHitSound);

                HitEffect(RequiredParticles.instance.GetStoneHitEffect());
            }

            if (hit.collider.CompareTag("Glass"))
            {
                HitAudio(weaponAudioClipsSB.glassHitSound);
            }

            if (hit.collider.CompareTag("SandGround"))
            {
                HitAudio(weaponAudioClipsSB.sandHitSound);

                HitEffect(RequiredParticles.instance.GetSandHitEffect());
            }

            if (hit.collider.CompareTag("Robot"))
            {
                HitAudio(weaponAudioClipsSB.metalHitSound);

                HitEffect(RequiredParticles.instance.GetMetalHitEffect());

                RoboBomb roboBomb = hit.collider.GetComponentInParent<RoboBomb>();
                if (roboBomb) roboBomb.TakeDamage(weaponData.weaponDamage);
                else hit.collider.GetComponentInParent<Robot>().TakeDamage(weaponData.weaponDamage);
            }

            if (hit.collider.CompareTag("WalkingRobots"))
            {
                HitAudio(weaponAudioClipsSB.metalHitSound);

                HitEffect(RequiredParticles.instance.GetMetalHitEffect());

                hit.collider.GetComponentInParent<WalkingRobots>().TakeDamage(weaponData.weaponDamage);
            }
        }
    }

    void HitAudio(AudioClip ac)
    {
        gunAudioSource.PlayOneShot(ac, volume);
    }

    void HitEffect(ParticleSystem currentEffect)
    {
        currentEffect.transform.position = hit.point;
        currentEffect.transform.rotation = Quaternion.LookRotation(hit.normal);
        currentEffect.Play();
    }

    public bool BulletAdder(int ammoAdder, int max)
    {
        if (storageSize < max)
        {
            gunAudioSource.PlayOneShot(weaponAudioClipsSB.collectSound);
            storageSize += ammoAdder;
            if (storageSize > max)
            {
                storageSize = max;
            }
            if(gameObject.activeSelf) { magText.text = magSize.ToString() + "/" + storageSize.ToString(); }
            return true;
        }

        return false;
    }

    public void AssiginComponenets(PlayerManager playerManager,RecoilManager recoilManager)
    {
        this.playerManager = playerManager;
        this.recoilManager = recoilManager;
    }

    public void AssiginPickUpWeaponComponenets(PlayerManager playerManager,
                                                RecoilManager recoilManager,
                                                Animator animator,
                                                TMP_Text magText,
                                                AudioSource gunAudioSource)
    {
        this.playerManager = playerManager;
        this.recoilManager = recoilManager;
        this.animator = animator;
        this.magText = magText;
        this.gunAudioSource = gunAudioSource;
    }
}
