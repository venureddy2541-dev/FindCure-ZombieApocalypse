using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Playables;

public class PasswordChecker : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] GameObject door;
    [SerializeField] Waves waves;
    [SerializeField] GameObject nextStage;

    [SerializeField] AudioSource doorAudios;
    [SerializeField] AudioClip switchSound;

    [SerializeField] TMP_Text screenText;
    TMP_Text passText;

    string password;
    public string Password;
    int i = 0;
    int maxKeys = 4;

    bool opened = true;


    void Awake()
    {
        passText = GameObject.FindWithTag("PassCode").GetComponent<TMP_Text>();
        doorAudios = GameObject.FindWithTag("SwitchAudio").GetComponent<AudioSource>();
        Password = Random.Range(1000,10000).ToString();
        waves.key = Password;
        GameManager.gameManager.PassKeyAssigner(index,Password);
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
                passText.text = "";
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

        doorAudios.Play();
        door.GetComponent<PlayableDirector>().Play();
        nextStage.SetActive(true);
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
