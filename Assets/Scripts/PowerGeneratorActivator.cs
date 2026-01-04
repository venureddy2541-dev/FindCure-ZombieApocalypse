using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerGeneratorActivator : MonoBehaviour
{
    [SerializeField] GameObject generatorScreen;
    [SerializeField] GameObject Lights;

    [SerializeField] Renderer[] Renderers;
    [SerializeField] Material glowMaterial;
    [SerializeField] Material offMaterial;

    [SerializeField] Image powerStatus;
    [SerializeField] GameObject nextStage;
    [SerializeField] GameObject timeBomb;
    [SerializeField] AudioClip switchAC;
    [SerializeField] AudioSource generatorSound;
    bool Exit = false;
    bool Enter = false;
    bool isTriggered = false;
    public bool isActivated = false;
    bool GeneratorRunning = true;
    int timeDelay;
    bool executed = false;
    FinalStage finalStage;

    void Awake()
    {
        finalStage = GetComponentInParent<FinalStage>();
    }

    public void Activator()
    {
        if(GameManager.gameManager.GamePause && !Enter) { return; }

        generatorSound.PlayOneShot(switchAC);
        if(isActivated) return;

        Enter = true;
        isActivated = true;
        timeBomb.SetActive(GeneratorRunning);
        Lights.SetActive(GeneratorRunning);
        nextStage.SetActive(GeneratorRunning);
        powerStatus.gameObject.SetActive(true);
        StartCoroutine(ShutDownAndRestart());
    }

    IEnumerator ShutDownAndRestart()
    {
        while(true)
        {
            if(GeneratorRunning) 
            {
                generatorSound.Play();
                SwitchOnLights();
                timeDelay = 60;
                GeneratorTimeStatus();
            }
            else 
            { 
                generatorSound.Stop(); 
                SwitchOffLights();
                timeDelay = 15; 
                GeneratorTimeStatus();
            }

            yield return new WaitForSeconds(timeDelay);  

            GeneratorRunning = !GeneratorRunning;

            for(int i = 0;i<4;i++)
            {
                GeneratorRunning = !GeneratorRunning;

                if(GeneratorRunning) 
                { 
                    SwitchOnLights();
                }
                else
                { 
                    SwitchOffLights();
                }

                Lights.SetActive(GeneratorRunning);
                float blinkTime = Random.Range(0f,0.2f);
                yield return new WaitForSeconds(blinkTime);
            }
        }
    }

    void GeneratorTimeStatus()
    {
        if(finalStage.count <= 0) 
        { 
            if(!executed)
            {
                executed = true;
                StopCoroutine("TimeStatus"); 
                powerStatus.gameObject.SetActive(false); 
            }
            return; 
        }
        StartCoroutine("TimeStatus");
    }

    IEnumerator TimeStatus()
    {
        float i = 0;
        float statusRef;
        float timeDelayRef = Mathf.RoundToInt(timeDelay);

        while(i < timeDelayRef)
        {
            if(GeneratorRunning) 
            { 
                i += Time.deltaTime;
                statusRef = (1-(i/timeDelayRef));
            }
            else 
            { 
                timeDelayRef -= Time.deltaTime;
                i = (1 - (timeDelayRef / timeDelay));
                statusRef = i;
            }

            powerStatus.fillAmount = statusRef;
            yield return null;
        }
    }

    void SwitchOnLights()
    {
        foreach (Renderer r in Renderers)
        {
            r.sharedMaterial = glowMaterial;
        }
    }

    void SwitchOffLights()
    {
        foreach (Renderer r in Renderers)
        {
            r.sharedMaterial = offMaterial;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && !Enter)
        {
            Cursor.lockState = CursorLockMode.None;
            other.GetComponent<PlayerFiring>().StopShootingOrThrowing();
            if(!isTriggered)
            {
                isTriggered = true;
                StartCoroutine(Blinking());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && !Exit)
        {
            if(Enter == true){ Exit = true; }
            Cursor.lockState = CursorLockMode.Locked;
            other.GetComponent<PlayerFiring>().ActivateShootingOrThrowing();
        }
    }

    IEnumerator Blinking()
    {
        yield return new WaitForSeconds(1f);
        for(int i = 0;i<4;i++)
        {
            float timeGap = Random.Range(0f,0.5f);
            yield return new WaitForSeconds(timeGap);
            generatorScreen.SetActive(!generatorScreen.activeSelf);
            
            yield return new WaitForSeconds(0.05f);
            generatorScreen.SetActive(!generatorScreen.activeSelf);
        }
        generatorScreen.SetActive(true);
    }
}
