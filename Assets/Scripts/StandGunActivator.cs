using UnityEngine;

public class StandGunActivator : MonoBehaviour
{
    [SerializeField] AudioSource standGunAS;
    [SerializeField] AudioClip switchAC;
    PlayerFiring playerFiring;
    PlayerHealth playerHealth;
    GameObject player;

    [SerializeField] StandGun standGun;

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player = other.gameObject;
            Cursor.lockState = CursorLockMode.None;
            playerFiring = other.GetComponent<PlayerFiring>();
            playerHealth = other.GetComponent<PlayerHealth>();
            playerFiring.StopShootingOrThrowing();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerFiring.ActivateShootingOrThrowing();
        }
    }

    public void StandGun()
    {
        standGunAS.PlayOneShot(switchAC);

        if(playerFiring.GamePaused || !playerFiring.reloading || standGun.gunHealthRef <= 0){ return; }

        standGun.AssignPlayer(player);
        playerHealth.ActivateStandGunMode();
        standGun.ActivateStandGunMode();
    }
}
