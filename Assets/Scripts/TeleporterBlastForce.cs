using UnityEngine;

public class TeleporterBlastForce : MonoBehaviour
{
    [SerializeField] float explosionForce = 10f;
    [SerializeField] float radius = 10f;
    float upForceMin = 0f;
    float upForceMax = 1f;

    void Start()
    {
        Vector3 position = transform.position;
        
        foreach(Transform chield in transform)
        {
            Rigidbody rb = chield.GetComponent<Rigidbody>();
            rb.AddExplosionForce(explosionForce,position,radius,Random.Range(upForceMin,upForceMax),ForceMode.Impulse);
        }
    }
}
