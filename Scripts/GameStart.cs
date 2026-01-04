using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameStart : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject audioMB;
    [SerializeField] EnemySpawner enemySpawner1;
    [SerializeField] EnemySpawner enemySpawner2;
    [SerializeField] Enemy enemy1;
    [SerializeField] Enemy enemy2;
    [SerializeField] Enemy enemy3;
    [SerializeField] Enemy enemy4;
    [SerializeField] Robot robot;
    [SerializeField] RoboBomb roboBomb;
    [SerializeField] WalkingRobots walkingRobots;
    [SerializeField] TMP_Text modeIndicatorText;
    [SerializeField] VideoPlayer videoPlayer;
    AudioSource audioSource;

    void Awake()
    {
        modeIndicatorText.text = "default(EASY)";
        audioSource = GetComponent<AudioSource>();
        Health(15000,5000,50,100,150,200,500,50,50);
    }

    public void start()
    {
        menu.SetActive(false);
        audioMB.SetActive(false);
        videoPlayer.Stop();
        Cursor.lockState = CursorLockMode.Locked;
        PressAudio();
        sceneLoader.SceneLoadManager(SceneManager.GetActiveScene().buildIndex + 1,false);
    }

    public void exit()
    {
        PressAudio();
        Application.Quit();
    }

    public void easy()
    {
        modeIndicatorText.text = "EASY";
        PressAudio();
        Health(15000,5000,50,100,150,200,500,50,50);
    }

    public void medium()
    {
        modeIndicatorText.text = "MEDIUM";
        PressAudio();
        Health(30000,10000,100,150,200,250,1000,100,100);
    }

    public void hard()
    {
        modeIndicatorText.text = "HARD";
        PressAudio();
        Health(45000,15000,150,200,250,300,1500,150,150);
    }

    void PressAudio()
    {
        audioSource.Play();
    }

    void Health(int spawnerHealth1,int spawnerHealth2,int enemyHealht1,int enemyHealht2,int enemyHealht3,int enemyHealht4,int robotHealth,int roboBombHealth,int walkingRobotsHealth)
    {
        enemySpawner1.Health = spawnerHealth1;
        enemySpawner2.Health = spawnerHealth2;

        enemy1.Health = enemyHealht1;
        enemy2.Health = enemyHealht2;
        enemy3.Health = enemyHealht3;
        enemy4.Health = enemyHealht4;

        robot.Health = robotHealth;
        roboBomb.Health = roboBombHealth;
        walkingRobots.Health = walkingRobotsHealth;
    }

    public void ActivateButtons()
    {
        menu.SetActive(true);
        audioMB.SetActive(true);
        videoPlayer.Play();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
