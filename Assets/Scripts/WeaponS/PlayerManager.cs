using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] Transform playerCameraRoot;
    [SerializeField] Animator animator;
    [SerializeField] Transform NormalPoint;
    [SerializeField] Transform FpsHands;
    PlayerController playerController;
    public AudioSource audioSource;
    public WeaponHandle weaponHandle;
    public WeaponType currentWeapon;
    public GameObject playerWeaponPos;
    bool gamePaused = false;
    public bool GamePaused{ get { return gamePaused; } }


    [Header("Granade Values")]
    public GameObject granade;
    public Transform granadePos;
    public AudioClip granadeThrowSound;
    public TMP_Text granadeCountText;
    public TMP_Text granadeTimeText;
    int granadeCount;
    public int GranadeCount { get { return granadeCount; } }
    public bool fireState = true;
    public bool granadeState = false;
    bool previousGranadeState = false;
    public float granadeThrowSpeed = 100f;


    [Header("Cinemachine Components")]
    public CinemachineImpulseSource weaponShake;
    public CinemachineCamera playerCamera;

    public LayerMask playerHitLayers;
    public GameObject flashLight;
    public GameObject crossHair;
    public GameObject weaponTexts;
    float time;
    public bool fired;

    //Keep this true intially
    public bool idle = true;
    bool previousfireState = false;


    //flashLight On and Off
    bool isOn = false;
    bool granadeInHand = false;

    //Tougles between weapons and granade
    public bool onZoom = false;
    public bool canZoom = true;
    public bool CanZoom { get { return canZoom; } set { canZoom = value; } }
    SensitivityManager sensitivityManager;
    public SpecialOperation specialOperation;

    void OnEnable()
    {
        if(SensitivityManager.sensitivityManager) SensitivityManager.sensitivityManager.UpdateAdsSensy += UpdateSensy;
        Cursor.lockState = CursorLockMode.Locked;
        if(isOn) { flashLight.SetActive(true); }
    }

    void Start()
    {
        sensitivityManager = SensitivityManager.sensitivityManager;
        playerController = GetComponent<PlayerController>();
        playerController.RotationSpeed = (sensitivityManager)? sensitivityManager.sensitivity[SensyType.ADS] : 1f;
        UpdateGranadeText(granadeCount);
    }

    void OnDisable()
    {
        if(SensitivityManager.sensitivityManager) SensitivityManager.sensitivityManager.UpdateAdsSensy -= UpdateSensy;
        if(isOn) { flashLight.SetActive(false); }
    }

    void UpdateSensy(float value)
    {
        playerController.RotationSpeed = (sensitivityManager)? sensitivityManager.sensitivity[SensyType.ADS] : 1f;
    }

    void OnSpecialOperation(InputValue value)
    {
        if(specialOperation != null && !idle)
        {
            specialOperation.Perform();
        }
    }

    void OnFlashLight(InputValue value)
    {
        if(idle) return;

        if (flashLight != null)
        {
            isOn = !isOn;
            flashLight.SetActive(isOn);
        }
    }

    void OnZoomed(InputValue other)
    {
        if(idle || !canZoom) return;
        if(!other.isPressed) return;

        if(!currentWeapon.CanZoom()){ return; }

        ToggleScopeOnAndOff(!onZoom);
    }

    public void ToggleScopeOnAndOff(bool state)
    {
        onZoom = state;
        SetScope(onZoom);
        SetWeaponScrolling(!onZoom);
    }

    public void SetWeaponScrolling(bool currentZoomState)
    {
        weaponHandle.canScroll = currentZoomState;
    }

    public void SetScope(bool currentZoomState)
    {
        if(crossHair) crossHair.SetActive(!currentWeapon.Zoom(currentZoomState));

        if(currentZoomState)
        { 
            if(!currentWeapon.AdsPoint) return;
            FpsHands.localPosition = currentWeapon.AdsPoint.localPosition;
            FpsHands.localRotation = currentWeapon.AdsPoint.localRotation;  
            playerController.RotationSpeed = (sensitivityManager)? sensitivityManager.sensitivity[weaponHandle.GetWeaponType.sensyType] : 1f;
        }
        else 
        { 
            FpsHands.localPosition = NormalPoint.localPosition;
            FpsHands.localRotation = NormalPoint.localRotation;  
            playerController.RotationSpeed = (sensitivityManager)? sensitivityManager.sensitivity[SensyType.ADS] : 1f;
        }
    }

    void OnGranade(InputValue value)
    {
        if(idle || granadeCount <= 0) return;

        ToggleFireAndGranadeState();
    }

    void ToggleFireAndGranadeState()
    {
        fired = false;
        currentWeapon.Fire(fired);
        currentWeapon.ToggleWeaponReload(false);
        granadeState = !granadeState;
        animator.SetBool("GranadeState",granadeState);
        fireState = !granadeState;
    }

    void OnReload(InputValue value)
    {
        if(idle || fired || !fireState) return;
        
        if(currentWeapon.ToggleWeaponReload(true))
        {
            DisableScope();
        }
    }

    public void DisableScope()
    {
        if(onZoom) ToggleScopeOnAndOff(false);
        canZoom = false;
    }

    void OnFiring(InputValue other)
    {
        if(idle) return;

        fired = other.isPressed;
        if (fireState)
        {
            currentWeapon.Fire(fired);
        }

        if (granadeState)
        {
            if(granadeCount <= 0) ToggleFireAndGranadeState();
            else if(fired)
            {
                granadeTimeText.text = "";
                time = 5f;
                StopAllCoroutines();
                StartCoroutine(GranadeTimer());
            }
        }
    }

    IEnumerator GranadeTimer()
    {
        animator.SetTrigger("Hold");
        granadeInHand = true;
        while (time >= 0)
        {
            granadeTimeText.text = time.ToString("F1");
            time -= Time.deltaTime;
            CanThrow();
            yield return null;
        }

        CanThrow();
        granadeTimeText.text = "";
    }

    void CanThrow()
    {
        if (!fired && granadeInHand)
        {
            granadeInHand = false;
            animator.SetTrigger("Throw");
            ThrowGranade();
        }
    }

    void ThrowGranade()
    {
        granadeCount--;
        granadeCountText.text = granadeCount.ToString();
        audioSource.PlayOneShot(granadeThrowSound);
        GameObject gb = Instantiate(granade, granadePos.position, playerCameraRoot.rotation);
        gb.GetComponent<Granade>().ExecuteGranade(time);
        Rigidbody rb = gb.GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * granadeThrowSpeed, ForceMode.Impulse);
    }

    public void WeaponAssigner(WeaponType weaponTypeRef)
    {
        if(currentWeapon.reloading){ currentWeapon.ToggleWeaponReload(false); }

        currentWeapon = weaponTypeRef;
        currentWeapon.UpdateWeaponData();
    }

    public void WeaponActivator()
    {
        Cursor.lockState = CursorLockMode.Locked;
        idle = false;
        fireState = true;
        weaponTexts.SetActive(true);
    }

    void OnPause(InputValue value)
    {
        gamePaused = true;
        idle = gamePaused;
        weaponHandle.canScroll = !gamePaused;
        playerController.enabled = !gamePaused;
        audioSource.Stop();
        ToggleShootingOrThrowing(FireStateEnum.CantFire);

        StopCoroutine("ReloadTime");
        if(onZoom) ToggleScopeOnAndOff(false);
        GameManager.gameManager.PauseMenu("player");
    }

    public void ContinueState()
    {
        gamePaused = false;
        idle = gamePaused;
        weaponHandle.canScroll = !gamePaused;
        playerController.enabled = !gamePaused;
        ToggleShootingOrThrowing(FireStateEnum.CanFire);
    }

    public void IdleState(bool state)
    {
        idle = state;
        if(state)
        {
            currentWeapon.Fire(!state);
            currentWeapon.ToggleWeaponReload(!state);
        }
    }

    public void ToggleShootingOrThrowing(FireStateEnum fireStateEnum)
    {
        if(fireStateEnum == FireStateEnum.CanFire)
        {
            EnableCursor();
            ActivateShootingOrThrowing();
        }
        else
        {
            DisableCursor();
            StopShootingOrThrowing();
        }
    }

    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        crossHair.SetActive(false);
    }

    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        crossHair.SetActive(true);
    }

    public void StopShootingOrThrowing()
    {
        if(fireState) { previousfireState = fireState; currentWeapon.fired = false; fireState = false; }
        if(granadeState) { previousGranadeState = granadeState; granadeState = false; }
    }

    public void ActivateShootingOrThrowing()
    {
        if(previousfireState) { previousfireState = fireState; fireState = true; }
        if(previousGranadeState) { previousGranadeState = granadeState; granadeState = true; }
    }

    public void UpdateGranadeText(int granadeCountRef)
    {
        for(int i = granadeCountRef;i > 0;i--)
        {
            granadeCount++;
        }
        if(granadeCountText) granadeCountText.text = granadeCount.ToString();
    }

    public void StopAll()
    {
        idle = true;
        DisableScope();
    }

    public bool CheckSprintConditions()
    {
        if(!currentWeapon.shootRate || currentWeapon.reloading) return false;
        else return true;
    }

    public void ResetBlockedConstraints()
    {
        canZoom = true;
    }
}
