using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CarWeapon : MonoBehaviour
{
    [SerializeField] Transform particleHolder;
    [SerializeField] HitEffects hitEffectsObject;
    [SerializeField] ParticleSystem[] weaponMeazelFlashes;
    [SerializeField] Image[] fireBallStrengthImages;
    [SerializeField] Image fireBallImage;
    [SerializeField] GameObject _FireBall;
    FireBall fireBall;

    [SerializeField] ParticleSystem _FireBallParticles;
    int vehicalGunbulletDamage = 200;

    [SerializeField] AudioSource gunSounds;
    AudioClip machineGunSound;
    [SerializeField] AudioClip fireBallSound;
    [SerializeField] AudioClip fireBallSoundLoop;

    [SerializeField] RectTransform crossHair;
    [SerializeField] LayerMask layers;
    [SerializeField] float rotateSpeed;
    bool fireRate = true;
    [SerializeField] Transform shootPos;

    [SerializeField] float forceSpeed;
    float fireBallDamage = 5000f;
    float presentDamage;
    float coolDownTime = 35f;

    public float volume;

    bool triggered = true;
    bool fireBallSelected = false;
    bool isPressed;
    public bool IsPressed { get { return isPressed; }}
    public float soundPitch = 0.7f;
    [SerializeField] float hitForce;
    
    Rigidbody fireBallRb;
    Collider fireBallCol;

    [SerializeField] List<ParticleSystem> groundHitParticles = new List<ParticleSystem>();
    [SerializeField] List<ParticleSystem> enemyHitParticles = new List<ParticleSystem>();
    [SerializeField] List<ParticleSystem> metalHitParticles = new List<ParticleSystem>();  
    [SerializeField] List<ParticleSystem> woodHitParticles = new List<ParticleSystem>();  
    [SerializeField] List<ParticleSystem> electricHitParticles = new List<ParticleSystem>(); 
    int enemyHitParticlesIndex = 0;
    int metalHitParticlesIndex = 0;
    int woodHitParticlesIndex = 0;
    int electricHitParticlesIndex = 0;
    int groundHitParticlesIndex = 0;

    void Start()
    {
        fireBall = _FireBall.GetComponent<FireBall>();
        fireBallCol = _FireBall.GetComponent<Collider>();
        fireBallRb = _FireBall.GetComponent<Rigidbody>();
        machineGunSound = gunSounds.clip;
        for(int i=0;i<10;i++)
        {
            groundHitParticles.Add(Instantiate(hitEffectsObject.groundEffect,transform.position,Quaternion.identity,particleHolder));
            enemyHitParticles.Add(Instantiate(hitEffectsObject.enemyEffect,transform.position,Quaternion.identity,particleHolder));
            metalHitParticles.Add(Instantiate(hitEffectsObject.metalEffect,transform.position,Quaternion.identity,particleHolder));
            woodHitParticles.Add(Instantiate(hitEffectsObject.woodEffect,transform.position,Quaternion.identity,particleHolder));
            electricHitParticles.Add(Instantiate(hitEffectsObject.electricShieldEffect,transform.position,Quaternion.identity,particleHolder));
        }
    }

    void Update()
    {
        crossHair.position = Input.mousePosition;

        FireBallShooter();
        MachineGun();
    }

    void FireBallShooter()
    {
        isPressed = Input.GetKey(KeyCode.LeftShift);

        if(isPressed)
        {
            WeaponRotation();
        }

        if(triggered && isPressed)
        {
            MachineGunStopper();
            _FireBall.SetActive(true);
            fireBallCol.enabled = false;
            fireBallSelected = true;
            triggered = false;
            StartCoroutine(FireBall());
        }
        else if(!isPressed && fireBallSelected)
        {
            gunSounds.clip = machineGunSound;
            fireBallImage.fillAmount = 0f;

            foreach(Image image in fireBallStrengthImages)
            {
                image.fillAmount = 0;
            }

            fireBall.FireBallDamage(presentDamage);
            fireBallRb.isKinematic = false;
            fireBallCol.enabled = true;
            fireBallRb.AddRelativeForce(Vector3.forward*forceSpeed,ForceMode.Impulse);
            fireBallSelected = false;
        }
    }

    void MachineGun()
    {
        if(!fireBallSelected)
        {
            bool isFire = Input.GetMouseButton(0);
            if (isFire && !GameManager.gameManager.GamePause)
            {
                if (!gunSounds.isPlaying)
                {
                    foreach (ParticleSystem ps in weaponMeazelFlashes)
                    {
                        ps.Play();
                    }
                    gunSounds.pitch = 1;
                    gunSounds.Play();
                }

                WeaponRotation();

                RaycastHit hit1;
                Ray ray = new Ray(shootPos.position, shootPos.forward);
                if (Physics.Raycast(ray,out hit1,Mathf.Infinity,layers))
                {
                    if (fireRate)
                    {
                        if (hit1.collider.CompareTag("Wood"))
                        {
                            HitAudio(hitEffectsObject.woodSound);
                            if(woodHitParticlesIndex >= woodHitParticles.Count) woodHitParticlesIndex = 0;
                            HitEffect(woodHitParticles[woodHitParticlesIndex], hit1.point, hit1.normal);
                            woodHitParticlesIndex++;
                        }
                        else if (hit1.collider.CompareTag("TileGround"))
                        {
                            HitAudio(hitEffectsObject.groundSound);
                            if(groundHitParticlesIndex >= groundHitParticles.Count) groundHitParticlesIndex = 0;
                            HitEffect(groundHitParticles[groundHitParticlesIndex], hit1.point, hit1.normal);
                            groundHitParticlesIndex++;
                        }
                        else if (hit1.collider.CompareTag("Metal"))
                        {
                            HitAudio(hitEffectsObject.metalSound);
                            if(metalHitParticlesIndex >= woodHitParticles.Count) metalHitParticlesIndex = 0;
                            HitEffect(metalHitParticles[metalHitParticlesIndex], hit1.point, hit1.normal);

                            OilBarrel oilBarrel = hit1.collider.GetComponent<OilBarrel>();
                            if (oilBarrel)
                            {
                                oilBarrel.TakeDamage(vehicalGunbulletDamage);
                            }
                            metalHitParticlesIndex++;
                        }
                        else if (hit1.collider.CompareTag("Enemy"))
                        {
                            HitAudio(hitEffectsObject.enemySound);
                            if(enemyHitParticlesIndex >= enemyHitParticles.Count) enemyHitParticlesIndex = 0;
                            HitEffect(enemyHitParticles[enemyHitParticlesIndex], hit1.point, hit1.normal);

                            hit1.collider.GetComponentInParent<Enemy>().TakeDamage(vehicalGunbulletDamage,-ray.direction,hitForce);
                            enemyHitParticlesIndex++;
                        }
                        else if (hit1.collider.CompareTag("EnemySpawner"))
                        {
                            HitAudio(hitEffectsObject.metalSound);
                            if(metalHitParticlesIndex >= metalHitParticles.Count) metalHitParticlesIndex = 0;
                            HitEffect(metalHitParticles[metalHitParticlesIndex], hit1.point, hit1.normal);

                            hit1.collider.GetComponent<EnemySpawner>().DamageTaker(vehicalGunbulletDamage);
                            metalHitParticlesIndex++;
                        }
                        else if(hit1.collider.CompareTag("ElectricShield"))
                        {
                            HitAudio(hitEffectsObject.electricShieldSound);
                            if(electricHitParticlesIndex >= electricHitParticles.Count) electricHitParticlesIndex = 0;
                            HitEffect(electricHitParticles[electricHitParticlesIndex], hit1.point, hit1.normal);
                            electricHitParticlesIndex++;
                        }

                        StartCoroutine(FireRate());
                    }
                }
            }
            else
            {
                MachineGunStopper();
            }
        }
    }

    IEnumerator FireRate()
    {
        fireRate = false;
        yield return new WaitForSeconds(0.05f);
        fireRate = true;
    }

    void HitAudio(AudioClip ac)
    {
        gunSounds.PlayOneShot(ac, volume);
    }

    void HitEffect(ParticleSystem ps,Vector3 hitPoint,Vector3 hitNormal)
    {
        ps.gameObject.transform.position = hitPoint;
        ps.gameObject.transform.LookAt(hitNormal);
        ps.Play();
    }

    public void MachineGunStopper()
    {
        foreach (ParticleSystem ps in weaponMeazelFlashes)
        {
            ps.Stop();
        }
        gunSounds.Stop();
    }

    public void AllWeaponStopper()
    {
        FireBallStopper();
        MachineGunStopper();
    }

    void FireBallStopper()
    {
        isPressed = false;
    }

    void WeaponRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,Mathf.Infinity,layers))
        {
            Vector3 startPos = (hit.point - transform.position);
            Quaternion endRotation = Quaternion.LookRotation(startPos);
            Vector3 eulers = endRotation.eulerAngles;
            eulers.x = (eulers.x > 180) ? eulers.x - 360 : eulers.x;
            eulers.x = Mathf.Clamp(eulers.x, -180, 25);
            endRotation.eulerAngles = eulers;
            transform.rotation = Quaternion.Slerp(transform.rotation,endRotation,Time.deltaTime*rotateSpeed);
        }
    }

    IEnumerator FireBall()
    {
        gunSounds.clip = fireBallSound;
        gunSounds.pitch = soundPitch;
        gunSounds.Play();
        
        float i = 0;
        while (i < 1f)
        {
            if(!isPressed) break;

            i += Time.deltaTime/5f;
            _FireBall.transform.localScale = new Vector3(i,i,i);
            _FireBallParticles.transform.localScale = new Vector3(i/2,i/2,i/2);

            foreach(Image image in fireBallStrengthImages)
            {
                image.fillAmount = i - 0.17f;
            }

            presentDamage = i*fireBallDamage; 
            yield return null;
        }

        if(isPressed)
        {
            gunSounds.clip = fireBallSoundLoop;
            gunSounds.pitch = 1;
            gunSounds.Play();
        }

        bool waitTime = true;
        while(waitTime)
        {
            if(!isPressed)
            {
                waitTime = false;
                float coolDownTimeRef = coolDownTime;
                while(coolDownTimeRef > 0)
                {
                    coolDownTimeRef -= Time.deltaTime;
                    fireBallImage.fillAmount = 1-(coolDownTimeRef/coolDownTime);
                    yield return null;
                }
                triggered = true;
                yield break;
            }

            yield return null;
        }
    }
}
