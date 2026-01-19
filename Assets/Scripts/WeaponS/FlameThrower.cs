using UnityEngine;
using System.Collections;

public class FlameThrower : WeaponType
{
    public override bool Zoom(bool zoomState)
    {
        return true;
    }

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
            OnFire();
            yield return new WaitForSeconds(weaponData.fireRate);
        }

        if(!fired)
        {
            var emission = mazilFlash.emission;
            emission.enabled = fired;
            gunAudioSource.Stop();
        }
    }

    protected override void OnFire()
    {
        if (magSize > 0)
        {
            magSize--;
            magText.text = magSize.ToString() + "/" + storageSize.ToString();

            if (!gunAudioSource.isPlaying)
            {
                gunAudioSource.clip = weaponData.weaponSound;
                gunAudioSource.Play();
            }

            var emission = mazilFlash.emission;
            emission.enabled = fired;

            if (magSize == 0)
            {
                fired = false;
                gunAudioSource.Stop();
                AutoReload();
            }
        }
        else
        {
            fired = false;
            gunAudioSource.PlayOneShot(audioClips.emptyGunSound);
        }

        if (magSize == 0 && storageSize == 0)
        {
            MessageBox.messageBox.PressentMessage("OUT OF AMMO", null);
        }
    }
}
