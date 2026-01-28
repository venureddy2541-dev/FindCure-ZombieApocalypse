using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyAttackTransition : MonoBehaviour
{
    GameObject targetRef;
    IsAlive isAlive;
    Collider[] cols;

    public void ChangingObject(EnemySpawner[] enemySpawners , GameObject target,float stopDistance)
    {
        targetRef = target;
        isAlive = target.GetComponent<IsAlive>();
        cols = target.GetComponents<Collider>();

        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            enemySpawner.attackPos = target;
            foreach(Enemy enemy in enemySpawner.enemies)
            {
                enemy.ChangeMountedObject(target,isAlive,stopDistance,cols);
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
