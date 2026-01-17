using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class MainLock : MonoBehaviour
{
    [SerializeField] GameObject screen;
    [SerializeField] TMP_Text screenText;
    [SerializeField] GameObject lasers;
    [SerializeField] GameObject Cam;
    [SerializeField] GameObject playerHands;
    [SerializeField] GameObject playerUI;
    [SerializeField] GameObject scanButton;
    [SerializeField] AudioClip accessGranted;
    [SerializeField] AudioSource switchAS;
    [SerializeField] AudioClip switchAC;
    GameObject player;
    PlayableDirector pd;
    bool triggered = false;

    void Start()
    {
        pd = GetComponent<PlayableDirector>();
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && !triggered)
        {
            Cursor.lockState = CursorLockMode.None;
            other.GetComponent<PlayerManager>().ToggleShootingOrThrowing(FireStateEnum.CantFire);
            player = other.gameObject;
            scanButton.SetActive(true);     
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            other.GetComponent<PlayerManager>().ToggleShootingOrThrowing(FireStateEnum.CanFire);
            scanButton.SetActive(false);     
        }
    }

    public void Scan()
    {
        if(GameManager.gameManager.GamePause) { return; }
        
        switchAS.PlayOneShot(switchAC);
        scanButton.SetActive(false);
        playerUI.SetActive(false);
        triggered = true;
        Cam.SetActive(true);
        player.SetActive(false);
        Invoke("WaitTime",2f); 
    } 

    void WaitTime()
    {
        playerHands.SetActive(true);
        pd.Play();
    }

    public void Pressed()
    {
        switchAS.PlayOneShot(switchAC);
        screenText.text = "";
    }

    public void End()
    {
        screen.GetComponent<Image>().color = Color.green;
        MessageBox.messageBox.PressentMessage("access granted",accessGranted);
        StartCoroutine(Blinker());
    }

    IEnumerator Blinker()
    {
        bool blinker = true;
        for(int i = 0;i<5;i++)
        {
            blinker = !blinker;
            lasers.SetActive(blinker);
            yield return new WaitForSeconds(0.05f);
        }
        playerHands.SetActive(false);
        Cam.SetActive(false);
        playerUI.SetActive(true);
        player.SetActive(true);
    }
}
