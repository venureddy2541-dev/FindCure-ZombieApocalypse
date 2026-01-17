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
            enemyAnimator.SetBool("running", false);
            navMesh.speed = 0f;
            transform.LookAt(player.transform);
            bloodSplit.transform.LookAt(player.transform);
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

    protected override float CurrentAttackType()
    {
        return Random.Range(1f,2f);
    }
}
