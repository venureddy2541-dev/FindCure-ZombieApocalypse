using UnityEngine;
using System.Collections;
using TMPro;

public class Rotator : MonoBehaviour
{
    [SerializeField] float xspeed = 5f;
    [SerializeField] float yspeed = 7f;
    [SerializeField] int x = 1;
    [SerializeField] int y = 0;
    
    void OnEnable()
    {
        if(gameObject.CompareTag("HealthKit") || gameObject.CompareTag("Granade") || gameObject.CompareTag("StandGun")|| gameObject.CompareTag("Weapon"))
        {
            Activator(x,y);
        }
    }

    public void Activator(int xval,int yval)
    {
        Vector2 vals = new Vector2(xval,yval);
        StartCoroutine("ActivateRotation",vals);
    }

    IEnumerator ActivateRotation(Vector2 vals)
    {
        while(true)
        {
            transform.Rotate(vals.x*xspeed,vals.y*yspeed,0);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
