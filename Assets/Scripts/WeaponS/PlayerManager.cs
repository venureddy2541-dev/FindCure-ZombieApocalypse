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
    FirstPersonController firstPersonController;
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
    public bool fireState = false;
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
    int granadeCount;
    public bool fired;
    public bool idle = true;
    bool previousfireState = false;


    //flashLight On and Off
    bool isOn = false;
    bool granadeInHand = false;

    //Tougles between weapons and granade
    public bool onZoom = false;
    public bool canZoom = true;

    public SpecialOperation specialOperation;

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if(isOn) { flashLight.SetActive(true); }
    }

    void Start()
    {
        firstPersonController = GetComponent<FirstPersonController>();

        UpdateGranadeText(granadeCount);
    }

    void OnDisable()
    {
        if(isOn) { flashLight.SetActive(false); }
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
        if(idle && !canZoom) return;

        onZoom = other.isPressed;
        SetWeaponScrolling(!onZoom);
        SetScope(onZoom);
    }

    public void SetWeaponScrolling(bool currentZoomState)
    {
        weaponHandle.canScroll = currentZoomState;
    }

    public void SetScope(bool currentZoomState)
    {
        crossHair.SetActive(currentWeapon.Zoom(currentZoomState));
    }

    void OnGranade(InputValue value)
    {
        if(idle) return;

        fired = false;
        currentWeapon.Fire(fired);
        currentWeapon.ToggleWeaponReload(false);
        granadeState = !granadeState;
        fireState = !granadeState;
    }

    void OnReload(InputValue value)
    {
        if(idle || fired || !fireState) return;
        
        currentWeapon.ToggleWeaponReload(true);
    }

    void OnFiring(InputValue other)
    {
        if(idle) return;

        fired = other.isPressed;
        if (fireState)
        {
            currentWeapon.Fire(fired);
        }

        if (granadeState && granadeCount > 0)
        {
            if (fired)
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
            ThrowGranade();
        }
    }

    void ThrowGranade()
    {
        granadeCount--;
        granadeCountText.text = granadeCount.ToString();
        audioSource.PlayOneShot(granadeThrowSound);
        GameObject gb = Instantiate(granade, granadePos.position, granadePos.rotation);
        gb.GetComponent<Granade>().ExecuteGranade(time);
        Rigidbody rb = gb.GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * granadeThrowSpeed, ForceMode.Impulse);
    }

    public void WeaponAssigner(WeaponType weaponTypeRef)
    {
        if(!currentWeapon.reloading){ currentWeapon.ToggleWeaponReload(false); }

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
        firstPersonController.enabled = !gamePaused;
        StopShootingOrThrowing();

        StopCoroutine("ReloadTime");
        currentWeapon.Zoom(false);
        GameManager.gameManager.PauseMenu("player");
    }

    public void ContinueState()
    {
        gamePaused = false;
        idle = gamePaused;
        weaponHandle.canScroll = !gamePaused;
        firstPersonController.enabled = !gamePaused;
        ActivateShootingOrThrowing();
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
        if(fireStateEnum == FireStateEnum.CanFire){ ActivateShootingOrThrowing(); }
        else{ StopShootingOrThrowing(); }
    }

    public void StopShootingOrThrowing()
    {
        Cursor.lockState = CursorLockMode.None;
        crossHair.SetActive(false);
        if(fireState) { previousfireState = fireState; currentWeapon.fired = false; fireState = false; }
        if(granadeState) { previousGranadeState = granadeState; granadeState = false; }
    }

    void ActivateShootingOrThrowing()
    {
        Cursor.lockState = CursorLockMode.Locked;
        crossHair.SetActive(true);
        if(previousfireState) { previousfireState = fireState; fireState = true; }
        if(previousGranadeState) { previousGranadeState = granadeState; granadeState = true; }
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

    public void StopAll()
    {
        idle = true;
        StopShootingOrThrowing();
        canZoom = false;
        SetScope(false); 
    }
}
