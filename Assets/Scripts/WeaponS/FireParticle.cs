using UnityEngine;
using System.Collections;

public class FireParticle : MonoBehaviour
{
    Transform currentHitObject;
    Vector3 hitPos;

    void OnEnable()
    {
        StartCoroutine("ResetParent");
    }

    void Update()
    {
        if(currentHitObject != null) { transform.position = currentHitObject.position; }
    }

    public void SetFollowPos(Transform currentParent)
    {
        currentHitObject = currentParent;
    }

    public void SetFollowPos(Vector3 hitPos)
    {
        transform.position = hitPos;
    }

    IEnumerator ResetParent()
    {
        yield return new WaitForSeconds(1f);
        currentHitObject = null;
        gameObject.SetActive(false);
    }
}
