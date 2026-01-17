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
        if (!reloaded)
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
                var emission = mazilFlash.emission;
                emission.enabled = true;
            }

            if (magSize == 0)
            {
                gunAudioSource.Stop();
                fired = false;
                var emission = mazilFlash.emission;
                emission.enabled = fired;
                if(storageSize > 0) { base.ToggleWeaponReload(true); }
            }
        }
        else if (storageSize == 0)
        {
            if (!gunAudioSource.isPlaying)
            {
                gunAudioSource.PlayOneShot(audioClips.emptyGunSound);
            }
            MessageBox.messageBox.PressentMessage("OUT OF AMMO", null);
        }
    }
}
