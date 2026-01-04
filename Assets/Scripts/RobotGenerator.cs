using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RobotGenerator : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject blastParticles;
    [SerializeField] AudioSource blastAudioSource;

    [SerializeField] GameObject[] robot;
    [SerializeField] Transform instantiatePos;
    [SerializeField] int count;
    int i = 0;
    [SerializeField] int health = 2000;
    [SerializeField] int reBirthTime = 30;

    List<GameObject> robots = new List<GameObject>();

    void Awake()
    {
        for(int i = 0;i<count;i++)
        {
            robots.Add(Instantiate(robot[Random.Range(0,robot.Length)],instantiatePos.position,instantiatePos.rotation,instantiatePos));
            RoboBomb roboBom = robots[i].GetComponent<RoboBomb>();
            if(roboBom) { roboBom.player = player; roboBom.blastAudioSource = blastAudioSource; }
            else { WalkingRobots walkingRobots = robots[i].GetComponent<WalkingRobots>(); walkingRobots.player = player; walkingRobots.blastAudioSource = blastAudioSource; }
            
            robots[i].SetActive(false);
        }
    }

    void Start()
    {
        StartCoroutine(RobotActivator());
    }

    IEnumerator RobotActivator()
    {
        while(i<count)
        {
            if(!robots[i].activeInHierarchy)
            {
                float timeDelay = Random.Range(0.5f,2f);
                yield return new WaitForSeconds(timeDelay);
                if(!player) yield break;
                robots[i].SetActive(true);
                i++;
            }
            else
            {
                i++;
                yield return null;
            }

            if(i == count)
            {
                yield return new WaitForSeconds(reBirthTime);
                i = 0;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            GetComponentInParent<FinalStage>().NormalRobotsDeadCount();
            Instantiate(blastParticles,transform.position,transform.rotation);
            Destroy(gameObject);
        }
    }
}
