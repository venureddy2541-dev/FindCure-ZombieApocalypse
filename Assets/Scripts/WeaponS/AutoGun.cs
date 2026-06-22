using UnityEngine;
using System.Collections;

public class AutoGun : WeaponType
{
    public override void Fire(bool fired)
    {
        this.fired = fired;
        if (!reloaded && fired)
        {
            StopCoroutine("Firing");
            StartCoroutine("Firing");
        }
    }

    IEnumerator Firing()
    {
        while (fired)
        {
            base.OnFire();
            yield return new WaitForSeconds(weaponData.fireRate);
            shootRate = true;
        }
    }
}
