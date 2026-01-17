using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField] int storeIndex;
    [SerializeField] GameObject playerCroshair;
    [SerializeField] TMP_Text healthKitCountText;
    [SerializeField] TMP_Text granadeCountText;
    [SerializeField] int healthKitCount = 5;
    [SerializeField] int granadeCount = 5;
    int requiredCurrencyForH = 70;
    int requiredCurrencyForG = 50;
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GameManager.gameManager.GetComponent<AudioSource>();
    }

    void Start()
    {
        healthKitCountText.text = healthKitCount.ToString();
        granadeCountText.text = granadeCount.ToString();
    }

    public void HealthKit()
    {
        audioSource.Play();
        if(healthKitCount > 0 && GameManager.gameManager.Currency >= requiredCurrencyForH)
        {
            GameManager.gameManager.UpdateCash(-requiredCurrencyForH);
            healthKitCount--;
            healthKitCountText.text = healthKitCount.ToString();
            PlayerHealth playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
            if(playerHealth != null)
            {
                playerHealth.UpdateHealthText(1);
                if(GameManager.gameManager.LevelRef > storeIndex)
                {
                    GameManager.gameManager.UpdateInventoryBetweenLevels(1,0,requiredCurrencyForH);
                }
            }
        }
    }

    public void Granade()
    {
        audioSource.Play();
        if(granadeCount > 0 && GameManager.gameManager.Currency >= requiredCurrencyForG)
        {
            GameManager.gameManager.UpdateCash(-requiredCurrencyForG);
            granadeCount--;
            granadeCountText.text = granadeCount.ToString();
            PlayerManager playerManager = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
            if(playerManager != null)
            {
                playerManager.UpdateGranadeText(1);
                if(GameManager.gameManager.LevelRef > storeIndex)
                {
                    GameManager.gameManager.UpdateInventoryBetweenLevels(0,1,requiredCurrencyForG);
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            other.gameObject.GetComponent<PlayerManager>().ToggleShootingOrThrowing(FireStateEnum.CantFire);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            other.gameObject.GetComponent<PlayerManager>().ToggleShootingOrThrowing(FireStateEnum.CanFire);
        }
    }
}
