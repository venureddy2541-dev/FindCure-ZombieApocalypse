using UnityEngine;

public class SelfDistruct : MonoBehaviour
{
    void Start()
    {
        Invoke("SelfDestroy",10);
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
