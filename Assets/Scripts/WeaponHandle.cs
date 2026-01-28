using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WeaponHandle : MonoBehaviour
{
    [SerializeField] TMP_Text magText;
    [SerializeField] AudioSource weaponAudioSource;
    [SerializeField] Transform origin;
    [SerializeField] Transform CantShootPos;
    [SerializeField] Transform originalPos;
    [SerializeField] Transform weapons;
    [SerializeField] float Speed;
    [SerializeField] LayerMask layers;
    
    [SerializeField] List<float> weaponsEndPoints;
    public List<GameObject> Weapons;
    public List<WeaponType> weaponTypes;
    public List<GameObject> DisplayWeapons;
    [SerializeField] List<Material> transparentMaterials;
    [SerializeField] List<Material> originalMaterials;
    PlayerManager playerManager;
    PlayerHealth playerHealth;
    IsAlive isAlive;
    public List<int> currentAmmoSizes = new List<int>();

    int index = 0;
    bool didHit = false;
    public bool canScroll = true;

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerHealth = GetComponent<PlayerHealth>();
        isAlive = GetComponent<IsAlive>();

        Weapons[index].SetActive(true);
        DisplayWeapons[index].SetActive(true);
    }

    void OnEnable()
    {
        Weapons[index].SetActive(true);
        DisplayWeapons[index].SetActive(true);
    }

    void Update()
    {   
        if(!isAlive.alive) return;

        if(canScroll && !playerManager.fired && weaponTypes[index].shootRate)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if(scroll > 0.1)
            {
                index++;
                if(index < Weapons.Count){ CurrentWeaponActivator(); }
                else{ index--; }
            }

            if(scroll < -0.1)
            {
                index--;
                if(index >= 0){ CurrentWeaponActivator(); }
                else{ index++; }
            }
        }

        WeaponCollisionDitecter();
    }

    void CurrentWeaponActivator()
    {
        for(int i = 0;i<Weapons.Count;i++)
        {
            if(index == i)
            {
                Weapons[index].SetActive(true);
                DisplayWeapons[index].SetActive(true);
                playerManager.WeaponAssigner(weaponTypes[index]);
            }
            else
            {
                DisplayWeapons[i].SetActive(false);
                Weapons[i].SetActive(false);
            }
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

        if (Physics.Raycast(origin.position,origin.forward,out hit,weaponsEndPoints[index],layers,QueryTriggerInteraction.Ignore))
        { 
            if(!didHit)
            {
                playerManager.ToggleShootingOrThrowing(FireStateEnum.CantFire);
            }
            didHit = true;
            weapons.position = CantShootPos.position;
            weapons.rotation = Quaternion.Slerp(weapons.rotation,CantShootPos.rotation,Time.deltaTime*Speed);
        }
        else
        {
            if(didHit)
            {
                playerManager.ToggleShootingOrThrowing(FireStateEnum.CanFire);
            }
            didHit = false;
            weapons.position = originalPos.position;
            weapons.rotation = Quaternion.Slerp(weapons.rotation,originalPos.rotation,Time.deltaTime*Speed);
        }
    }

    public void NewWeaponAssignier(GameObject newWeaponObject,Material mainMat,Material transperantMat,float endPoint)
    {
        GameObject newWeapon = Instantiate(newWeaponObject,new Vector3(weapons.position.x,weapons.position.y + 0.15f,weapons.position.z),weapons.rotation,weapons);
        Weapons.Add(newWeapon);
        WeaponType newWeaponType = newWeapon.GetComponent<WeaponType>();
        newWeaponType.magText = magText;
        newWeaponType.gunAudioSource = weaponAudioSource;
        weaponTypes.Add(newWeaponType);
        originalMaterials.Add(mainMat);
        transparentMaterials.Add(transperantMat);
        weaponsEndPoints.Add(endPoint);
        newWeapon.SetActive(false);
    }

    public bool AssignAmmo(int ammoSize,int maxAmmo,int index)
    {
        if(weaponTypes.Count <= index) return false;
        return weaponTypes[index].BulletAdder(ammoSize,maxAmmo);        
    }

    public void UpdateInitialAmmo()
    {
        currentAmmoSizes.Clear();
        foreach(WeaponType weaponType in weaponTypes)
        {
            currentAmmoSizes.Add(weaponType.weaponData.maxAmmo);
        }

        UpdateAmmoSizes(currentAmmoSizes);
    }

    public List<int> CurrentAmmoSizes()
    {
        currentAmmoSizes.Clear();
        foreach(WeaponType weaponType in weaponTypes)
        {
            currentAmmoSizes.Add(weaponType.storageSize);
        }

        return currentAmmoSizes;
    }

    public void UpdateAmmoSizes(List<int> previousAmmo)
    {
        for(int i=0;i<previousAmmo.Count;i++)
        {
            weaponTypes[i].storageSize = previousAmmo[i];
        }

        weaponTypes[index].UpdateWeaponData();
    }
}
