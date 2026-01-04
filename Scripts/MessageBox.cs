using UnityEngine;
using TMPro;

public class MessageBox : MonoBehaviour
{
    TMP_Text msgText;
    AudioSource audioSource;

    void Start()
    {
        msgText = GetComponent<TMP_Text>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PressentMessage(string msg , AudioClip ac)
    {
        CancelInvoke("ClearMsgBox");
        msgText.text = msg;
        audioSource.clip = ac;
        audioSource.Play();
        Invoke("ClearMsgBox",5);
    }

    void ClearMsgBox()
    {
        msgText.text = null;
    }

    public void CompleteClear()
    {
        audioSource.Stop();
        msgText.text = null;
    }
}
