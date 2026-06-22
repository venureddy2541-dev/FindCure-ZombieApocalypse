using UnityEngine;

public class Helper : MonoBehaviour
{
    public int level;
    [SerializeField] GameObject animatorDoor1;
    [SerializeField] GameObject animatorDoor2;
    MusicPlayer musicPlayer;

    void Awake()
    {
        GameObject gb = GameObject.FindWithTag("MusicPlayer");
        if(gb) gb.TryGetComponent(out musicPlayer);
    }
    
    public void SpawnersDead()
    {
        ResetMusic();
        animatorDoor1.SetActive(true);
        animatorDoor2.SetActive(true);
    }

    public void ResetMusic()
    {
        musicPlayer.ResetMusicPlayer();
    }
}
