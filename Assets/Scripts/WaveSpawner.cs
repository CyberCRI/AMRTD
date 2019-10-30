using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public static int enemiesAlive = 0;

    private int waveIndex = 0;
    private float countdown = 0f;
    private bool isDoneSpawning = true;

    [SerializeField]
    private Text waveCountdownText = null;
    [SerializeField]
    private float timeBeforeWave1 = 0f;
    [SerializeField]
    private float timeBetweenWaves = 0f;
    [SerializeField]
    private Transform spawnPoint = null;
    [SerializeField]
    GameManager gameManager = null;
    [SerializeField]
    private Wave[] waves = null;    

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        countdown = timeBeforeWave1;
        enemiesAlive = 0;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (isDoneSpawning && enemiesAlive <= 0)
        {
            if (countdown <= 0)
            {
                //Debug.Log("isDoneSpawning && enemiesAlive <= 0 && countdown <= 0");
                if (PlayerStatistics.waves < waves.Length)
                {
                    //Debug.Log("PlayerStatistics.waves < waves.Length");
                    StartCoroutine(spawnWave());
                    if (PlayerStatistics.waves < waves.Length-1)
                    {
                        countdown = timeBetweenWaves;
                    }
                }
                else
                {
                    // level completed
                    //Debug.Log("level completed");
                    gameManager.completeLevel();
                    this.enabled = false;
                }
            }
            else
            {
                countdown -= Time.deltaTime;
                countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
                waveCountdownText.text = string.Format("{0:00.00}", countdown);
            }
        }
    }

    IEnumerator spawnWave()
    {
        isDoneSpawning = false;
        PlayerStatistics.waves++;
        waveIndex = Mathf.Clamp(waveIndex, 0, waves.Length-1);
        Wave wave = waves[waveIndex];

        //Debug.Log("New wave incoming");

        for (int i = 0; i < wave.count; i++)
        {
            spawnEnemy(wave);
            yield return new WaitForSeconds(wave.timeBetweenSpawns);
        }

        waveIndex++;
        isDoneSpawning = true;
    }

    void spawnEnemy(Wave wave)
    {
        Instantiate(wave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemiesAlive++;
    }
}

