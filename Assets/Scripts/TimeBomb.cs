using UnityEngine;
using System.Collections;
using TMPro;

public class TimeBomb : MonoBehaviour
{
    [SerializeField] GameObject rangeObject;
    public GameObject blastParticle;
    public AudioSource blastAudioSource;
    [SerializeField] AudioClip beepAudio;
    public TMP_Text timerText;
    [SerializeField] float radius = 20f;
    [SerializeField] float Speed = 2f;
    int timeBombDamage = 10000;

    Vector3 startScale = new Vector3(0f,0.05f,0f);
    Vector3 endScale = new Vector3(40f,0.05f,40f);

    void Awake()
    {
        blastAudioSource = GameObject.FindWithTag("BlastAudio").GetComponent<AudioSource>();
    }
    
    void Start()
    {
        StartCoroutine(Timer());
        StartCoroutine(RangeLerp());
    }

    IEnumerator RangeLerp()
    {
        float scaleVal = 0f;
        while(scaleVal <= 1f)
        {
            scaleVal += Time.deltaTime*Speed;
            rangeObject.transform.localScale = Vector3.Lerp(startScale,endScale,scaleVal);
            yield return null;
            if(scaleVal >= 1f)
            {
                Vector3 tempScale = startScale;
                startScale = endScale;
                endScale = tempScale;
                scaleVal = 0f;
            }
        }
    }

    IEnumerator Timer()
    {
        int timer = 10;
        while(timer > 0)
        {
            timer -= 1;
            timerText.text = timer.ToString();
            blastAudioSource.PlayOneShot(beepAudio);
            yield return new WaitForSeconds(1);
        }

        timerText.text = null;
        Collider[] cols = Physics.OverlapSphere(transform.position,radius);
        foreach(Collider hitCol in cols)
        {
            if(hitCol.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hitCol.GetComponent<PlayerHealth>();
                if(playerHealth) playerHealth.TakeDamage(timeBombDamage);
            }

            if(hitCol.CompareTag("Robot"))
            {
                hitCol.GetComponentInParent<RoboBomb>().TakeDamage(timeBombDamage);
            }

            if(hitCol.CompareTag("RobotGenerator"))
            {
                hitCol.GetComponent<RobotGenerator>().TakeDamage(timeBombDamage);
            }
        }
        Instantiate(blastParticle,transform.position,Quaternion.identity);
        blastAudioSource.Play();
        Destroy(gameObject);
    }
}
