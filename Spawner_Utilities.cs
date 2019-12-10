using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Utilities : MonoBehaviour
{

    public GameObject enemyPrefab;
    public DamageReceiver player;
    public Texture crosshairTexture;
    public float spawnInterval = 2; // Spawn new enemy every n seconds
    public int enemiesPerWave = 5; // How many enemies per wave
    public Transform[] spawnPoints; // List of spawnpoints to be assigned to gameObjects

    float nextSpawnTime = 0;
    int waveNumber = 1;
    bool waitingForWave = true; // False to instantly start new waves
    float newWaveTimer = 0;
    int enemiesToEliminate;
    // How many enemies we already eliminated in the current wave
    int enemiesEliminated = 0;
    int totalEnemiesSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Wait 10 seconds for new wave to start
        newWaveTimer = 10;
        waitingForWave = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForWave)
        {
            if (newWaveTimer >= 0)
            {
                newWaveTimer -= Time.deltaTime;
            }
            else
            {
                
                //Initialize new wave
                enemiesToEliminate = waveNumber * enemiesPerWave;
                enemiesEliminated = 0;
                totalEnemiesSpawned = 0;
                waitingForWave = false;
            }
        }
        else
        {
            if (Time.time > nextSpawnTime)
            {
                nextSpawnTime = Time.time + spawnInterval;

                //Spawn enemy if total enemies is less than enemies needed to eliminate
                if (totalEnemiesSpawned < enemiesToEliminate)
                {
                    Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
                    //Spawns an enemy at a random spawnpoint
                    GameObject enemy = Instantiate(enemyPrefab, randomPoint.position, Quaternion.identity);
                    NPCEnemy npc = enemy.GetComponent<NPCEnemy>();
                    npc.playerTransform = player.transform;
                    npc.es = this;
                    totalEnemiesSpawned++;
                }
            }
        }

        if (player.playerHP <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
        }
    }


    // This function is checking if all the enemies in the wave have been eliminated and if they have it starts a timer until the beginning of the next wave.
    public void EnemyEliminated(NPCEnemy enemy)
    {
        enemiesEliminated++;

        if (enemiesToEliminate - enemiesEliminated <= 0)
        {
            //Start next wave
            newWaveTimer = 10;
            waitingForWave = true;
            waveNumber++;
        }
    }
}
