using UnityEngine;
using System.Collections.Generic;

public class FlameThrowerAssignier : MonoBehaviour
{
    [SerializeField] Vector3 spawnPos;
    [SerializeField] float weaponEndPoint = 1.5f;
    [SerializeField] List<Material> mainMat;
    [SerializeField] List<Material> transperantMat;
    [SerializeField] GameObject flameThrower;
    WeaponHandle weaponHandle;
    GameObject pauseManager;
    bool triggered = false;

    void Awake()
    {
        pauseManager = GameObject.FindWithTag("Manager");
        if(pauseManager.transform.Find(gameObject.name))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            WeaponAssigner();
        }
    }

    public void WeaponAssigner()
    {
        weaponHandle = GameObject.FindWithTag("Player").GetComponent<WeaponHandle>();
        weaponHandle.NewWeaponAssignier(flameThrower,mainMat,transperantMat,weaponEndPoint,spawnPos);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
