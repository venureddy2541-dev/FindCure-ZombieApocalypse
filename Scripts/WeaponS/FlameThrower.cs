using UnityEngine;

public class FlameThrower : PlayerFiring
{
    [SerializeField] float weaponEndPoint = 1.5f;
    [SerializeField] Material mainMat;
    [SerializeField] Material transperantMat;
    Transform weaponPos;
    [SerializeField] GameObject flameThrower;
    WeaponHandle weaponHandle;
    PlayerFiring playerFiring;
    GameObject player;
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
        player = GameObject.FindWithTag("Player");
        playerFiring = player.GetComponent<PlayerFiring>();
        weaponHandle = player.GetComponent<WeaponHandle>();
        weaponPos = GameObject.FindWithTag("PlayerWeaponsHolder").transform;
        GameObject newWeapon = Instantiate(flameThrower,new Vector3(weaponPos.position.x,weaponPos.position.y + 0.15f,weaponPos.position.z),weaponPos.rotation,weaponPos);
        newWeapon.SetActive(false);
        weaponHandle.NewWeaponAssignier(newWeapon,mainMat,transperantMat,weaponEndPoint);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
