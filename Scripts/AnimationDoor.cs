using UnityEngine;
using UnityEngine.Playables;

public class AnimationDoor : MonoBehaviour
{
    AudioSource doorSound;
    [SerializeField] GameObject shield;
    [SerializeField] PlayableDirector timeline;
    bool isPlayed = true;

    void Awake()
    {
        doorSound = GameObject.FindWithTag("SwitchAudio").GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isPlayed)
        {
            doorSound.loop = true;
            doorSound.Play();
            isPlayed = false;
            timeline.Play();
        }
    }

    public void AnimDoorShield()
    {
        doorSound.loop = false;
        doorSound.Stop();
        shield.SetActive(false);
    }
}
