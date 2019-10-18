using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{

    [SerializeField]
    private int waveIndex = 0;

    [SerializeField]
    private float countdown = 0f;
    [SerializeField]
    private float timeBeforeWave1 = 0f;
    [SerializeField]
    private float timeBetweenWaves = 0f;

    [SerializeField]
    private float timeBetweenSpawns = 0f;

    [SerializeField]
    private Transform enemyPrefab = null;
    [SerializeField]
    private Transform spawnPoint = null;

    [SerializeField]
    private Text waveCountdownText = null;

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
        if (countdown <= 0)
        {
            StartCoroutine(spawnWave());
            countdown = timeBetweenWaves;
        }

        countdown -= Time.deltaTime;

        waveCountdownText.text = Mathf.Round(countdown).ToString();
    }

    IEnumerator spawnWave()
    {
        waveIndex++;

        Debug.Log("New wave incoming");

        for (int i = 0; i < waveIndex; i++)
        {
            spawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void spawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}

