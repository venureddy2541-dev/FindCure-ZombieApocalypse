using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] GameObject explotionEffect;
    [SerializeField] GameObject[] ammos;
    GameObject ammoType;
    int woodHealth = 50;

    void OnEnable()
    {
        Invoke("Ammo",1f);
    }

    void Ammo()
    {
        int ammoIndex = Random.Range(0,ammos.Length);
        ammoType = Instantiate(ammos[ammoIndex],transform.position,Quaternion.identity,transform.parent);
    }

    public void TakeDamage(int damage)
    {
        woodHealth -= damage;
        if(woodHealth <= 0)
        {
            ammoType.GetComponent<Rotator>().Activator(0,1);
            woodHealth = 5;
            GameObject woodBox = Instantiate(explotionEffect,transform.position,transform.rotation);
            woodBox.transform.localScale = gameObject.transform.localScale;
            woodBox.GetComponent<ExplodeBarrel>().Explode();
            gameObject.SetActive(false);
        }
    }
}
