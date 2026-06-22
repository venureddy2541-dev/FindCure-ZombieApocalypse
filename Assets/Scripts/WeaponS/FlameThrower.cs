using UnityEngine;
using System.Collections;

public class FlameThrower : WeaponType
{
    [SerializeField] ParticleSystem fire;

    public override bool CanZoom()
    {
        return false;
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
            shootRate = true;
        }

        if(!fired)
        {
            if(gunAudioSource.loop)
            {
                var emission = fire.emission;
                emission.enabled = fired;
                gunAudioSource.loop = false;
                var mazilemission = mazilFlash.emission;
                mazilemission.enabled = false;
                gunAudioSource.Stop();
            }
        }
    }

    protected override void OnFire()
    {
        if (magSize > 0)
        {
            shootRate = false;
            magSize--;
            MagText.text = magSize.ToString() + "/" + storageSize.ToString();

            if (!gunAudioSource.isPlaying)
            {
                gunAudioSource.loop = true;
                gunAudioSource.clip = weaponData.weaponSound;
                gunAudioSource.Play();
                var fireEmission = fire.emission;
                fireEmission.enabled = fired;
                var emission = mazilFlash.emission;
                emission.enabled = fired;
            }
            else
            {
                var emission = mazilFlash.emission;
                emission.enabled = false;
            }

            if (magSize == 0)
            {
                fired = false;
                AutoReload();
            }
        }
        else
        {
            fired = false;
            gunAudioSource.PlayOneShot(weaponData.emptyGunSound);
        }

        if (magSize == 0 && storageSize == 0)
        {
            MessageBox.messageBox.PressentMessage("OUT OF AMMO", null);
        }
    }
}
