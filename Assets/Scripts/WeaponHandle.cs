using UnityEngine;
using System.Collections.Generic;

public class WeaponHandle : MonoBehaviour
{
    [SerializeField] Transform origin;
    [SerializeField] Transform CantShootPos;
    [SerializeField] Transform originalPos;
    [SerializeField] Transform weapons;
    [SerializeField] float Speed;
    [SerializeField] LayerMask layers;
    
    [SerializeField] List<float> weaponsEndPoints;
    public List<GameObject> Weapons;
    public List<GameObject> DisplayWeapons;
    [SerializeField] List<Material> transparentMaterials;
    [SerializeField] List<Material> originalMaterials;
    PlayerFiring playerFiring;
    PlayerHealth playerHealth;
    IsAlive isAlive;

    int count;
    bool didHit = false;

    void Awake()
    {
        playerFiring = GetComponent<PlayerFiring>();
        playerHealth = GetComponent<PlayerHealth>();
        isAlive = GetComponent<IsAlive>();

        Weapons[count].SetActive(true);
        DisplayWeapons[count].SetActive(true);
        foreach(GameObject weapon in Weapons)
        {
            WeaponShootActivator weaponShootActivator = weapon.GetComponent<WeaponShootActivator>();
            playerFiring.storageSize.Add(weaponShootActivator.weaponType.maxAmmo);
            playerFiring.magSize.Add(weaponShootActivator.weaponType.magSize);
        }
    }

    void OnEnable()
    {
        Weapons[count].SetActive(true);
        DisplayWeapons[count].SetActive(true);
    }

    void Update()
    {   
        if(!isAlive.alive) return;

        if(!playerFiring.granadeSelected)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if(scroll > 0.1)
            {
                count++;
                if(count < Weapons.Count)
                {
                    WeaponSelector(count);
                }
                else
                {
                    count--;
                }
            }

            if(scroll < -0.1)
            {
                count--;
                if(count >= 0)
                {
                    WeaponSelector(count);
                }
                else
                {
                    count++;
                }

            }
        }

        WeaponCollisionDitecter();
    }

    void WeaponSelector(int weaponNo)
    {
        if(!playerFiring.reloaded && !playerFiring.willZoom && !playerFiring.fired && !playerFiring.GamePaused)
        {
            CurrentWeaponActivator(weaponNo);
        }
    }

    void CurrentWeaponActivator(int weaponNo)
    {
        for(int i = 0;i<Weapons.Count;i++)
        {
            if(weaponNo == i)
            {
                Weapons[weaponNo].SetActive(true);
                DisplayWeapons[weaponNo].SetActive(true);
                playerFiring.WeaponAssigner(Weapons[weaponNo].GetComponent<WeaponShootActivator>().weaponType,weaponNo);
            }
            else
            {
                DisplayWeapons[i].SetActive(false);
                Weapons[i].SetActive(false);
            }
        }
    }

    public void Reset()
    {
        count = 0;
        CurrentWeaponActivator(count);
        foreach(GameObject gb in Weapons)
        {
            gb.transform.parent = weapons;
            gb.transform.position = originalPos.position;
            gb.transform.rotation = originalPos.rotation;
        }
    }

    public void ToTransparent()
    {
        for(int i = 0;i<Weapons.Count;i++)
        {
            Weapons[i].GetComponent<MeshRenderer>().material = transparentMaterials[i];
        }
    }

    public void ToOpaque()
    {
        for(int i = 0;i<Weapons.Count;i++)
        {
            Weapons[i].GetComponent<MeshRenderer>().material = originalMaterials[i];
        }
    }

    void WeaponCollisionDitecter()
    {
        RaycastHit hit;

        if (Physics.Raycast(origin.position,origin.forward,out hit,weaponsEndPoints[count],layers,QueryTriggerInteraction.Ignore))
        { 
            didHit = true;
            playerFiring.idle = false;
            weapons.transform.position = CantShootPos.position;
            weapons.transform.rotation = Quaternion.Slerp(weapons.transform.rotation,CantShootPos.rotation,Time.deltaTime*Speed);
        }
        else
        {
            if(didHit && playerFiring.cantShoot == true)
            {
                playerFiring.idle = true;
            }
            didHit = false;
            weapons.transform.position = originalPos.position;
            weapons.transform.rotation = Quaternion.Slerp(weapons.transform.rotation,originalPos.rotation,Time.deltaTime*Speed);
        }
    }

    public void NewWeaponAssignier(GameObject newWeapon,Material mainMat,Material transperantMat,float endPoint)
    {
        Weapons.Add(newWeapon);
        Weapon weapon = newWeapon.GetComponent<WeaponShootActivator>().weaponType;
        playerFiring.magSize.Add(weapon.magSize);
        playerFiring.storageSize.Add(weapon.maxAmmo);
        originalMaterials.Add(mainMat);
        transparentMaterials.Add(transperantMat);
        weaponsEndPoints.Add(endPoint);
        playerFiring.NewWeapon(newWeapon.GetComponent<Animator>(),newWeapon.GetComponentInChildren<ParticleSystem>());
    }
}
