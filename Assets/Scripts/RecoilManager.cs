using UnityEngine;
using System.Collections;

public class RecoilManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    WeaponData weaponData;
    public Transform camRootPos;

    public void AssiginWeaponData(WeaponData weaponData)
    {
        this.weaponData = weaponData;
    }

    public void TriggerRecoil()
    {
        StopAllCoroutines();
        Quaternion finalRotation = Quaternion.Euler(playerController.UpdateCamPitch(-weaponData.xRecoil),Random.Range(-weaponData.yRecoil,weaponData.yRecoil),0f);
        camRootPos.localRotation = finalRotation;
    }
}
