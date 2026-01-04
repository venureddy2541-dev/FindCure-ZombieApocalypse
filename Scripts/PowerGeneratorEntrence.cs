using UnityEngine;

public class PowerGeneratorEntrence : MonoBehaviour
{
    MessageBox messageBox;
    [SerializeField] AudioClip ac;
    
    bool triggered = true;

    void Awake()
    {
        messageBox = GameObject.FindWithTag("MessageBox").GetComponent<MessageBox>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && triggered)
        {
            triggered = false;
            messageBox.PressentMessage("Only one of the three generators is online:\nPower will shut down every 60 seconds for 20 seconds",ac);
        }
    }
}
