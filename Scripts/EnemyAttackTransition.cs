using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyAttackTransition : MonoBehaviour
{
    string targetNameRef;
    GameObject targetPosRef;

    public void ChangingObject(EnemySpawner[] enemySpawners , string targetName , GameObject targetPos,float stopDistance)
    {
        targetNameRef = targetName;
        targetPosRef = targetPos;
        IsAlive isAlive = targetPos.GetComponent<IsAlive>();

        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            enemySpawner.attackPos = targetPos;
            foreach(Enemy enemy in enemySpawner.enemies)
            {
                enemy.stopValue = stopDistance;
                enemy.player = targetPos;
                enemy.AliveChanger = isAlive;
            }
        }

        StartCoroutine("ChangingTag",enemySpawners);
    }

    IEnumerator ChangingTag(EnemySpawner[] enemySpawners)
    {
        yield return new WaitForSeconds(1f);

        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            foreach(Enemy enemy in enemySpawner.enemies)
            {
                Animator enemyAnimatorRef = enemy.enemyAnimator;
                if(enemyAnimatorRef.gameObject.activeInHierarchy)
                {
                    enemyAnimatorRef.SetBool("attack1",false);
                    enemyAnimatorRef.SetBool("attack2",false); 
                    enemyAnimatorRef.SetBool("neckAttack",false);
                }
                enemy.GetComponentInChildren<DamageManager>().aventName = targetNameRef;
            }
        }
    }
}
