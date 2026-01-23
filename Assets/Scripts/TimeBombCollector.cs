using UnityEngine;

public class TimeBombCollector : MonoBehaviour
{
    [SerializeField] AudioClip ac;
    [SerializeField] SpecialOperation specialOperation;

    void Awake()
    {
        if(GameManager.gameManager.transform.Find(gameObject.name))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            MessageBox.messageBox.PressentMessage("Use the TimeBoms and plant them near the Portel. Press x To Plant",ac);

            other.GetComponent<PlayerManager>().specialOperation = specialOperation;
            gameObject.SetActive(false);
        }
    }
}
