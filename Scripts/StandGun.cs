using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class StandGun : MonoBehaviour
{
    IsAlive isAlive;
    [SerializeField] EnemyAttackTransition enemyAttackTransition;

    [SerializeField] EnemySpawner[] enemySpawners;
    CinemachineImpulseSource impulse;
    [SerializeField] ParticleSystem bomb;
    [SerializeField] Slider standGunSlider;
    [SerializeField] PlayableDirector blastAnim;

    [SerializeField] GameObject standGunUIComponents;
    [SerializeField] Image shockWaveImage;
    [SerializeField] GameObject EnemySpawners;
    [SerializeField] GameObject standGunActivator;
    [SerializeField] GameObject GunShell;
    [SerializeField] GameObject gunRotator;
    [SerializeField] CinemachineCamera gunCamera;

    [SerializeField] ParticleSystem bullets;
    [SerializeField] RectTransform gunCrossHair;

    AudioSource standGunSounds;
    AudioSource blastSounds;
    [SerializeField] AudioClip shockWaveSound;

    bool triggered = true;
    bool activated = false;
    bool isRunning = true;
    bool pressed;
    [SerializeField] float speed = 200f;
    float timer = 0f;
    float radius = 10f;
    float shockWaveTime = 30f;
    [SerializeField] float rotationSpeed = 5f;
    float zombieStopDistance = 1.2f;
    [SerializeField] int gunHealth = 15000;
    public int gunHealthRef;
    [SerializeField] LayerMask layers;
    [SerializeField] PlayerInput standGunInput;
    Vector2 crosshairPos;
    MusicPlayer musicPlayer;
    [SerializeField] AudioClip music;
    [SerializeField] Waves waves;

    GameObject player;
    PlayerFiring playerFiring;
    PlayerHealth playerHealth;
    [SerializeField] float hitForce;

    void Awake()
    {
        GameManager.gameManager.standGun = this;
        blastSounds = GameObject.FindWithTag("BlastAudio").GetComponent<AudioSource>();
        musicPlayer = GameObject.FindWithTag("MusicPlayer").GetComponent<MusicPlayer>();
        isAlive = GetComponent<IsAlive>();
        impulse = GetComponent<CinemachineImpulseSource>();
        standGunSounds = GetComponent<AudioSource>();

        gunHealthRef = gunHealth;
        standGunSlider.value = gunHealth;
        standGunUIComponents.SetActive(false);
        standGunInput.enabled = false;
    }

    void OnExit(InputValue value)
    {
        if(GameManager.gameManager.GamePause) { return; }
        
        if (pressed) return;

        activated = false;
        Deactivator();
    }

    void OnPause(InputValue value)
    {
        if(value.isPressed)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pressed = false;
            GameManager.gameManager.PauseMenu("standgun");
        }
    }

    void OnFire(InputValue value)
    {
        pressed = value.isPressed;
        if(activated && isRunning)
        {
            isRunning = false;
            StartCoroutine("Firing");
        }
    }

    IEnumerator Firing()
    {
        while (activated && !GameManager.gameManager.GamePause && player.activeSelf)
        {
            gunCrossHair.position = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layers))
            {
                Vector3 hitPos = hit.point - GunShell.transform.position;
                Quaternion rotation = Quaternion.LookRotation(hitPos);
                Vector3 eulers = rotation.eulerAngles;

                eulers.y = NormalizeAngle360(eulers.y);
                eulers.x = (eulers.x > 180) ? eulers.x - 360 : eulers.x;

                eulers.y = Mathf.Clamp(eulers.y, 45f, 135f);
                eulers.x = Mathf.Clamp(eulers.x, -3f, 18f);
                eulers.z = 0;
                GunShell.transform.rotation = Quaternion.Slerp(GunShell.transform.rotation, Quaternion.Euler(eulers), Time.deltaTime * rotationSpeed);
            }
            if (pressed)
            {
                GunAudio();
                var emission = bullets.emission;
                emission.enabled = true;
                impulse.GenerateImpulse(new Vector3(0, -0.3f, 0));
                gunRotator.transform.Rotate(1 * Time.deltaTime * speed, 0, 0);
            }
            else
            {
                FireStoping();
            }
            yield return null;
        }

        if(!player.activeSelf)
        {
            Deactivator();
        }
        FireStoping();
        isRunning = true;
        yield break;
    }

    void FireStoping()
    {
        standGunSounds.Stop();
        var emission = bullets.emission;
        emission.enabled = false;
        impulse.GenerateImpulse(Vector3.zero);
    }

    void GunAudio()
    {
        if(!standGunSounds.isPlaying)
        {
            standGunSounds.Play();
        }
    }

    float NormalizeAngle360(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }
    
    public void ActivateStandGunMode()
    {
        if (musicPlayer.presentClip != music && waves.Count > 0)
        {
            musicPlayer.UpdateMusic(music);
        }
        
        Cursor.visible = false;
        gunCamera.Priority = 20;
        standGunUIComponents.gameObject.SetActive(true);
        standGunInput.enabled = true;
        gunCrossHair.gameObject.SetActive(true);  
        EnemySpawners.SetActive(true);
        activated = true;
    }

    void Deactivator()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        standGunUIComponents.gameObject.SetActive(false);
        standGunInput.enabled = false;
        gunCamera.Priority = 0;
        gunCrossHair.gameObject.SetActive(false);
        if(player.activeSelf)
        {
            playerHealth.ActivateNormalMode();
        }
    }

    public void GunDamage(int damage)
    {
        gunHealthRef -= damage;
        standGunSlider.value = gunHealthRef;
        if(gunHealthRef <= 0 && triggered)
        {
            triggered = false;
            isAlive.alive = false;
            activated = false;
            Deactivator();
            EnemyTarget("player",player);
            blastAnim.Play();
            blastSounds.Play();
        }
    }

    public void EnemyTarget(string targetName , GameObject targetPos)
    {
        enemyAttackTransition.ChangingObject(enemySpawners,targetName,targetPos,zombieStopDistance);
    }

    void OnBlast(InputValue value)
    {
        if(GameManager.gameManager.GamePause) { return; }

        if(value.isPressed && timer <= 0)
        {
            blastSounds.PlayOneShot(shockWaveSound);
            bomb.Play();
            Collider[] other = Physics.OverlapSphere(transform.position,radius);
            foreach(Collider col in other)
            {
                if(col.CompareTag("EnemyMain"))
                {
                    col.gameObject.GetComponent<Enemy>().TakeDamage(300,-(col.transform.position - transform.position),hitForce);
                }
            }
            timer = shockWaveTime;
            StartCoroutine("Timer");
        }
    }

    IEnumerator Timer()
    {
        while (timer >= 0)
        {
            shockWaveImage.fillAmount = (1 - (timer / shockWaveTime));
            timer -= Time.deltaTime;
            yield return null;
        }
    }
    
    public void AssignPlayer(GameObject playerRef)
    {
        player = playerRef;
        playerHealth = playerRef.GetComponent<PlayerHealth>();
        playerFiring = playerRef.GetComponent<PlayerFiring>();
    }
}
