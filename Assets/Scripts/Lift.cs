using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections;

public class Lift : MonoBehaviour
{
    [SerializeField] PlayableDirector liftTimeline;
    bool triggered = true;

    void Awake()
    {
        if(GameManager.gameManager.transform.Find(gameObject.name))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {        
        if(other.CompareTag("Player") && triggered)
        {
            triggered = false;
            GameManager.gameManager.GameStart();
            liftTimeline.Play();
        }
    }

    void ParentAssignier()
    {
        transform.parent.parent = GameManager.gameManager.transform;
    }
}
