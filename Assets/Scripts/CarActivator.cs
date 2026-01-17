using UnityEngine;

public class CarActivator : MonoBehaviour
{
    [SerializeField] AudioSource carAS;
    [SerializeField] AudioClip switchAC;
    [SerializeField] GameObject carOptions;
    GameObject player;
    PlayerManager playerManager;
    PlayerHealth playerHealth;
    [SerializeField] Car car;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            if (!car.destroyed && !playerHealth.loopControler)
            {
                player = other.gameObject;
                Cursor.lockState = CursorLockMode.None;
                playerManager = other.GetComponent<PlayerManager>();
                playerManager.ToggleShootingOrThrowing(FireStateEnum.CantFire);
                carOptions.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SetToPlayer();
        }
    }

    public void SetToPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerManager.ToggleShootingOrThrowing(FireStateEnum.CanFire);
        carOptions.SetActive(false);
    }

    public void DriveButton()
    {
        carAS.PlayOneShot(switchAC);
        if (playerManager.GamePaused || !playerHealth.isAlive.alive || playerHealth.loopControler) { return; }

        carOptions.SetActive(false);
        Cursor.visible = false;
        car.AssignPlayer(player);
        playerHealth.AssignCar(car);
        playerHealth.ActivateDriveMode();
        car.ActivateDriveMode();
    }
}
