using UnityEngine;
using System.Collections;

public class FireParticle : MonoBehaviour
{
    [SerializeField] ParticleSystem thisParticleSystem;
    Transform currentHitObject;
    Vector3 hitPos;

    void OnEnable()
    {
        StartCoroutine("ResetFollowPos");
    }

    void Update()
    {
        if(currentHitObject != null) { transform.position = currentHitObject.position; }
    }

    public void Play()
    {
        thisParticleSystem.Play();
    }

    public void SetFollowPos(Transform currentParent)
    {
        currentHitObject = currentParent;
    }

    public void SetFollowPos(Vector3 hitPos)
    {
        transform.position = hitPos;
    }

    IEnumerator ResetFollowPos()
    {
        yield return new WaitForSeconds(1f);
        currentHitObject = null;
        gameObject.SetActive(false);
    }
}
