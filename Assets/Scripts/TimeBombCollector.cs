using UnityEngine;

public class TimeBombCollector : MonoBehaviour
{
    [SerializeField] MessageBox messageBox;
    [SerializeField] AudioClip ac;
    [SerializeField] GameObject plantArea1;
    [SerializeField] GameObject plantArea2;
    [SerializeField] GameObject plantArea3;
    [SerializeField] GameObject plantArea4;

    void Awake()
    {
        messageBox = GameObject.FindWithTag("MessageBox").GetComponent<MessageBox>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            messageBox.PressentMessage("Use the TimeBoms and plant them near the Portel. Press x To Plant",ac);
            PlayerFiring playerFiring = other.GetComponent<PlayerFiring>();
            playerFiring.timeBombCount = 4;
            playerFiring.bombPlantAreas.Add(plantArea1);
            playerFiring.bombPlantAreas.Add(plantArea2);
            playerFiring.bombPlantAreas.Add(plantArea3);
            playerFiring.bombPlantAreas.Add(plantArea4);
            Destroy(gameObject);
        }
    }
}
