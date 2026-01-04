using UnityEngine;
using System.Collections;

public class ShellManager : MonoBehaviour
{
    [SerializeField] float inActiveTime;
    [SerializeField] GameObject[] weapons;
    WeaponShootActivator weaponShootActivator;

    void Awake()
    {
        weaponShootActivator = transform.parent.parent.GetComponent<WeaponShootActivator>();
    }

    void OnEnable()
    {
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        StartCoroutine(InactiveState());
    }

    IEnumerator InactiveState()
    {
        yield return new WaitForSeconds(inActiveTime);
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
        gameObject.SetActive(false);
    }
}
