using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{   
    [SerializeField] Slider musicSlider;
    [SerializeField] TMP_Text musicText;
    [SerializeField] Slider otherSoundsSlider;
    [SerializeField] TMP_Text otherSoundsText;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioMixerSnapshot mute;
    [SerializeField] AudioMixerSnapshot normal;
    [SerializeField] GameObject audioMB;

    [SerializeField] MusicPlayer musicPlayer;
    public MusicPlayer musicPlayerRef { get { return musicPlayer; }}

    public static AudioManager audioManager;

    void Awake()
    {
        if(audioManager != null && audioManager != this)
        {
            Destroy(gameObject);
            return;
        }

        audioManager = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetotherSoundsVolume(otherSoundsSlider.value = PlayerPrefs.GetFloat("GameVolume",1f));
        SetMusicAudioVolume(musicSlider.value = PlayerPrefs.GetFloat("MusicVolume",1f));
    }

    public void MusicSoundSlider(float volume)
    {
        SetMusicAudioVolume(volume);
    }

    public void OtherSoundsSlider(float volume)
    {
        SetotherSoundsVolume(volume);
    }

    void SetotherSoundsVolume(float volume)
    {
        volume = Mathf.Clamp(volume,0.0001f,1f);
        PlayerPrefs.SetFloat("GameVolume",volume);
        otherSoundsText.text = Mathf.RoundToInt(volume*100).ToString();
        volume = Mathf.Log10(volume)*20;
        audioMixer.SetFloat("Sfx",volume);
    }

    void SetMusicAudioVolume(float volume)
    {
        volume = Mathf.Clamp(volume,0.0001f,1f);
        PlayerPrefs.SetFloat("MusicVolume",volume);
        musicText.text = Mathf.RoundToInt(volume*100).ToString();
        volume = Mathf.Log10(volume)*20;
        audioMixer.SetFloat("Music",volume);
    }

    public void SetMute()
    {
        mute.TransitionTo(0f);
    }

    public void SetNormal()
    {
        normal.TransitionTo(0.5f);
    }

    public void AudioManagerButtons(bool activate)
    {
        audioMB.SetActive(activate);
    }
}
