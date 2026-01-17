using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ScreenOnButton : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject screen;
    [SerializeField] CinemachineCamera screenCam;
    [SerializeField] GameObject player;
    PlayerInput playerIS;
    WeaponHandle weaponHandle;

    void Awake()
    {
        playerIS = player.GetComponent<PlayerInput>();
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            other.GetComponent<PlayerManager>().ToggleShootingOrThrowing(FireStateEnum.CantFire);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            other.GetComponent<PlayerManager>().ToggleShootingOrThrowing(FireStateEnum.CanFire);
        }
    }

    public void ScreenOn()
    {
        if(GameManager.gameManager.GamePause) { return; }

        audioSource.Play();
        screenCam.Priority = 20;
        playerIS.enabled = false;
        screen.SetActive(true);
    }
}
