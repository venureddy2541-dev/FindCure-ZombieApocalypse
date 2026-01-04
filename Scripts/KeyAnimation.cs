using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class KeyAnimation : MonoBehaviour
{
    PlayableDirector pd;
    [SerializeField] FinalStagePasswordChecker finalStagePasswordChecker;
    TMP_Text passCode;
    [SerializeField] TMP_Text screen;
    [SerializeField] GameObject Cam;
    [SerializeField] GameObject animPlayer;
    [SerializeField] GameObject getKey;
    [SerializeField] GameObject playerUI;
    [SerializeField] AudioSource switchAS;
    [SerializeField] AudioClip switchAC;
    PlayerFiring playerFiring;
    bool triggered = false;

    void Start()
    {
        passCode = GameObject.FindWithTag("PassCode").GetComponent<TMP_Text>();
        screen.text = finalStagePasswordChecker.Password;
        pd = GetComponent<PlayableDirector>();
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && !triggered)
        {
            Cursor.lockState = CursorLockMode.None;
            playerFiring = other.GetComponent<PlayerFiring>();
            playerFiring.StopShootingOrThrowing();
            getKey.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerFiring.ActivateShootingOrThrowing();
            getKey.SetActive(false);
        }
    }

    public void GetKey()
    {
        if(GameManager.gameManager.GamePause) { return; }
        
        switchAS.PlayOneShot(switchAC);
        triggered = true;
        playerUI.SetActive(false);
        getKey.SetActive(false);
        playerFiring.gameObject.SetActive(false);
        animPlayer.SetActive(true);
        Cam.SetActive(true);
        Invoke("WaitTime",2f);
    }

    void WaitTime()
    {
        pd.Play();
    }

    public void OnComplition()
    {
        passCode.text = "Key : " + finalStagePasswordChecker.Password;
        Cursor.lockState = CursorLockMode.Locked;
        playerUI.SetActive(true);
        playerFiring.ActivateShootingOrThrowing();
        playerFiring.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
