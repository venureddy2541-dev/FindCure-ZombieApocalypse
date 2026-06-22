using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] Transform rightHandPoint;
    [SerializeField] Transform leftHandPoint;
    [SerializeField] Transform weaponPoint;

    public void SwitchWeaponToRightHand()
    {
        weaponPoint.SetParent(rightHandPoint);
        weaponPoint.localPosition = Vector3.zero;
        weaponPoint.localRotation = Quaternion.identity;
    }   

    public void SwitchWeaponToLeftHand()
    {
        weaponPoint.SetParent(leftHandPoint);
    } 
}
