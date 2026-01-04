using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

public class MovingLasers : MonoBehaviour
{
    [SerializeField] PlayableDirector pb;
    [SerializeField] float speed;
    [SerializeField] Transform movableLasers;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    [SerializeField] AudioClip lasersActivated;
    MessageBox messageBox;

    public bool triggered = false;
    public int Key;
    public int tempKey;

    void Awake()
    {
        messageBox = GameObject.FindWithTag("MessageBox").GetComponent<MessageBox>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            pb.Play();
            messageBox.PressentMessage("Lasers are activated,Fall back and secure the key from the fallen soldiers",lasersActivated);
            movableLasers.gameObject.SetActive(true);
            StartCoroutine("StartLasers");
        }
    }

    IEnumerator StartLasers()
    {
        float lerpVal = 0f;
        while (Key != tempKey)
        {
            lerpVal += Time.deltaTime * speed;
            float newLerpVal = Mathf.Sin(lerpVal - Mathf.PI / 2f);
            newLerpVal = (newLerpVal + 1) / 2;
            movableLasers.position = Vector3.Lerp(startPos.position, endPos.position, newLerpVal);
            yield return null;
        }
    }
}
