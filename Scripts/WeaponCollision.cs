using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    [SerializeField] GameObject CantShootPos;
    [SerializeField] GameObject gun;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("hi");
        gun.transform.position = CantShootPos.transform.position;
        gun.transform.rotation = CantShootPos.transform.rotation;
    }

    void OnTriggerExit(Collider other)
    {
        gun.transform.position = transform.position;
        gun.transform.rotation = transform.rotation;
    }
}
