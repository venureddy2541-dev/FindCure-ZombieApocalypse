using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplodeBarrel : MonoBehaviour
{
	public float explosionForce = 10f;
	public float explosionRadius = 10f;
	public float upForceMin = 0.0f;
	public float upForceMax = 1f;

	public float lifeTime = 2.0f;

	public List<Rigidbody> rbs;
	public Vector3[] startPos;
	public Quaternion[] startRot;

	void Start()
	{
		startPos = new Vector3[rbs.Count];
		startRot = new Quaternion[rbs.Count];

		for(int i =0;i<rbs.Count;i++)
		{
			startPos[i] = rbs[i].transform.localPosition;
			startRot[i] = rbs[i].transform.localRotation;
		}
	} 

	public void Explode()
	{
		foreach (Rigidbody rb in rbs)
		{
			rb.isKinematic = false;
			rb.AddExplosionForce(explosionForce,transform.position, explosionRadius, Random.Range(upForceMin, upForceMax), ForceMode.Impulse);
		}

		Invoke("LifeTime",lifeTime);
	}

	void LifeTime()
	{
		for(int i=0;i<rbs.Count;i++)
		{
			rbs[i].linearVelocity = Vector3.zero;
			rbs[i].angularVelocity = Vector3.zero;

			rbs[i].isKinematic = true;

			rbs[i].transform.localPosition = startPos[i];
			rbs[i].transform.localRotation = startRot[i];
		}

		gameObject.SetActive(false);
	}
}
