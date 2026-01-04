using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    [SerializeField] GameObject nextStage;
    [SerializeField] Enemy[] enemies;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            nextStage.SetActive(true);
            foreach(Enemy enemy in enemies)
            {
                enemy.isProvoked = true;
            }
        }
    }
}
