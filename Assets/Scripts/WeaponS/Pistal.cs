using UnityEngine;

public class Pistal : WeaponType
{
    public override bool Zoom(bool zoomState)
    {
        base.ZoomInAndOut(zoomState);
        return true;
    }
}
