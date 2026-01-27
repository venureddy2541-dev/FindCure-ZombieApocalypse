using UnityEngine;

public class ActivateSpawners : MonoBehaviour
{
    [SerializeField] GameObject EnemySpawners;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && EnemySpawners != null)
        {
            EnemySpawners.SetActive(true);
        }
    }
}
