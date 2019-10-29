using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{

    public static int enemiesAlive = 0;

    [SerializeField]
    private int waveIndex = 0;

    [SerializeField]
    private float countdown = 0f;
    [SerializeField]
    private float timeBeforeWave1 = 0f;
    [SerializeField]
    private float timeBetweenWaves = 0f;

    [SerializeField]
    private Wave[] waves = null;
    [SerializeField]
    private Transform spawnPoint = null;

    [SerializeField]
    private Text waveCountdownText = null;

    private bool isDoneSpawning = true;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        countdown = timeBeforeWave1;
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
                StartCoroutine(spawnWave());
                countdown = timeBetweenWaves;
            }
            else
            {
                countdown -= Time.deltaTime;
                countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
            }
            waveCountdownText.text = string.Format("{0:00.00}", countdown);
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

