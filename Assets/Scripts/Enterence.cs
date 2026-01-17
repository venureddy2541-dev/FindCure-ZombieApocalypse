using UnityEngine;

public class Enterence : MonoBehaviour
{
    [SerializeField] AudioClip ac;
    [SerializeField] string msg;
    public bool triggered = true;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && triggered)
        {
            triggered = false;
            MessageBox.messageBox.PressentMessage(msg,ac);
        }
    }
}
