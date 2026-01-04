using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            other.gameObject.GetComponent<PlayerFiring>().StopShootingOrThrowing();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            other.gameObject.GetComponent<PlayerFiring>().ActivateShootingOrThrowing();
        }
    }
}
