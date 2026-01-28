using UnityEngine;

public class StandGunActivator : MonoBehaviour
{
    [SerializeField] GameObject trapDoor;
    [SerializeField] AudioSource standGunAS;
    [SerializeField] AudioClip switchAC;
    PlayerManager playerManager;
    PlayerHealth playerHealth;
    GameObject player;

    [SerializeField] StandGun standGun;

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            if(player == null || player != other.gameObject)
            {
                player = other.gameObject;
                playerManager = other.GetComponent<PlayerManager>();
                playerHealth = other.GetComponent<PlayerHealth>();
            }
            playerManager.ToggleShootingOrThrowing(FireStateEnum.CantFire);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerManager.ToggleShootingOrThrowing(FireStateEnum.CanFire);
        }
    }

    public void StandGun()
    {
        standGunAS.PlayOneShot(switchAC);

        if(playerManager.GamePaused || standGun.gunHealthRef <= 0){ return; }

        trapDoor.SetActive(true);
        standGun.AssignPlayer(player);
        playerHealth.ActivateStandGunMode();
        standGun.ActivateStandGunMode();
    }
}
