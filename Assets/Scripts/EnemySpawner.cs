using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyGetter enemyGetter;
    [SerializeField] int points;
    int remainingZombiesCount;
    int spawnerCount = 0;
    public int enemyCount = 20;
    int count = 0;
    [SerializeField] int maxspawnerCount;
    [SerializeField] int health;
    int healthRef;
    bool destroied = false;

    [SerializeField] Slider slider;
    AudioSource blastSounds;
    Vector3[] spawnPos = { new Vector3(-6f, 0, 0), new Vector3(6f, 0, 0), new Vector3(0, 0, -6f), new Vector3(0, 0, 6f) };
    public List<Enemy> enemies = new List<Enemy>();

    public EnemyTarget damageTaker;
    
    [SerializeField] GameObject orgPlayer;
    public GameObject attackPos;
    [SerializeField] GameObject Storage;
    IsAlive playerIsAlive;
    PlayerHealth playerHealth;
    SpawnerRebirth spawnerRebirth;
    Waves waves;
    [SerializeField] GameObject electricShield;
    [SerializeField] Collider electricShieldCol;
    Material shieldMat;
    bool gotZombies = false;

    void Awake()
    {
        shieldMat = electricShield.GetComponent<Renderer>().material;
        spawnerRebirth = gameObject.GetComponentInParent<SpawnerRebirth>();
        waves = GetComponentInParent<Waves>();
        playerIsAlive = orgPlayer.GetComponent<IsAlive>();
        blastSounds = GameObject.FindWithTag("BlastAudio").GetComponent<AudioSource>();
        slider.maxValue = health;
        playerHealth = attackPos.GetComponent<PlayerHealth>();
    }

    void OnEnable()
    {
        destroied = false;
        slider.gameObject.SetActive(true);
        healthRef = health;
        slider.value = health;
        count = 0;
        shieldMat.SetFloat("_desolve",1);
        StartCoroutine("EnemySpawnBuilder");
    }

    void OnDisable()
    {
        electricShieldCol.enabled = false;
        StopCoroutine(ElectricShield());
    }

    IEnumerator ElectricShield()
    {
        float i;
        float j;
        float lerp;
        bool activating = true;

        while(true)
        {
            i = 1;
            j = 0;
            
            yield return new WaitForSeconds(5f);

            while (i > j)
            {
                if(activating)
                {
                    i -= Time.deltaTime/2;
                    lerp = i;
                    if(i < 0.5 && i > 0.4) { electricShieldCol.enabled = true; }
                }
                else
                {
                    j += Time.deltaTime/2;
                    lerp = j;
                    if(j < 0.5 && j > 0.4) { electricShieldCol.enabled = false; }
                }
                shieldMat.SetFloat("_desolve",lerp);
                yield return null;
            }

            activating = !activating;
            yield return null;
        }
    }

    void GetZombies()
    {
        IsAlive isALive = attackPos.GetComponent<IsAlive>();
        enemies = enemyGetter.GetEnemiesByList();
        for (int i = 0; i < enemies.Count; i++)
        {
            int enemyPosIndex = Random.Range(0, spawnPos.Length);
            enemies[i].transform.position = transform.position + spawnPos[enemyPosIndex];
            enemies[i].AssiginPlayerData(orgPlayer,attackPos);
            enemies[i].navMesh.enabled = true;
            enemies[i].enabled = true;
        }
    }

    IEnumerator EnemySpawnBuilder()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
        }
        
        if(!gotZombies)  
        { 
            gotZombies = true; GetZombies(); 
        }
        InvokeRepeating("EnemySpawn", 1f, 1f);
        electricShieldCol.enabled = false;
        StartCoroutine(ElectricShield());
    }

    void EnemySpawn()
    {
        if(attackPos == null || !playerIsAlive.alive) return;

        if (playerHealth)
        {
            if(playerHealth.loopControler) return;
        }

        if (gameObject.activeInHierarchy && !enemies[count].gameObject.activeInHierarchy)
        {
            EnemySpawnRef(enemies[count]);
        }
        count++;
        count = count%enemyCount;
    }

    public void EnemySpawnRef(Enemy enemy)
    {
        enemy.isProvoked = true;
        int enemyPosIndex = Random.Range(0, spawnPos.Length);
        enemy.transform.position = transform.position + spawnPos[enemyPosIndex];
        enemy.gameObject.SetActive(true);
    }

    public void DamageTaker(int damage)
    {
        healthRef -= damage;
        slider.value = healthRef;
        if (healthRef <= 0 && !destroied)
        {
            destroied = true;
            GameManager.gameManager.UpdateCash(points);
            CancelInvoke("EnemySpawn");
            slider.gameObject.SetActive(false);
            blastSounds.Play();

            ParticleSystem currentEffect = RequiredParticles.instance.GetspawnerBlastParticle();
            currentEffect.transform.position = transform.position;
            currentEffect.transform.rotation = Quaternion.identity;
            currentEffect.Play();

            spawnerCount++;
            if(spawnerCount < maxspawnerCount)
            {
                waves.Counting();
                if (spawnerRebirth)
                {
                    destroied = false;
                    spawnerRebirth.ReBirth();
                }
                gameObject.SetActive(false);
            }
            else
            {
                for(int i=0;i<enemies.Count;i++)
                {
                    if(!enemies[i].dead && enemies[i].gameObject.activeInHierarchy)
                    {   
                        enemies[i].level = enemyGetter.level;
                        remainingZombiesCount++;
                    }
                }
                waves.CountAdder(remainingZombiesCount);
                waves.Counting();
                gameObject.SetActive(false);
            }
        }
    }

    public int Health
    {
        set { health = value; }
    }
}
