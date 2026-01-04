using UnityEngine;
using System.Collections.Generic;

public class WeaponShootActivator : MonoBehaviour
{
    PlayerFiring playerFiring;
    public Weapon weaponType;
    [SerializeField] GameObject bulletShell;
    [SerializeField] GameObject bulletShellPos;
    List<GameObject> shellsPool = new List<GameObject>();
    [SerializeField] int pollSize;
    [SerializeField] float upForce = 5;
    [SerializeField] float leftForce = 5;
    int order = 0;

    void Awake()
    {
        int i = 0;
        while(i < pollSize)
        {
            shellsPool.Add(Instantiate(bulletShell,bulletShellPos.transform.position,bulletShellPos.transform.rotation,bulletShellPos.transform));
            shellsPool[i].gameObject.SetActive(false);
            i++;
        }
    }

    void Start()
    {
        playerFiring = GetComponentInParent<PlayerFiring>();
    }

    public void ShellFiring()
    {
        shellsPool[order].SetActive(true);
        Rigidbody rb = shellsPool[order].GetComponent<Rigidbody>();
        rb.AddForce((transform.up*upForce) + (transform.right*-leftForce),ForceMode.VelocityChange);
        order++;
        if(order >= pollSize){ order = 0; }
    }

    public void WeaponShootActi()
    {
        playerFiring.shootRate = true;
    }
}
