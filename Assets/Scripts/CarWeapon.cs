using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CarWeapon : MonoBehaviour
{
    [SerializeField] WeaponAudioClipsSB weaponAudioClipsSB;
    [SerializeField] Transform particleHolder;
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


    void Start()
    {
        fireBall = _FireBall.GetComponent<FireBall>();
        fireBallCol = _FireBall.GetComponent<Collider>();
        fireBallRb = _FireBall.GetComponent<Rigidbody>();
        machineGunSound = gunSounds.clip;
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
                            HitAudio(weaponAudioClipsSB.woodHitSound);

                            HitEffect(RequiredParticles.instance.GetWoodHitEffect(), hit1.point, hit1.normal);
                        }
                        else if (hit1.collider.CompareTag("TileGround"))
                        {
                            HitAudio(weaponAudioClipsSB.wallHitSound);

                            HitEffect(RequiredParticles.instance.GetStoneHitEffect(), hit1.point, hit1.normal);
                        }
                        else if (hit1.collider.CompareTag("Metal"))
                        {
                            HitAudio(weaponAudioClipsSB.metalHitSound);

                            HitEffect(RequiredParticles.instance.GetMetalHitEffect(), hit1.point, hit1.normal);

                            OilBarrel oilBarrel = hit1.collider.GetComponent<OilBarrel>();
                            if (oilBarrel)
                            {
                                oilBarrel.TakeDamage(vehicalGunbulletDamage);
                            }
                        }
                        else if (hit1.collider.CompareTag("Enemy"))
                        {
                            HitAudio(weaponAudioClipsSB.enemyHitSound);

                            HitEffect(RequiredParticles.instance.GetEnemyHitEffect(), hit1.point, hit1.normal);

                            hit1.collider.GetComponentInParent<Enemy>().TakeDamage(vehicalGunbulletDamage,-ray.direction,hitForce);
                        }
                        else if (hit1.collider.CompareTag("EnemySpawner"))
                        {
                            HitAudio(weaponAudioClipsSB.metalHitSound);

                            HitEffect(RequiredParticles.instance.GetMetalHitEffect(), hit1.point, hit1.normal);

                            hit1.collider.GetComponent<EnemySpawner>().DamageTaker(vehicalGunbulletDamage);
                        }
                        else if(hit1.collider.CompareTag("ElectricShield"))
                        {
                            HitAudio(weaponAudioClipsSB.electricShieldHitSound);

                            HitEffect(RequiredParticles.instance.GetElectricShieldHitEffect(), hit1.point, hit1.normal);
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

    void HitEffect(ParticleSystem currentEffect,Vector3 hitPoint,Vector3 hitNormal)
    {
        currentEffect.transform.position = hitPoint;
        currentEffect.transform.LookAt(hitNormal);
        currentEffect.Play();
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
