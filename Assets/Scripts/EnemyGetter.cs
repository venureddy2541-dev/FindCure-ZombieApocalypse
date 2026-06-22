using UnityEngine;
using System.Collections.Generic;

public class EnemyGetter : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerMountedObject;
    public Waves level;
    [SerializeField] GameObject spawnpointsParent;
    [SerializeField] List<ZombieType> zombieType = new List<ZombieType>();
    public Dictionary<varient,List<Enemy>> enemies = new Dictionary<varient,List<Enemy>>();

    void Awake()
    {
        foreach(var item in zombieType)
        {
            foreach(Transform point in item.spawnpoints)
            {
                item.positions.Add(point.position);
            }
        }

        if(spawnpointsParent) spawnpointsParent.SetActive(false);
    }

    public void GetEnemies()
    {
        enemies = ZombiesManager.zombiesManager.ReturnEnemies(zombieType);
        PlaceEnemies();
    }

    public List<Enemy> GetEnemiesByList()
    {
        enemies = ZombiesManager.zombiesManager.ReturnEnemies(zombieType);
        List<Enemy> tempList = new List<Enemy>();
        foreach(var item in enemies)
        {
            tempList.AddRange(item.Value);
        }

        return tempList;
    }

    void PlaceEnemies()
    {
        foreach(var item in zombieType)
        {
            for(int i=0;i<item.size;i++)
            {
                Enemy enemy = enemies[item.type][i];
                enemy.level = level;
                enemy.transform.position = item.positions[i];
                enemy.AssiginPlayerData(player,playerMountedObject);
                enemy.navMesh.enabled = true;
                enemy.enabled = true;
                enemy.gameObject.SetActive(true);
            }
        }
    }
}
