using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Linq;

public class Car : MonoBehaviour
{
    [SerializeField] GameObject carUIComponenets;
    [SerializeField] CarActivator carActivator;
    [SerializeField] CinemachineCamera carCam;
    [SerializeField] CarWeapon carWeapon;
    [SerializeField] Slider carSlider;
    [SerializeField] Car car;
    public GameObject carOptions;
    public GameObject crossHair;

 
    [SerializeField] PlayerInput carInputSystem;
    [SerializeField] ParticleSystem engineSmokeEffect;
    [SerializeField] AudioSource blastSound;
    [SerializeField] EnemyAttackTransition enemyAttackTransition;

    [SerializeField] LayerMask layers;
    [SerializeField] Transform[] playerExitPositions;
    Vector3[] positionAdustments = new Vector3[4] {new Vector3(0,0,1f),new Vector3(0,0,- 1f),new Vector3(1f,0,0),new Vector3(-1f,0,0)};

    [SerializeField] EnemySpawner[] enemySpawners;
    [SerializeField] PlayableDirector blastAnim;

    [SerializeField] ParticleSystem[] tierParticles;
    [SerializeField] ParticleSystem smokeEffect;

    [SerializeField] GameObject weapon;
    [SerializeField] GameObject[] Lights;
    [SerializeField] WheelCollider frontLWheel;
    [SerializeField] WheelCollider frontRWheel;
    [SerializeField] WheelCollider rearLWheel;
    [SerializeField] WheelCollider rearRWheel;

    Vector3 startPos;
    [SerializeField] BlastForce[] blastForces;
    [SerializeField] BlastForce[] blastForcesForWheel;

    [SerializeField] int carHealth = 10000;
    int carHealthRef;
    float zombieStopDistance = 1.2f;
    string damageTaker = "player";

    bool isBrakeing = false;
    bool isStart = true;
    bool engineStarted = false;
    bool isEngineSoundPlaying = true;
    bool isOn = false;
    bool dust;
    public bool destroyed = false;

    public float motorForce = 3500f;
    public float brakeForce = 6000f;
    public float speedRef;
    float steerMaxAngle = 35f;
    float maxSpeed = 25f;
    float verticalVal;
    float horizontalVal;
    float currentBrakeVal;
    float tierAngle;

    Rigidbody rb;

    AudioSource carSounds;
    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip exitSound;

    GameObject player;
    PlayerHealth playerHealth;
    PlayerFiring playerFiring;

    Transform originalPoint;

    void Awake()
    {
        GameManager.gameManager.car = this;
        blastSound = GameObject.FindWithTag("BlastAudio").GetComponent<AudioSource>();
        originalPoint = transform;
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        carUIComponenets.SetActive(false);
        carWeapon.enabled = false;
        car.enabled = false;
    }

    void Start()
    {
        carHealthRef = carHealth;
        carSlider.value = carHealth;
        carSounds = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (engineStarted)
        {
            FlashLight();
        }
    }

    void FixedUpdate()
    {
        if (isStart)
        {
            rb.constraints = RigidbodyConstraints.None;
            StartCoroutine("EngineStart");
            isStart = false;
        }

        if (engineStarted)
        {
            PlayerInput();
            Forces();
        }
        CameraController();
        WheelAngle();
        VehicalExit();
    }

    IEnumerator EngineStart()
    {
        carSounds.PlayOneShot(startSound);
        yield return new WaitForSeconds(1.3f);
        carInputSystem.enabled = true;
        engineSmokeEffect.Play();
        engineStarted = true;
        isEngineSoundPlaying = true;
    }

    void FlashLight()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if(playerFiring.GamePaused) { return; }

            isOn = !isOn;
            foreach (GameObject light in Lights)
            {
                light.SetActive(isOn);
            }
        }
    }

    void CameraController()
    {
        bool camActivator = Input.GetMouseButton(1);
        carCam.gameObject.GetComponent<CinemachineInputAxisController>().enabled = camActivator;
    }

    void PlayerInput()
    {
        horizontalVal = Input.GetAxis("Horizontal");
        verticalVal = Input.GetAxis("Vertical");

        float speed = rb.linearVelocity.magnitude;
        float maxMotorTorque = Mathf.Clamp01(1 - (speed / maxSpeed));

        if (verticalVal < 0)
        {
            WheelSlipValue(1);
        }
        if (verticalVal > 0)
        {
            WheelSlipValue(3);
        }

        rearLWheel.motorTorque = verticalVal * motorForce * maxMotorTorque;
        rearRWheel.motorTorque = verticalVal * motorForce * maxMotorTorque;

        speedRef = speed;
        float engineSoundPitch = speed / 5f;
        float pitchClamp = Mathf.Clamp(engineSoundPitch, 0.5f, 4f);
        carSounds.pitch = pitchClamp;
        if (isEngineSoundPlaying)
        {
            carSounds.Play();
            isEngineSoundPlaying = false;
        }

        isBrakeing = Input.GetKey(KeyCode.Space);
        BreakParticles(speed);
        foreach (ParticleSystem tierDust in tierParticles)
        {
            var emission = tierDust.emission;
            emission.enabled = dust;
        }

        VehicalSpeedToZero();

        currentBrakeVal = isBrakeing ? brakeForce : 0f;
    }

    void VehicalSpeedToZero()
    {
        if (verticalVal == 0)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime);
        }
    }

    void WheelSlipValue(int value)
    {
        WheelFrictionCurve Lfriction = frontLWheel.sidewaysFriction;
        WheelFrictionCurve Rfriction = frontRWheel.sidewaysFriction;
        Lfriction.extremumSlip = value;
        Rfriction.extremumSlip = value;
        frontLWheel.sidewaysFriction = Lfriction;
        frontRWheel.sidewaysFriction = Rfriction;
    }

    void BreakParticles(float speed)
    {
        dust = (speed > 4f) ? isBrakeing : false;
    }

    void Forces()
    {
        frontLWheel.brakeTorque = currentBrakeVal / 2;
        frontRWheel.brakeTorque = currentBrakeVal / 2;
        rearLWheel.brakeTorque = currentBrakeVal;
        rearRWheel.brakeTorque = currentBrakeVal;
    }

    void WheelAngle()
    {
        if (horizontalVal > 0)
        {
            tierAngle = steerMaxAngle + transform.eulerAngles.y;
            WheelAngleRef(tierAngle);
        }
        if (horizontalVal < 0)
        {
            tierAngle = steerMaxAngle - transform.eulerAngles.y;
            WheelAngleRef(-tierAngle);
        }
        if (horizontalVal == 0)
        {
            tierAngle = transform.eulerAngles.y;
            WheelAngleRef(tierAngle);
        }
        frontLWheel.steerAngle = horizontalVal * steerMaxAngle;
        frontRWheel.steerAngle = horizontalVal * steerMaxAngle;
    }

    void WheelAngleRef(float wheelVal)
    {
        frontLWheel.transform.rotation = Quaternion.Euler(0, wheelVal, 0);
        frontRWheel.transform.rotation = Quaternion.Euler(0, wheelVal, 0);
    }

    public void CarDamage(int damage)
    {
        if(car.enabled == true)
        {
            carHealthRef -= damage;
            carSlider.value = carHealthRef;
            if (!smokeEffect.isPlaying && carHealthRef <= 2000)
            {
                smokeEffect.Play();
            }
            if (carHealthRef <= 0 && !destroyed)
            {
                destroyed = true;
                carActivator.enabled = false;
                blastSound.Play();
                VehicalStoper();
                blastAnim.Play();

                foreach (BlastForce blastForce in blastForces)
                {
                    blastForce.GenerateForceForOther();
                }

                foreach (BlastForce blastForce in blastForcesForWheel)
                {
                    blastForce.ForceForRequiredDir();
                }
                frontLWheel.enabled = false;
                frontRWheel.enabled = false;
                rearLWheel.enabled = false;
                rearRWheel.enabled = false;
                rb.constraints = RigidbodyConstraints.FreezePositionX;
                rb.constraints = RigidbodyConstraints.FreezePositionZ;
                rb.constraints = RigidbodyConstraints.FreezeRotationY;
            }
        }
    }

    void VehicalExit()
    {
        if (Input.GetKey(KeyCode.F) && !playerFiring.GamePaused && !carWeapon.IsPressed)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            VehicalStoper();
        }
    }

    void VehicalStoper()
    {
        VehicalSpeedToZero();
        carWeapon.AllWeaponStopper();
        isStart = true;
        currentBrakeVal = 6000f;
        Forces();
        PlayerExitPositions();
    }

    void PlayerExitPositions()
    {
        foreach (Transform newPos in playerExitPositions)
        {
            Vector3 bottom = new Vector3(newPos.position.x, -15f, newPos.position.z);
            Vector3 top = new Vector3(newPos.position.x, -13f, newPos.position.z);
            Collider[] cols = Physics.OverlapCapsule(bottom, top, 0.5f, layers);

            if (cols.Length == 0)
            {
                player.transform.position = newPos.position;
                VehicalToPlayerChanger();
                return;
            }
        }

        if(carHealthRef <= 0) 
        { 
            player.transform.position = transform.position; 
            VehicalToPlayerChanger(); 
        }
        else
        {
            Debug.Log("Can't Exit");
        }
    }

    void VehicalToPlayerChanger()
    {
        carUIComponenets.SetActive(false);
        carInputSystem.enabled = false;
        carSounds.Stop();
        carSounds.PlayOneShot(exitSound);
        carSlider.gameObject.SetActive(false);
        carCam.Priority = 0;
        engineSmokeEffect.Stop();
        weapon.GetComponent<CarWeapon>().enabled = false;

        playerHealth.ActivateNormalMode();
        if(carHealthRef <= 0) { playerHealth.HealthConditions(playerHealth.playerHealthRef); }

        if(enemySpawners.All(x => x != null)) { enemyAttackTransition.ChangingObject(enemySpawners,damageTaker,player,zombieStopDistance); }
        Cursor.visible = true;
        gameObject.GetComponent<Car>().enabled = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Metal"))
        {
            OilBarrel oilBarrel = other.gameObject.GetComponent<OilBarrel>();
            if (oilBarrel) oilBarrel.TakeDamage(100);
        }
    }

    void OnPause(InputValue value)
    {
        if(value.isPressed)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            crossHair.SetActive(false);
            GameManager.gameManager.PauseMenu("vehical");
            carSounds.Stop();
            isEngineSoundPlaying = true;
        }
    }
    public void ActivateDriveMode()
    {
        carCam.Priority = 20;
        carUIComponenets.SetActive(true);
        car.enabled = true;
        carWeapon.enabled = true;
        carSlider.gameObject.SetActive(true);
    }

    public void AssignPlayer(GameObject playerRef)
    {
        player = playerRef;
        playerHealth = playerRef.GetComponent<PlayerHealth>();
        playerFiring = playerRef.GetComponent<PlayerFiring>();
    }

    public void ContinueState()
    {
        crossHair.SetActive(true);
    }

    public void Deactivate()
    {
        carOptions.SetActive(false);
    }
}
