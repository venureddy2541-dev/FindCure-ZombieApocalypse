using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    AudioSource musicPlayer;
    AudioClip initialMusicClip;
    public AudioClip InitialMusicClip { get { return initialMusicClip; }}
    public AudioClip presentClip;

    void Awake()
    {
        musicPlayer = GetComponent<AudioSource>();
        initialMusicClip = musicPlayer.clip;
        presentClip = initialMusicClip;
    }

    public void UpdateMusic(AudioClip ac)
    {
        presentClip = ac;
        musicPlayer.clip = ac;
        musicPlayer.Play();
    }

    public void ResetMusicPlayer()
    {
        presentClip = initialMusicClip;
        musicPlayer.clip = initialMusicClip;
        musicPlayer.Play();
    }

    public void StopMusic()
    {
        musicPlayer.Stop();
    }
}
