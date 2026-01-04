using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnGenerator : MonoBehaviour
{
    bool triggered = true;
    [SerializeField] GameObject[] spawnersBase;
    public AudioSource blastSounds;
    [SerializeField] AudioClip music;
    MusicPlayer musicPlayer;
    
    void Awake()
    {
        musicPlayer = GameObject.FindWithTag("MusicPlayer").GetComponent<MusicPlayer>();
        for(int i = 0;i<spawnersBase.Length;i++)
        {
            spawnersBase[i].SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggered)
        {
            triggered = false;
            musicPlayer.UpdateMusic(music);
            Invoke("EnemyBaseActivator",2);
        }
    }

    void EnemyBaseActivator()
    {
        for(int i = 0;i<spawnersBase.Length;i++)
        {
            spawnersBase[i].SetActive(true);
        }
    }
}
