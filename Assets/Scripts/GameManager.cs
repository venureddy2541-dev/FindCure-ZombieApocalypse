using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using StarterAssets;
using System;

public class GameManager : MonoBehaviour
{
    public static event Action NewGame;
    [SerializeField] int currency = 300;
    int currencyRef;
    TMP_Text currencyBoard;

    [SerializeField] List<int> Ammo = new List<int>();
    [SerializeField] int healthKitCountRef;
    [SerializeField] int granadeCountRef;

    [SerializeField] Image gameLoadImage;
    [SerializeField] TMP_Text passcodePrinter;
    [SerializeField] Dictionary<int,string> passKeys = new Dictionary<int,string>(5);
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject playerDeathMenu;
    [SerializeField] GameObject newGameMenu;
    [SerializeField] Transform wayPointsHolder;
    [SerializeField] GameObject lift;
    [SerializeField] Enterence[] enterence;
    public List<GameObject> levels;
    GameObject tempWeapon;
    Transform wayPoint;
    [SerializeField] AudioSource audioSource;

    public bool continueState = false;
    private int level = 0;
    public int LevelRef { get { return level; }}
    private bool pause = false;

    public static GameManager gameManager;
    AudioManager audioManager;
    MusicPlayer musicPlayer;

    public GameObject player;
    PlayerManager playerManager;
    PlayerHealth playerHealth;
    WeaponHandle weaponHandle;

    public Car car;
    public StandGun standGun;

    string presentPlayerState = "player";

    void Awake()
    {
        if(gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
            return;
        }

        gameManager = this;
        DontDestroyOnLoad(gameObject);
        player = GameObject.FindWithTag("Player");
        playerManager = player.GetComponent<PlayerManager>();
        playerHealth = player.GetComponent<PlayerHealth>();
        weaponHandle = player.GetComponent<WeaponHandle>();
        audioManager = AudioManager.audioManager;
        musicPlayer = audioManager.musicPlayerRef;
        musicPlayer.ResetMusicPlayer();
        audioManager.SetMute();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded -= LoadedScene;
        SceneManager.sceneLoaded += LoadedScene;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadedScene;
    }

    void LoadedScene(Scene scene , LoadSceneMode mode)
    {
        if(continueState)
        {
            audioManager.SetNormal();
            musicPlayer.ResetMusicPlayer();
            if(level < wayPointsHolder.childCount)
            {
                StartCoroutine(PlayerSpawnWaitTime());
            }

            continueState = false;
        }
    }

    IEnumerator PlayerSpawnWaitTime()
    {
        while (player == null)
        {
            player = GameObject.FindWithTag("Player");
            yield return null;
        }

        playerManager = player.GetComponent<PlayerManager>();
        playerHealth = player.GetComponent<PlayerHealth>();
        weaponHandle = player.GetComponent<WeaponHandle>();
        
        PlacingPlayer();
        playerManager.WeaponActivator();
        currencyBoard = GameObject.FindWithTag("CurrencyText").GetComponent<TMP_Text>();

        if(level > 4)
        {
            tempWeapon = GameObject.FindWithTag("PickableWeapon");
            tempWeapon.transform.parent = transform;
            tempWeapon.GetComponent<FlameThrowerAssignier>().WeaponAssigner();
        }
        
        if(level > 0 && level < 6) 
        { 
            passcodePrinter.text = "code : " + passKeys[level - 1]; 
        }

        if(level > 0) { UpdateInventory(); }
        else { currency = 300; UpdateCash(0); weaponHandle.UpdateInitialAmmo();}
    }

    void PlacingPlayer()
    {
        wayPoint = wayPointsHolder.GetChild(level).transform;
        player.transform.position = wayPoint.position;
        player.transform.rotation = wayPoint.rotation;
    }

    void UpdateInventory()
    {
        currency = 0;
        UpdateCash(currencyRef);
        playerHealth.UpdateHealthText(healthKitCountRef);
        playerManager.UpdateGranadeText(granadeCountRef);
        weaponHandle.UpdateAmmoSizes(Ammo);
    }

    public bool GamePause
    {
        get { return pause; }
    }

    public void PlayAgain()
    { 
        NewGame?.Invoke();
        SceneLoader.sceneLoader.videoPlayer.Stop();
        audioManager.AudioManagerButtons(false);
        musicPlayer.StopMusic();
        ButtonSound();
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        SceneLoader.sceneLoader.SceneLoadManager(SceneManager.GetActiveScene().buildIndex,false);

        Destroy(gameObject);
    }

    public void LoadPreviousStage()
    {
        passcodePrinter.text = null;
        musicPlayer.StopMusic();
        ButtonSound();
        for(int i=level;i<enterence.Length;i++)
        {
            enterence[i].triggered = true;
        }
        playerDeathMenu.SetActive(false);
        passKeys.Clear();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        continueState = true;
        SceneLoader.sceneLoader.SceneLoadManager(SceneManager.GetActiveScene().buildIndex,false);
    }

    public void BackToMainScene()
    {
        NewGame?.Invoke();
        passcodePrinter.text = null;
        SceneLoader.sceneLoader.videoPlayer.Stop();
        musicPlayer.StopMusic();
        ButtonSound();
        Time.timeScale = 1f;
        playerDeathMenu.SetActive(false);
        pauseMenu.SetActive(false);
        audioManager.AudioManagerButtons(false);
        newGameMenu.SetActive(false);
        SceneLoader.sceneLoader.SceneLoadManager(SceneManager.GetActiveScene().buildIndex - 1,true);
        
        Destroy(gameObject);
    }

    public void Continue()
    {
        ButtonSound();
        Time.timeScale = 1f;
        pause = false;
        pauseMenu.SetActive(false);
        audioManager.AudioManagerButtons(false);
        switch (presentPlayerState)
        {
            case "player" :

                    playerManager.ContinueState();
                    break;

            case "standgun" :

                    Cursor.visible = false;
                    break;

            case "vehical" :

                    Cursor.visible = false;
                    car.ContinueState();
                    break;
        }
    }

    public void PauseMenu(string name)
    {
        presentPlayerState = name;
        Time.timeScale = 0f;
        pause = true;
        pauseMenu.SetActive(true);
        audioManager.AudioManagerButtons(true);
    }
    
    void ButtonSound()
    {
        audioSource.Play();
    }

    public void NewGameMenu()
    {
        newGameMenu.SetActive(true);
    }

    public void DeadMenu()
    {
        if(car) { car.Deactivate(); }
        playerDeathMenu.SetActive(true);
    }

    public void ManageLevel()
    {
        level++;
        
        ManageInventory(playerHealth.HealthKitCount,playerManager.GranadeCount,weaponHandle.CurrentAmmoSizes(),currency);
    }

    void ManageInventory(int healthKitCount,int granadeCount,List<int> ammoSize,int presentCurrency)
    {
        Ammo.Clear();
        currencyRef = presentCurrency;
        granadeCountRef = granadeCount;
        healthKitCountRef = healthKitCount;
        foreach(int ammoRef in ammoSize)
        {
            Ammo.Add(ammoRef);
        }
    }

    public void PassKeyAssigner(int index,string pass)
    {
        passKeys.Add(index,pass);
    }

    public void GameStart()
    {
        gameLoadImage.gameObject.SetActive(true);
        StartCoroutine("StartCoroutine");
    }

    IEnumerator StartCoroutine()
    {
        StarterAssetsInputs starterAssetsInputs = player.GetComponent<StarterAssetsInputs>();
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        playerInput.enabled = false;
        starterAssetsInputs.sprint = false;

        yield return new WaitForSeconds(1f);

        float i = 0;
        float val = 1;
        bool loading = true;
        bool unloading = false;
        while(i<val)
        {
            if(loading)
            {
                i += Time.deltaTime;
                gameLoadImage.fillAmount = i;
                if(i >= val)
                {
                    gameLoadImage.fillAmount = 1;
                    loading = false;

                    yield return new WaitForSeconds(1f);

                    wayPoint = wayPointsHolder.GetChild(0).transform;
                    while(Vector3.Distance(wayPoint.position,player.transform.position) > 0.5f)
                    {
                        player.transform.position = wayPoint.position;
                        player.transform.rotation = wayPoint.rotation;
                        yield return null;
                    }

                    yield return new WaitForSeconds(1f);
                    i = 0;
                    unloading = true;
                    audioManager.SetNormal();
                }
            }

            if(unloading)
            {
                val -= Time.deltaTime;
                gameLoadImage.fillAmount = val;
            }

            yield return null;
        }

        playerManager.WeaponActivator();
        currencyBoard = GameObject.FindWithTag("CurrencyText").GetComponent<TMP_Text>();
        weaponHandle.UpdateInitialAmmo();
        UpdateCash(0);
        gameLoadImage.gameObject.SetActive(false);
        playerInput.enabled = true;
    }

    public void UpdateCash(int points)
    {
        currency += points;
        currencyBoard.text = "currency : " + currency.ToString() + "$";
    }

    public int Currency
    {
        get { return currency; }
    }

    public void UpdateInventoryBetweenLevels(int healthKitCount,int granadeCount,int presentCurrency)
    {
        healthKitCountRef += healthKitCount;
        granadeCountRef += granadeCount;
        currencyRef -= presentCurrency;
    }
}