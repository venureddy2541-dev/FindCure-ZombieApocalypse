using UnityEngine;

public class Crate : MonoBehaviour
{
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
        ammoType.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        woodHealth -= damage;
        if(woodHealth <= 0)
        {
            ammoType.SetActive(true);
            ammoType.GetComponent<Rotator>().Activator(0,1);

            ExplodeCreate ec = RequiredParticles.instance.GetCreateEffect();
            ec.transform.localScale = gameObject.transform.localScale;
            ec.transform.position = transform.position;
            ec.transform.rotation = transform.rotation;
            ec.Explode();

            Destroy(gameObject);
        }
    }
}
