using UnityEngine;
using System.Collections;

public class AutoGun : WeaponType
{
    public override void Fire(bool fired)
    {
        this.fired = fired;
        if (!reloaded)
        {
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

    protected override void WeaponSound()
    {
        if (!gunAudioSource.isPlaying)
        {
            gunAudioSource.clip = weaponData.weaponSound;
            gunAudioSource.Play();
        }
    }
}
