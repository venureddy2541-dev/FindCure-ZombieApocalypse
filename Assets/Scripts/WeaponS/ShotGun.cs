using UnityEngine;
using System.Collections.Generic;

public class ShotGun : WeaponType
{
    [SerializeField] int palletSize = 8;

    protected override void InitiateShoot(Vector3 startPos,Vector3 direction)
    {
        for(int j=0;j<palletSize;j++)
        {
            base.InitiateShoot(startPos,direction);
        }
    }
}
