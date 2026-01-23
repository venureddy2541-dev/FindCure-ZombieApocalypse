using UnityEngine;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using System.Collections.Generic;

public class WeaponType : MonoBehaviour
{
    [SerializeField] WeaponAudioClipsSB weaponAudioClipsSB;
    public Animator animator;
    public ParticleSystem mazilFlash; 
    public LayerMask playerHitLayers;

    [Header("Cinemachine Components")]
    public CinemachineImpulseSource weaponShake;
    public CinemachineCamera playerCamera;

    public GameObject scopeZoomImage;
    public int magSize;
    public int storageSize;
    public WeaponData weaponData;

    [Header("WeaponSounds")]
    public AudioSource gunAudioSource;

    [Header("AmmoTexts")]
    public TMP_Text magText;

    int damage;
    float time;
    int granadeCount;
    int temp;
    float impulseRate;
    int originalZooom = 40;

    public bool reloading = true;

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

    protected virtual void Start()
    {
        impulseRate = weaponData.offScopeImpulseVal;
        magSize = weaponData.magSize;
        UpdateWeaponData();
    }

    public virtual void UpdateWeaponData()
    {
        magText.text = magSize.ToString()+"/"+storageSize.ToString();
        gunAudioSource.Stop();
        gunAudioSource.clip = weaponData.weaponSound;
    }

    public virtual bool Zoom(bool zoomState)
    {
        scopeZoomImage.SetActive(zoomState);
        ZoomInAndOut(zoomState);
        return !zoomState;
    }

    public void ZoomInAndOut(bool zoomState)
    {
        if(zoomState)
        {
            impulseRate = weaponData.onScopeImpulseVal;
            playerCamera.Lens.FieldOfView = weaponData.weaponZoom;
        }
        else
        {
            impulseRate = weaponData.offScopeImpulseVal;
            playerCamera.Lens.FieldOfView = originalZooom;
        }
    }

    public virtual void Fire(bool fired)
    {
        this.fired = fired;
        if (fired && !reloaded && shootRate)
        {
            OnFire();
        }
    }

    public void AutoReload()
    {
        if(magSize == 0 && storageSize > 0)
        {
            ToggleWeaponReload(true);
        }
    }

    protected virtual void OnFire()
    {
        if (magSize > 0)
        { 
            magSize--;
            magText.text = magSize.ToString() + "/" + storageSize.ToString();

            shootRate = false;
            WeaponShake();

            WeaponAnimation();

            WeaponSound();

            MazilFlash();

            ShootRay();

            if(magSize == 0)
            { 
                fired = false;
                if(gunAudioSource.loop) { gunAudioSource.loop = false; }
                gunAudioSource.Stop();
                AutoReload();
            }
        }
        else
        {
            gunAudioSource.PlayOneShot(weaponAudioClipsSB.emptyGunSound);
        }

        if(magSize == 0 && storageSize == 0)
        {
            MessageBox.messageBox.PressentMessage("OUT OF AMMO", null);
        }
    }

    public virtual void ToggleWeaponReload(bool canReload)
    {
        if(canReload) 
        { 
            GunReload(); 
        }
        else 
        { 
            gunAudioSource.Stop();
            StopCoroutine(ReloadTime()); 
            reloading = true; 
            reloaded = false;
        }
    }

    void GunReload()
    {
        if (weaponData.magSize == magSize) return;

        if (reloading)
        {
            reloading = false;
            if (storageSize > 0)
            {
                gunAudioSource.clip = weaponData.reloadSound;
                gunAudioSource.Play();
                reloaded = true;
                StartCoroutine(ReloadTime());
            }
            else
            {
                reloading = true;
                MessageBox.messageBox.PressentMessage("OUT OF AMMO", null);
            }
        }
    }

    IEnumerator ReloadTime()
    {
        yield return new WaitForSeconds(weaponData.reloadTime);
        if (storageSize >= weaponData.magSize)
        {
            temp = weaponData.magSize - magSize;
            magSize += temp;
            storageSize -= temp;
        }
        else
        {
            magSize = storageSize;
            storageSize = 0;
        }
        
        magText.text = magSize.ToString() + "/" + storageSize.ToString();

        gunAudioSource.clip = weaponData.weaponSound;
        reloading = true;
        reloaded = false;
    }

    protected virtual void WeaponShake()
    {
        weaponShake.GenerateImpulse(new Vector3(0, 0, 1f) * impulseRate);
    }

    protected virtual void WeaponAnimation()
    {
        animator.SetTrigger("Shooting");
    }

    protected virtual void WeaponSound()
    {
        gunAudioSource.Play();
    }

    void MazilFlash()
    {
        mazilFlash.Play();
    }

    void ShootRay()
    {
        InitiateShoot(Camera.main.transform.position,Camera.main.transform.forward);
    }

    protected virtual void InitiateShoot(Vector3 startPos,Vector3 endPos)
    {
        FireBullet(startPos,endPos);
        HitObject();
    }

    void FireBullet(Vector3 startPos,Vector3 endPos)
    {
        ray = new Ray(startPos,endPos);
        Physics.Raycast(ray, out hit, Mathf.Infinity, playerHitLayers, QueryTriggerInteraction.Ignore);
    }

    void HitObject()
    {
        if (hit.collider)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                HitAudio(weaponAudioClipsSB.enemyHitSound);

                HitEffect(RequiredParticles.instance.GetEnemyHitEffect());

                damage = weaponData.weaponDamage;
                hit.collider.GetComponentInParent<Enemy>().TakeDamage(damage,-ray.direction,weaponData.bulletHitForce);
            }

            if (hit.collider.CompareTag("EnemySpawner"))
            {
                HitAudio(weaponAudioClipsSB.metalHitSound);

                HitEffect(RequiredParticles.instance.GetMetalHitEffect());

                damage = weaponData.weaponDamage;
                hit.collider.GetComponent<EnemySpawner>().DamageTaker(damage);
            }

            if (hit.collider.CompareTag("Wood"))
            {
                HitAudio(weaponAudioClipsSB.woodHitSound);

                HitEffect(RequiredParticles.instance.GetWoodHitEffect());

                Crate crate = hit.collider.gameObject.GetComponent<Crate>();
                if (crate)
                {
                    crate.TakeDamage(DamageByWeapon(hit.point));
                }
            }

            if (hit.collider.CompareTag("Metal") || hit.collider.CompareTag("Vehical"))
            {
                HitAudio(weaponAudioClipsSB.metalHitSound);

                HitEffect(RequiredParticles.instance.GetMetalHitEffect());

                OilBarrel barrel = hit.collider.gameObject.GetComponent<OilBarrel>();
                if (barrel)
                {
                    barrel.TakeDamage(DamageByWeapon(hit.point));
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

                damage = weaponData.weaponDamage;
                RoboBomb roboBomb = hit.collider.GetComponentInParent<RoboBomb>();
                if (roboBomb) roboBomb.TakeDamage(damage);
                else hit.collider.GetComponentInParent<Robot>().TakeDamage(damage);
            }

            if (hit.collider.CompareTag("WalkingRobots"))
            {
                HitAudio(weaponAudioClipsSB.metalHitSound);

                HitEffect(RequiredParticles.instance.GetMetalHitEffect());

                damage = weaponData.weaponDamage;
                hit.collider.GetComponentInParent<WalkingRobots>().TakeDamage(damage);
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

    int DamageByWeapon(Vector3 hitPoint)
    {
        return weaponData.weaponDamage;
        //return DistByDamage(hitPoint);
    }

    int DistByDamage(Vector3 hitPos)
    {
        float dist = Vector3.Distance(transform.position, hitPos);
        int damage = Mathf.RoundToInt(((weaponData.weaponDamage * 10f) / dist));
        return damage;
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
}
