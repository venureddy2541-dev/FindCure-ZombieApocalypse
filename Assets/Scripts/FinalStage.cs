using UnityEngine;

public class FinalStage : MonoBehaviour
{
    [SerializeField] Elivator elivator;
    [SerializeField] GameObject elivatorMenu;
    [SerializeField] GameObject lockText;
    [SerializeField] GameObject indiCator;
    [SerializeField] GameObject generatorPath;
    MessageBox messageBox;
    [SerializeField] AudioClip ac;
    [SerializeField] AudioClip firingRobotsActivated;
    [SerializeField] AudioClip elivatorActive;
    public int count = 12;
    [SerializeField] GameObject FiringRobots;
    [SerializeField] GameObject Player;
    bool isTriggered = true;
    GameObject gameManager;

    void Awake()
    {
        gameManager = GameObject.FindWithTag("Manager");
        if(gameManager.transform.Find(gameObject.name) != null)
        {
            Destroy(gameObject);
        }
        messageBox = GameObject.FindWithTag("MessageBox").GetComponent<MessageBox>();
    }

    public void NormalRobotsDeadCount()
    {
        count--;
        if(count == 4)
        {
            messageBox.PressentMessage("Warning: Autonomous defense systems activated",firingRobotsActivated);
            //count = 4;
            PlayerHealthIncreaser(Player,1000);
            Invoke("AcctivateFiringRobots",5);
        }

        if(count == 0)
        {
            GameManager.gameManager.ManageLevel();
            transform.parent = gameManager.transform;
            GameManager.gameManager.levels.Add(gameObject);
            messageBox.PressentMessage("All Clear - Proceed to the Elevator",elivatorActive);

            elivator.locked = false;
            lockText.SetActive(false);
            indiCator.SetActive(true);
            elivatorMenu.SetActive(true);
        }
    }  

    void AcctivateFiringRobots()
    {
        FiringRobots.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isTriggered)
        {
            isTriggered = false;
            Player = other.gameObject;
            PlayerHealthIncreaser(other.gameObject,500);
            messageBox.PressentMessage("Power's out. Follow the path to Start the generator.",ac);
            generatorPath.SetActive(true);
        }
    }

    void PlayerHealthIncreaser(GameObject other,int health)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        playerHealth.slider.maxValue = health;
        playerHealth.playerHealth = health;
        playerHealth.playerHealthRef = health;
        playerHealth.HealthConditions(0);
    }
}
