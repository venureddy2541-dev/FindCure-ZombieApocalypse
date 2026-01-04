using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ScreenOnButton : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject screen;
    [SerializeField] GameObject crossHair;
    [SerializeField] CinemachineCamera screenCam;
    [SerializeField] GameObject player;
    PlayerInput playerIS;
    PlayerFiring playerFiring;
    WeaponHandle weaponHandle;

    void Awake()
    {
        playerIS = player.GetComponent<PlayerInput>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            crossHair.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            crossHair.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
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
