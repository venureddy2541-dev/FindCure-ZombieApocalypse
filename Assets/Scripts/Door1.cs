using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.Playables;

public class Door1 : MonoBehaviour
{
    [SerializeField] GameObject lights;
    [SerializeField] GameObject door;
    CinemachineImpulseSource groundShake;
    bool triggered = true;
    AudioSource groundAudios;
    [SerializeField] AudioClip wallMovingSound;
    PlayableDirector pd;

    void Start()
    {
        pd = door.GetComponent<PlayableDirector>();
        groundAudios = GetComponent<AudioSource>();
        groundShake = GetComponent<CinemachineImpulseSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && triggered)
        {
            groundAudios.Play();
            groundAudios.PlayOneShot(wallMovingSound);
            triggered = false;
            groundShake.GenerateImpulse();
            lights.SetActive(false);
            pd.Play();
        }
    }
}
