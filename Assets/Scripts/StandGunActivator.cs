using UnityEngine;

public class StandGunActivator : MonoBehaviour
{
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
            player = other.gameObject;
            Cursor.lockState = CursorLockMode.None;
            playerManager = other.GetComponent<PlayerManager>();
            playerHealth = other.GetComponent<PlayerHealth>();
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

        if(playerManager.GamePaused || /*!playerManager.reloading*/ standGun.gunHealthRef <= 0){ return; }

        standGun.AssignPlayer(player);
        playerHealth.ActivateStandGunMode();
        standGun.ActivateStandGunMode();
    }
}
