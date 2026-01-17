using UnityEngine;
using System.Collections;
using TMPro;
using Unity.Cinemachine;

public class WeaponType : MonoBehaviour
{
    public HitParticles particles;
    public AudioClips audioClips;

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
        //storageSize = weaponData.maxAmmo;
        UpdateWeaponData();
    }

    public virtual void UpdateWeaponData()
    {
        magText.text = magSize.ToString()+"/"+storageSize.ToString();
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
        if (fired && !reloaded && shootRate && magSize > 0)
        {
            OnFire();
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
            gunAudioSource.clip = null;
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

        reloading = true;
        reloaded = false;
    }

    protected virtual void OnFire()
    {
        if (magSize > 0)
        { 
            magSize--;
            magText.text = magSize.ToString() + "/" + storageSize.ToString();

            shootRate = false;
            weaponShake.GenerateImpulse(new Vector3(0, 0, 1f) * impulseRate);

            WeaponAnimation();

            WeaponSound();

            MazilFlash();

            Shoot();

            if (magSize == 0)
            {
                fired = false;
                if(storageSize > 0) { ToggleWeaponReload(true); }
            }
        }
        else if (storageSize == 0)
        {
            if (!gunAudioSource.isPlaying)
            {
                gunAudioSource.PlayOneShot(audioClips.emptyGunSound);
            }
            MessageBox.messageBox.PressentMessage("OUT OF AMMO", null);
        }
    }

    protected virtual void WeaponAnimation()
    {
        animator.SetTrigger("Shooting");
    }

    protected virtual void WeaponSound()
    {
        if(gunAudioSource.clip != weaponData.weaponSound)
        {
            gunAudioSource.clip = weaponData.weaponSound;
        }
        gunAudioSource.Play();
    }

    void MazilFlash()
    {
        mazilFlash.Play();
    }

    void Shoot()
    {
        InitiateShoot(Camera.main.transform.position,Camera.main.transform.forward);

        /*else if (hit.hit.collider && granadeSelected)
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

        if (hit.hit.collider && timeBomb)
        {
            float dist = Vector3.Distance(hit.point, transform.position);

            if (dist <= 5f)
            {
                for (int i = 0; i < bombPlantAreas.Count; i++)
                {
                    if (bombPlantAreas[i].name == hit.hit.collider.name)
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
                MessageBox.messageBox.PressentMessage("Go close to the object and press X to place the TimeBomb", null);
            }
        }*/
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
                HitAudio(audioClips.enemyHitSound);
                
                if(particles.enemyHitEffectsIndex >= particles.enemyHitEffects.Length) particles.enemyHitEffectsIndex = 0;
                particles.enemyHitEffects[particles.enemyHitEffectsIndex].gameObject.transform.position = hit.point;
                particles.enemyHitEffects[particles.enemyHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                particles.enemyHitEffects[particles.enemyHitEffectsIndex].Play();
                damage = weaponData.weaponDamage;
                particles.enemyHitEffectsIndex++;

                hit.collider.GetComponentInParent<Enemy>().TakeDamage(damage,-ray.direction,weaponData.bulletHitForce);
            }

            if (hit.collider.CompareTag("EnemySpawner"))
            {
                HitAudio(audioClips.metalHitSound);
                if(particles.metalHitEffectsIndex >= particles.metalHitEffects.Length) particles.metalHitEffectsIndex = 0;
                particles.metalHitEffects[particles.metalHitEffectsIndex].gameObject.transform.position = hit.point;
                particles.metalHitEffects[particles.metalHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                particles.metalHitEffects[particles.metalHitEffectsIndex].Play();

                damage = weaponData.weaponDamage;

                hit.collider.GetComponent<EnemySpawner>().DamageTaker(damage);
                particles.metalHitEffectsIndex++;
            }

            if (hit.collider.CompareTag("Wood"))
            {
                HitAudio(audioClips.woodHitSound);
                if(particles.woodHitEffectsIndex >= particles.woodHitEffects.Length) particles.woodHitEffectsIndex = 0;
                particles.woodHitEffects[particles.woodHitEffectsIndex].gameObject.transform.position = hit.point;
                particles.woodHitEffects[particles.woodHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                particles.woodHitEffects[particles.woodHitEffectsIndex].Play();
                Crate crate = hit.collider.gameObject.GetComponent<Crate>();
                if (crate)
                {
                    crate.TakeDamage(DamageByWeapon(hit.point));
                }
                particles.woodHitEffectsIndex++;
            }

            if (hit.collider.CompareTag("Metal") || hit.collider.CompareTag("Vehical"))
            {
                HitAudio(audioClips.metalHitSound);
                if(particles.metalHitEffectsIndex >= particles.metalHitEffects.Length) particles.metalHitEffectsIndex = 0;
                particles.metalHitEffects[particles.metalHitEffectsIndex].gameObject.transform.position = hit.point;
                particles.metalHitEffects[particles.metalHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                particles.metalHitEffects[particles.metalHitEffectsIndex].Play();
                OilBarrel barrel = hit.collider.gameObject.GetComponent<OilBarrel>();
                if (barrel)
                {
                    barrel.TakeDamage(DamageByWeapon(hit.point));
                }
                particles.metalHitEffectsIndex++;
            }

            if (hit.collider.CompareTag("TileGround"))
            {
                HitAudio(audioClips.wallHitSound);
                Instantiate(particles.stoneHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            if (hit.collider.CompareTag("Glass"))
            {
                HitAudio(audioClips.glassHitSound);
                Instantiate(particles.hitGlassEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            if (hit.collider.CompareTag("SandGround"))
            {
                HitAudio(audioClips.sandHitSound);
                if(particles.sandHitEffectsIndex >= particles.sandHitEffects.Length) particles.sandHitEffectsIndex = 0;
                particles.sandHitEffects[particles.sandHitEffectsIndex].gameObject.transform.position = hit.point;
                particles.sandHitEffects[particles.sandHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                particles.sandHitEffects[particles.sandHitEffectsIndex].Play();
                particles.sandHitEffectsIndex++;
            }

            if (hit.collider.CompareTag("Robot"))
            {
                HitAudio(audioClips.metalHitSound);

                if(particles.metalHitEffectsIndex >= particles.metalHitEffects.Length) particles.metalHitEffectsIndex = 0;
                particles.metalHitEffects[particles.metalHitEffectsIndex].gameObject.transform.position = hit.point;
                particles.metalHitEffects[particles.metalHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                particles.metalHitEffects[particles.metalHitEffectsIndex].Play();
                damage = weaponData.weaponDamage;
                particles.metalHitEffectsIndex++;

                RoboBomb roboBomb = hit.collider.GetComponentInParent<RoboBomb>();

                if (roboBomb) roboBomb.TakeDamage(damage);
                else hit.collider.GetComponentInParent<Robot>().TakeDamage(damage);
            }

            if (hit.collider.CompareTag("WalkingRobots"))
            {
                HitAudio(audioClips.metalHitSound);

                if(particles.metalHitEffectsIndex >= particles.metalHitEffects.Length) particles.metalHitEffectsIndex = 0;
                particles.metalHitEffects[particles.metalHitEffectsIndex].gameObject.transform.position = hit.point;
                particles.metalHitEffects[particles.metalHitEffectsIndex].gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                particles.metalHitEffects[particles.metalHitEffectsIndex].Play();
                damage = weaponData.weaponDamage;
                particles.metalHitEffectsIndex++;

                hit.collider.GetComponentInParent<WalkingRobots>().TakeDamage(damage);
            }
        }
    }

    void HitAudio(AudioClip ac)
    {
        gunAudioSource.PlayOneShot(ac, volume);
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
            gunAudioSource.PlayOneShot(audioClips.collectSound);
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
