using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ZombiesManager : MonoBehaviour
{
    int totalPoolSize = 0;
    [SerializeField] int poolSize = 20;
    [SerializeField] int poolSizeForFrame = 5;
    [SerializeField] GameObject[] zombieVarients = new GameObject[4];
    Dictionary<varient,List<Enemy>> zombies = new Dictionary<varient,List<Enemy>>();
    Dictionary<varient,List<EnemyState>> states = new Dictionary<varient,List<EnemyState>>();
    Dictionary<varient,int> indexes = new Dictionary<varient,int>();
    Dictionary<varient,List<int>> currentActiveZombies = new Dictionary<varient,List<int>>();
    public static ZombiesManager zombiesManager;

    void Awake()
    {
        if(zombiesManager != null && zombiesManager != this)
        {
            Destroy(gameObject);
            return;
        }

        zombiesManager = this;
        DontDestroyOnLoad(this);
        totalPoolSize = poolSize*zombieVarients.Length;
        StartCoroutine(SpawnZombies());
    }

    IEnumerator SpawnZombies()
    {
        AssiginDictionaries();

        int tempPoolSize = 0;
        while(tempPoolSize < poolSize)
        {
            for(int i = tempPoolSize;i<(tempPoolSize + poolSizeForFrame);i++)
            {
                zombies[varient.SkinLessZombie].Add(Instantiate(zombieVarients[0],transform.position,Quaternion.identity,transform).GetComponent<Enemy>());
                states[varient.SkinLessZombie].Add(EnemyState.InActive);
                DisableZombie(zombies[varient.SkinLessZombie][i]);

                zombies[varient.ScarryZombie].Add(Instantiate(zombieVarients[1],transform.position,Quaternion.identity,transform).GetComponent<Enemy>());
                states[varient.ScarryZombie].Add(EnemyState.InActive);
                DisableZombie(zombies[varient.ScarryZombie][i]);

                zombies[varient.FastZombie].Add(Instantiate(zombieVarients[2],transform.position,Quaternion.identity,transform).GetComponent<Enemy>());
                states[varient.FastZombie].Add(EnemyState.InActive);
                DisableZombie(zombies[varient.FastZombie][i]);

                zombies[varient.BigZombie].Add(Instantiate(zombieVarients[3],transform.position,Quaternion.identity,transform).GetComponent<Enemy>());
                states[varient.BigZombie].Add(EnemyState.InActive);
                DisableZombie(zombies[varient.BigZombie][i]);
            }

            tempPoolSize += 5;
            yield return null;
        }
    }

    void DisableZombie(Enemy enemy)
    {   
        enemy.ForceReset();
        enemy.gameObject.SetActive(false);
        enemy.navMesh.enabled = false;
        enemy.enabled = false;
    }

    public Dictionary<varient,List<Enemy>> ReturnEnemies(List<ZombieType> zombieType)
    {
        Dictionary<varient,List<Enemy>> tempDic = new Dictionary<varient, List<Enemy>>();
        tempDic[varient.SkinLessZombie] = new List<Enemy>();
        tempDic[varient.ScarryZombie] = new List<Enemy>();
        tempDic[varient.FastZombie] = new List<Enemy>();
        tempDic[varient.BigZombie] = new List<Enemy>();

        foreach(var item in zombieType)
        {
            for(int i=0;i<item.size;i++)
            {
                bool objectFound = false;
                for(int j=0;j<zombies[item.type].Count;j++)
                {
                    if(states[item.type][indexes[item.type]] == EnemyState.Active)
                    {
                        indexes[item.type]++;
                        indexes[item.type] = (indexes[item.type] + 1)%zombies[item.type].Count;
                    }
                    else
                    {
                        objectFound = true;
                        break;
                    }
                }

                if(objectFound)
                {
                    states[item.type][indexes[item.type]] = EnemyState.Active;
                    currentActiveZombies[item.type].Add(indexes[item.type]);
                    tempDic[item.type].Add(zombies[item.type][indexes[item.type]]);
                    indexes[item.type]++;
                    indexes[item.type] = (indexes[item.type] + 1)%zombies[item.type].Count;
                }
                else { break; }
            }
        }

        return tempDic;
    }

    public void ResetActiveZombies()
    {
        foreach(var item in currentActiveZombies)
        {
            List<int> temp = item.Value;
            for(int i=0;i<temp.Count;i++)
            {
                states[item.Key][temp[i]] = EnemyState.InActive;
                DisableZombie(zombies[item.Key][temp[i]]);
            }
        }

        currentActiveZombies[varient.ScarryZombie] = new List<int>();
        currentActiveZombies[varient.SkinLessZombie] = new List<int>();
        currentActiveZombies[varient.FastZombie] = new List<int>();
        currentActiveZombies[varient.BigZombie] = new List<int>();
    }

    public void ResetZombies()
    {
        foreach(var item in zombies)
        {
            List<Enemy> temp = item.Value;
            for(int i=0;i<temp.Count;i++)
            {
                states[item.Key][i] = EnemyState.InActive;
                DisableZombie(temp[i]);
                temp[i].transform.position = transform.position;
                temp[i].transform.rotation = Quaternion.identity;
            }
        }
    }

    void AssiginDictionaries()
    {
        zombies[varient.SkinLessZombie] = new List<Enemy>(poolSize);
        zombies[varient.ScarryZombie] = new List<Enemy>(poolSize);
        zombies[varient.FastZombie] = new List<Enemy>(poolSize);
        zombies[varient.BigZombie] = new List<Enemy>(poolSize);

        states[varient.SkinLessZombie] = new List<EnemyState>();
        states[varient.ScarryZombie] = new List<EnemyState>();
        states[varient.FastZombie] = new List<EnemyState>();
        states[varient.BigZombie] = new List<EnemyState>();

        currentActiveZombies[varient.SkinLessZombie] = new List<int>();
        currentActiveZombies[varient.ScarryZombie] = new List<int>();
        currentActiveZombies[varient.FastZombie] = new List<int>();
        currentActiveZombies[varient.BigZombie] = new List<int>();

        indexes[varient.SkinLessZombie] = 0;
        indexes[varient.ScarryZombie] = 0;
        indexes[varient.FastZombie] = 0;
        indexes[varient.BigZombie] = 0;
    }

    /*public Enemy GetEnemyByType(varient varientRef)
    {
        Enemy enemy = CheckAndGetZombie(varientRef);
        EnableZombie(enemy);
        return enemy;
    }

    Enemy CheckAndGetZombie(varient varientRef)
    {
        int loopCount = 0;
        while(loopCount < poolSize)
        {
            Enemy enemy = zombies[varientRef][states[varientRef]];
            states[varientRef] = (states[varientRef] + 1)%poolSize;

            if(!enemy.gameObject.activeSelf)
            {
                return enemy;
            }

            loopCount++;
        }

        return null;
    }*/
}
