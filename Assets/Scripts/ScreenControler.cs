using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ScreenControler : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] CinemachineCamera screenCam;
    [SerializeField] GameObject player;
    PlayerInput playerIS;
    PlayerFiring playerFiring;
    WeaponHandle weaponHandle;
    int scrollVal = 1;

    void Awake()
    {
        playerIS = player.GetComponent<PlayerInput>();
    }

    public void Next()
    {
        if(GameManager.gameManager.GamePause) { return; }

        audioSource.Play();
        scrollBar.value = (scrollVal <= 1)? scrollVal : scrollVal = 0;
        scrollVal++;
    }

    public void Cancle()
    {
        if(GameManager.gameManager.GamePause) { return; }
        
        audioSource.Play();
        screenCam.Priority = 0;
        playerIS.enabled = true;
        transform.parent.gameObject.SetActive(false);
        scrollVal = 0;
    }
}
