using UnityEngine;
using TMPro;

public class Ammo : MonoBehaviour
{
    [SerializeField] int AmmoSize;
    [SerializeField] int maxAmmo;
    string ammoType;
    [SerializeField] int ammoIndexNumber;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            bool canDestroy = other.GetComponent<WeaponHandle>().AssignAmmo(AmmoSize,maxAmmo,ammoIndexNumber);
            if(canDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
