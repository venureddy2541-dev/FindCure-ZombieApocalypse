using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyAttackTransition : MonoBehaviour
{
    GameObject targetRef;

    public void ChangingObject(EnemySpawner[] enemySpawners , GameObject target,float stopDistance)
    {
        targetRef = target;
        IsAlive isAlive = target.GetComponent<IsAlive>();

        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            enemySpawner.attackPos = target;
            foreach(Enemy enemy in enemySpawner.enemies)
            {
                enemy.ChangeMountedObject(target,isAlive,stopDistance);
            }
        }

        StartCoroutine("ChangeTag",enemySpawners);
    }

    IEnumerator ChangeTag(EnemySpawner[] enemySpawners)
    {
        yield return new WaitForSeconds(0.5f);

        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            foreach(Enemy enemy in enemySpawner.enemies)
            {
                enemy.ChangeTarget();
            }
        }
    }
}
