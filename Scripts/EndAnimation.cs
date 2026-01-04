using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.Video;

public class EndAnimation : MonoBehaviour
{
    PlayableDirector pd;
    [SerializeField] GameObject playerUI;
    [SerializeField] GameObject collectButton;
    [SerializeField] GameObject cam;
    [SerializeField] CinemachineImpulseSource cis;
    [SerializeField] GameObject vaccine;
    [SerializeField] GameObject vaccineTimeLine;
    [SerializeField] GameObject hand;
    GameObject player;
    [SerializeField] AudioSource blastAudio;
    [SerializeField] AudioClip glassBrokenClip;
    [SerializeField] int speed;
    [SerializeField] TMP_Text endTitle;
    [SerializeField] Volume volume;
    Vignette vignette;

    void Start()
    {
        pd = GetComponent<PlayableDirector>();
    }

    public void ParentingVaccine()
    {
        vaccine.transform.parent = hand.transform;
    }

    public void Blast()
    {
        cis.GenerateImpulse();
        blastAudio.Play();
        blastAudio.PlayOneShot(glassBrokenClip);
        vaccine.transform.position = vaccineTimeLine.transform.position;
        vaccine.transform.rotation = vaccineTimeLine.transform.rotation;
        vaccine.transform.parent = vaccineTimeLine.transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            player = other.gameObject;
            collectButton.SetActive(true);
            other.GetComponent<PlayerFiring>().StopShootingOrThrowing();  
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            collectButton.SetActive(false);
            other.GetComponent<PlayerFiring>().ActivateShootingOrThrowing();
        }
    }

    public void Collect()
    {
        if(GameManager.gameManager.GamePause) { return; }

        Cursor.visible = false;
        playerUI.SetActive(false);
        collectButton.SetActive(false);
        player.SetActive(false); 
        cam.SetActive(true);
        pd.Play();
    }

    public void End()
    {
        StartCoroutine("Blinking");
    }

    IEnumerator Blinking()
    {
        while (vignette == null)
        {
            volume.profile.TryGet(out vignette);
            yield return null;
        }
        
        vignette.color.value = Color.black;
        
        float increment = 0f;
        float currentIntesity;
        float lerp;
        float[] eyeCLosing = {1f,0.5f,1f}; 

        for(int i = 0;i<3;i++)
        {
            lerp = 0f;
            currentIntesity = increment;
            increment = eyeCLosing[i];
            while (lerp < 1f)
            {
                lerp += Time.deltaTime; 
                vignette.intensity.value = Mathf.Lerp(currentIntesity,increment,lerp);
                yield return null;
            }
            yield return new WaitForEndOfFrame();
        }

        endTitle.gameObject.SetActive(true);
        Material matInstance = endTitle.fontMaterial;

        yield return new WaitForSeconds(1f);

        lerp = 0;
        while(lerp < 1)
        {
            lerp += Time.deltaTime;
            matInstance.SetFloat(ShaderUtilities.ID_FaceDilate,Mathf.Lerp(-1,0,lerp));
            endTitle.fontMaterial =  matInstance;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        endTitle.gameObject.SetActive(false);
        vignette.intensity.value = 0;
        SceneLoader.sceneLoader.videoPlayer.Play();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameManager.gameManager.NewGameMenu();
    }
}

