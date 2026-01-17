using UnityEngine;

public class FlameThrowerAssignier : MonoBehaviour
{
    [SerializeField] float weaponEndPoint = 1.5f;
    [SerializeField] Material mainMat;
    [SerializeField] Material transperantMat;
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
        weaponHandle.NewWeaponAssignier(flameThrower,mainMat,transperantMat,weaponEndPoint);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
