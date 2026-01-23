using UnityEngine;
using System.Collections.Generic;

public class WeaponShootActivator : MonoBehaviour
{
    [SerializeField] WeaponType weaponType;
    public WeaponData weaponData;
    [SerializeField] GameObject bulletShell;
    [SerializeField] GameObject bulletShellPos;
    Queue<GameObject> shellsPool;
    Queue<Rigidbody> shellsRbPool;
    [SerializeField] int poolSize;
    [SerializeField] float upForce = 5;
    [SerializeField] float leftForce = 5;
    int order = 0;

    void Awake()
    {
        shellsPool = new Queue<GameObject>(poolSize);
        shellsRbPool = new Queue<Rigidbody>(poolSize);

        for(int i =0;i<poolSize;i++)
        {
            GameObject gb = Instantiate(bulletShell,bulletShellPos.transform.position,bulletShellPos.transform.rotation,bulletShellPos.transform);
            shellsPool.Enqueue(gb);
            shellsRbPool.Enqueue(gb.GetComponent<Rigidbody>());
            gb.SetActive(false);
        }
    }

    public void ShellFiring()
    {
        GameObject currentShell = shellsPool.Dequeue();
        shellsPool.Enqueue(currentShell);
        Rigidbody rb = shellsRbPool.Dequeue();
        shellsRbPool.Enqueue(rb);

        currentShell.SetActive(true);
        rb.AddForce((transform.up*upForce) + (transform.right*-leftForce),ForceMode.VelocityChange);
    }

    public void WeaponShootActi()
    {
        weaponType.shootRate = true;
    }
}
