using UnityEngine;

public class BlastForce : MonoBehaviour
{
    Rigidbody rb;
    Collider col;
    float Speed;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void GenerateForceForOther()
    {
        col.isTrigger = false;
        rb.isKinematic = false;
        Speed = Random.Range(8f, 10f);
        float otherSpeed = Random.Range(8f, 10f);
        int direction = Random.Range(-1, 2);

        rb.AddForce(new Vector3(direction*otherSpeed,1*Speed,direction*otherSpeed) ,ForceMode.Impulse);

    }

    public void ForceForRequiredDir()
    {
        gameObject.GetComponent<tier>().enabled = false;
        col.isTrigger = false;
        rb.isKinematic = false;
        float  Speed1 = Random.Range(1f, 2f);

        rb.AddForce(new Vector3(1, 0, 0) * Speed1, ForceMode.Impulse);

    }
}
