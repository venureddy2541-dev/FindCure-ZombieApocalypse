using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyAttackTransition : MonoBehaviour
{
    EnemyTarget targetNameRef;
    GameObject targetPosRef;

    public void ChangingObject(EnemySpawner[] enemySpawners , EnemyTarget targetName , GameObject targetPos,float stopDistance)
    {
        targetNameRef = targetName;
        targetPosRef = targetPos;
        IsAlive isAlive = targetPos.GetComponent<IsAlive>();

        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            enemySpawner.attackPos = targetPos;
            foreach(Enemy enemy in enemySpawner.enemies)
            {
                enemy.stopValueRef = stopDistance;
                enemy.playerMountedObject = targetPos;
                enemy.AliveChanger = isAlive;
            }
        }

        StartCoroutine("ChangeTag",enemySpawners);
    }

    IEnumerator ChangeTag(EnemySpawner[] enemySpawners)
    {
        yield return new WaitForSeconds(1f);

        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            foreach(Enemy enemy in enemySpawner.enemies)
            {
                Animator enemyAnimatorRef = enemy.enemyAnimator;
                if(enemyAnimatorRef.gameObject.activeInHierarchy)
                {
                    enemyAnimatorRef.SetFloat("AttackType",0);
                }
                enemy.GetComponentInChildren<DamageManager>().enemyTarget = targetNameRef;
            }
        }
    }
}
