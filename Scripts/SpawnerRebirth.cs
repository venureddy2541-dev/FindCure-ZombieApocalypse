using UnityEngine;

public class SpawnerRebirth : MonoBehaviour
{
    public GameObject EnemySpawner;

    public void ReBirth()
    {
        Invoke("RebirthRef",10f);
    }

    void RebirthRef()
    {
        EnemySpawner.SetActive(true);
    }
}
