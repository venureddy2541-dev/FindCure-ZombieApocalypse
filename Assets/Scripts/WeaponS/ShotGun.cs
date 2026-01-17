using UnityEngine;
using System.Collections.Generic;

public class ShotGun : WeaponType
{
    [SerializeField] int palletSize = 8;
    [SerializeField] float splitValue;

    public override bool Zoom(bool zoomState)
    {
        base.ZoomInAndOut(zoomState);
        return true;
    }

    protected override void InitiateShoot(Vector3 startPos,Vector3 endPos)
    {
        for(int j=0;j<palletSize;j++)
        {
            float upSplit = Random.Range(-splitValue,splitValue);
            float leftSplit = Random.Range(-splitValue,splitValue);
            Vector3 splitDirc = new Vector3(0f,1f*upSplit,1f*leftSplit);
            base.InitiateShoot(startPos,endPos + splitDirc);
        }
    }
}
