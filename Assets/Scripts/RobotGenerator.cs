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
    List<RoboBomb> roboBombs = new List<RoboBomb>();
    List<WalkingRobots> walkingRobots = new List<WalkingRobots>();

    void Awake()
    {
        for(int i = 0;i<count;i++)
        {
            robots.Add(Instantiate(robot[Random.Range(0,robot.Length)],instantiatePos.position,instantiatePos.rotation,instantiatePos));
            RoboBomb roboBomb = robots[i].GetComponent<RoboBomb>();
            if(roboBomb)
            {
                roboBombs.Add(roboBomb); 
                roboBomb.reBirth = true;
                roboBomb.player = player; 
                roboBomb.blastAudioSource = blastAudioSource; 
            }
            else 
            { 
                WalkingRobots walkingRobot = robots[i].GetComponent<WalkingRobots>(); 
                walkingRobots.Add(walkingRobot); 
                walkingRobot.reBirth = true;
                walkingRobot.player = player;
                walkingRobot.blastAudioSource = blastAudioSource;
            }

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
            FinalStage finalStage = GetComponentInParent<FinalStage>();
            int remainingRobotsCount = 0;
            
            for(int i = 0;i<roboBombs.Count;i++)
            {
                if(roboBombs[i].gameObject.activeSelf)
                {
                    remainingRobotsCount++;
                    roboBombs[i].MasterDead(finalStage);
                }
            }

            for(int j =0;j<walkingRobots.Count;j++)
            {
                if(walkingRobots[j].gameObject.activeSelf)
                {
                    remainingRobotsCount++;
                    walkingRobots[j].MasterDead(finalStage);
                }
            }
            
            finalStage.UpdateCount(remainingRobotsCount);
            finalStage.RobotsDeadCount();
            Instantiate(blastParticles,transform.position,transform.rotation);
            Destroy(gameObject);
        }
    }
}
