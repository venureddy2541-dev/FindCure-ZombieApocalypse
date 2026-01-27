using UnityEngine;

public class GiantZombie : Enemy
{
    BloodSplit bloodSplit;

    protected override void Start()
    {
        base.Start();
        bloodSplit = GetComponentInChildren<BloodSplit>();
    }

    protected override void Behaviour()
    {
        SpecialPower();
    }

    void SpecialPower()
    {
        if(exactDistance <= 10 && exactDistance >= 4)
        {
            enemyAnimator.SetFloat("WalkIndex",0);
            enemyAnimator.SetBool("running", false);
            navMesh.speed = 0f;
            transform.LookAt(playerMountedObject.transform);
            bloodSplit.transform.LookAt(playerMountedObject.transform);
            bloodSplit.AcidAttack(exactDistance);
        }
        else
        {
            bloodSplit.StopAttack();
            ChaseTarget();
        }
    }

    protected override void StopEverything()
    {
        base.StopEverything();
        bloodSplit.StopAttack();
    }

    protected override int CurrentAttackType()
    {
        return Random.Range(1,2);
    }

    protected override void Crawl()
    {
        //Cant Crawl
    }
}
