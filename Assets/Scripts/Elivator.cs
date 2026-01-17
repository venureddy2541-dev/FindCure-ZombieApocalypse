using UnityEngine;
using System.Collections;

public class Elivator : MonoBehaviour
{
    [SerializeField] AudioSource elivatorAs;
    [SerializeField] Elivator elivator;
    [SerializeField] GameObject elivatorMenu;
    [SerializeField] GameObject lockText;

    [SerializeField] Transform floor1Pos;
    [SerializeField] Transform groundPos;
    [SerializeField] AudioClip lockedAudioClip;
    [SerializeField] AudioClip switchAC;
    [SerializeField] float speed; 
    bool running = false;
    public bool locked = true;
    int floorindex = 1;

    void Awake()
    {
        if(GameManager.gameManager.LevelRef > 5)
        {
            elivator.locked = false;
            lockText.SetActive(false);
            elivatorMenu.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(locked) { MessageBox.messageBox.PressentMessage("Locked",lockedAudioClip); }
            Cursor.lockState = CursorLockMode.None;
            other.GetComponent<PlayerManager>().ToggleShootingOrThrowing(FireStateEnum.CantFire);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            other.GetComponent<PlayerManager>().ToggleShootingOrThrowing(FireStateEnum.CanFire);
        }
    }

    public void Floor1()
    {
        if(GameManager.gameManager.GamePause) { return; }

        elivatorAs.PlayOneShot(switchAC);
        if(!running && floorindex != 1) { running = true; StartCoroutine(Teliport(floor1Pos.position)); }
    }

    public void Ground()
    {
        if(GameManager.gameManager.GamePause) { return; }
        
        elivatorAs.PlayOneShot(switchAC);
        if(!running && floorindex != 2) { running = true; StartCoroutine(Teliport(groundPos.position)); }
    }

    IEnumerator Teliport(Vector3 newEndPos)
    {
        elivatorAs.Play();
        float i = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = newEndPos;
        
        while (i < 1)
        {
            i += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, endPos, i);
            yield return new WaitForEndOfFrame();
        }
        
        elivatorAs.Stop();
        floorindex = (floorindex != 1) ? 1 : 2;
        running = false;
    }
}
