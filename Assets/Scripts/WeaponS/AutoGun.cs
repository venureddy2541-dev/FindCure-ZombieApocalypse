using UnityEngine;
using System.Collections;

public class AutoGun : WeaponType
{
    public override void Fire(bool fired)
    {
        this.fired = fired;
        if (!reloaded && fired)
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

        if(!fired) 
        { 
            if(gunAudioSource.loop) 
            { 
                gunAudioSource.loop = false;
                gunAudioSource.Stop();
            } 
        }
    }

    protected override void WeaponSound()
    {
        if (!gunAudioSource.isPlaying)
        {
            gunAudioSource.loop = true;
            gunAudioSource.clip = weaponData.weaponSound;
            gunAudioSource.Play();
        }
    }
}
