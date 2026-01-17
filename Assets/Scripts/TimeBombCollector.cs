using UnityEngine;

public class TimeBombCollector : MonoBehaviour
{
    [SerializeField] AudioClip ac;
    [SerializeField] GameObject plantArea1;
    [SerializeField] GameObject plantArea2;
    [SerializeField] GameObject plantArea3;
    [SerializeField] GameObject plantArea4;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            MessageBox.messageBox.PressentMessage("Use the TimeBoms and plant them near the Portel. Press x To Plant",ac);
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            playerManager.timeBombCount = 4;
            playerManager.bombPlantAreas.Add(plantArea1);
            playerManager.bombPlantAreas.Add(plantArea2);
            playerManager.bombPlantAreas.Add(plantArea3);
            playerManager.bombPlantAreas.Add(plantArea4);
            Destroy(gameObject);
        }
    }
}
