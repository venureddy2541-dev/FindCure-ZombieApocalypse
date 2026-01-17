using UnityEngine;

public class PowerGeneratorEntrence : MonoBehaviour
{
    [SerializeField] AudioClip ac;
    
    bool triggered = true;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && triggered)
        {
            triggered = false;
            MessageBox.messageBox.PressentMessage("Only one of the three generators is online:\nPower will shut down every 60 seconds for 20 seconds",ac);
        }
    }
}
