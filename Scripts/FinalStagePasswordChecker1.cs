using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Playables;

public class FinalStagePasswordChecker : MonoBehaviour
{
    [SerializeField] MovingLasers movingLasers;
    [SerializeField] GameObject movingLasersObject;
    [SerializeField] PlayableDirector staticLasersPD;

    [SerializeField] AudioSource doorAudios;
    [SerializeField] AudioClip switchSound;

    [SerializeField] TMP_Text screenText;
    TMP_Text passCode;

    string password;
    public string Password;
    int i = 0;
    int maxKeys = 4;

    bool opened = true;

    void Awake()
    {
        passCode = GameObject.FindWithTag("PassCode").GetComponent<TMP_Text>();
        Password = Random.Range(1000,10000).ToString();
    }

    public void Adding(string key)
    {
        if (GameManager.gameManager.GamePause) { return; }

        doorAudios.PlayOneShot(switchSound);
        if(i<maxKeys)
        {
            password += key;
            screenText.text = password;
            i++;
        }
    }

    public void Enter()
    {
        if (GameManager.gameManager.GamePause) { return; }

        doorAudios.PlayOneShot(switchSound);
        if(opened)
        {
            if(password == Password)
            {
                passCode.text = null;
                opened = false;
                screenText.text = "Opening";
                Open();
            }
            else
            {
                screenText.text = "Incorrect";
            }
        }
    }

    public void Open()
    {
        if (GameManager.gameManager.GamePause) { return; }

        movingLasers.triggered = true;
        movingLasersObject.SetActive(false);
        staticLasersPD.Play();
    } 

    public void Cancle()
    {
        if (GameManager.gameManager.GamePause) { return; }
        
        doorAudios.PlayOneShot(switchSound);
        if(opened)
        {
            i = 0;
            password = null;
            screenText.text = password;
        }
    }
}
