using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UI;

public class WeaponHandle : MonoBehaviour
{
    [SerializeField] AnimationEvents animationEvents;
    [SerializeField] RecoilManager recoilManager;
    [SerializeField] Animator animator;
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
    public Image weaponImageSlot;
    public List<Sprite> DisplayWeapons;

    [Serializable]
    public class MatListClass
    {
        public List<Material> transparentMaterials;
        public List<Material> originalMaterials;
    }
    public List<MatListClass> materialsList;

    List<MeshRenderer> renderers = new List<MeshRenderer>();

    public List<SkinnedMeshRenderer> bodyRenderers = new List<SkinnedMeshRenderer>();
    public List<MatListClass> bodyMaterials = new List<MatListClass>();
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
        animator.runtimeAnimatorController = weaponTypes[index].overrideController;
        recoilManager.AssiginWeaponData(weaponTypes[index].weaponData);
        
        for(int i=0;i<weaponTypes.Count;i++)
        { 
            weaponTypes[i].AssiginComponenets(playerManager,recoilManager);
            renderers.Add(Weapons[i].GetComponent<MeshRenderer>());
        }
    }

    void OnEnable()
    {
        Weapons[index].SetActive(true);
        animator.runtimeAnimatorController = weaponTypes[index].overrideController;
        if(DisplayWeapons[index]) weaponImageSlot.sprite = DisplayWeapons[index];
    }

    void Update()
    {   
        if(!isAlive.alive) return;

        if(canScroll && !playerManager.fired)
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
                animator.runtimeAnimatorController = weaponTypes[index].overrideController;
                animator.CrossFadeInFixedTime("idle/walk/run",0.1f);
                animationEvents.SwitchWeaponToRightHand();
                recoilManager.AssiginWeaponData(weaponTypes[index].weaponData);
                if(DisplayWeapons[index]) weaponImageSlot.sprite = DisplayWeapons[index];
                playerManager.WeaponAssigner(weaponTypes[index]);
            }
            else
            {
                Weapons[i].SetActive(false);
            }
        }
    }

    public void ToTransparent()
    {
        for(int i = 0;i<Weapons.Count;i++)
        {
            Material[] mats = renderers[i].materials;
            for(int j=0;j<materialsList[i].transparentMaterials.Count;j++)
            {
                mats[j] = materialsList[i].transparentMaterials[j];
            }
            renderers[i].materials = mats;
        }

        for(int k=0;k<bodyRenderers.Count;k++)
        {
            Material[] mats = bodyRenderers[k].materials;
            for(int l=0;l<bodyMaterials[k].transparentMaterials.Count;l++)
            {
                mats[l] = bodyMaterials[k].transparentMaterials[l];
            }
            bodyRenderers[k].materials = mats;
        }
    }

    public void ToOpaque()
    {
        for(int i = 0;i<Weapons.Count;i++)
        {
            Material[] mats = renderers[i].materials;
            for(int j=0;j<materialsList[i].originalMaterials.Count;j++)
            {
                mats[j] = materialsList[i].originalMaterials[j];
            }
            renderers[i].materials = mats;
        }

        for(int k=0;k<bodyRenderers.Count;k++)
        {
            Material[] mats = bodyRenderers[k].materials;
            for(int l=0;l<bodyMaterials[k].originalMaterials.Count;l++)
            {
                mats[l] = bodyMaterials[k].originalMaterials[l];
            }
            bodyRenderers[k].materials = mats;
        }
    }

    void WeaponCollisionDitecter()
    {
        RaycastHit hit;

        if (Physics.Raycast(origin.position,origin.forward,out hit,weaponsEndPoints[index],layers,QueryTriggerInteraction.Ignore))
        {
            if(!didHit)
            {
                playerManager.StopShootingOrThrowing();
            }
            didHit = true;
            /*weapons.position = CantShootPos.position;
            weapons.rotation = Quaternion.Slerp(weapons.rotation,CantShootPos.rotation,Time.deltaTime*Speed);*/
        }
        else
        {
            if(didHit)
            {
                playerManager.ActivateShootingOrThrowing();
            }
            didHit = false;
            /*weapons.position = originalPos.position;
            weapons.rotation = Quaternion.Slerp(weapons.rotation,originalPos.rotation,Time.deltaTime*Speed);*/
        }
    }

    public void NewWeaponAssignier(GameObject newWeaponObject,List<Material> mainMat,List<Material> transperantMat,float endPoint,Vector3 spawnPos)
    {
        GameObject newWeapon = Instantiate(newWeaponObject,weapons.position,weapons.rotation,weapons);
        newWeapon.transform.localPosition = spawnPos;        
        Weapons.Add(newWeapon);
        WeaponType newWeaponType = newWeapon.GetComponent<WeaponType>();
        newWeaponType.AssiginPickUpWeaponComponenets(playerManager,recoilManager,animator,magText,weaponAudioSource);
        weaponTypes.Add(newWeaponType);
        
        renderers.Add(newWeapon.GetComponent<MeshRenderer>());
        MatListClass matListClass = new MatListClass
        {
            transparentMaterials = transperantMat,
            originalMaterials = mainMat
        };

        materialsList.Add(matListClass);
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

    public WeaponType GetWeaponType
    {
        get { return weaponTypes[index]; }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Store"))
        {
            playerManager.ToggleShootingOrThrowing(FireStateEnum.CantFire);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Store"))
        {
            playerManager.ToggleShootingOrThrowing(FireStateEnum.CanFire);
        }
    }
}
