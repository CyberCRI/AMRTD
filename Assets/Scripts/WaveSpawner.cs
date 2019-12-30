using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance;
    public static int enemiesAlive = 0;

    private int waveIndex = 0;
    private float countdown = 0f;
    private bool isDoneSpawning = true;
    private Transform[] spawnPoints = null;

    private Text waveCountdownText = null;
    [SerializeField]
    private float timeBeforeWave1 = 0f;
    [SerializeField]
    private float timeBetweenWaves = 0f;
    [SerializeField]
    private SpawnMode spawnMode = SpawnMode.RANDOMDISCRETE;
    [SerializeField]
    GameManager gameManager = null;
    [SerializeField]
    private Wave[] waves = null;
    private Wave currentWave = null;

    private enum SpawnMode
    {
        RANDOMDISCRETE,
        RANDOMCONTINUOUS
    };

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;

            countdown = timeBeforeWave1;
            enemiesAlive = 0;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (
            isDoneSpawning
            &&
            (
                (GameManager.instance.isObjectiveDefenseMode())
                ||
                (enemiesAlive <= 0)
            )
        )
        {
            if (countdown <= 0f)
            {
                if (waveIndex < waves.Length)
                {
                    waveCountdownText.text = string.Format("{0:00.00}", 0f);
                    StartCoroutine(spawnWave());
                    if (PlayerStatistics.waves < waves.Length - 1)
                    {
                        countdown = timeBetweenWaves;
                    }
                }
                else if (enemiesAlive <= 0)
                {
                    gameManager.winLevel();
                    this.enabled = false;
                }
            }
            else
            {
                countdown -= Time.deltaTime;
                countdown = Mathf.Max(countdown, 0f);
                waveCountdownText.text = string.Format("{0:00.00}", countdown);
            }
        }
    }

    public void linkUI(
        Text _waveCountdownText
    )
    {
        waveCountdownText = _waveCountdownText;
    }

    public void linkMap(
        Transform[] _spawnPoints
        )
    {
        spawnPoints = new Transform[_spawnPoints.Length];
        _spawnPoints.CopyTo(spawnPoints, 0);
    }

    public void setCountdownToZero()
    {
        Debug.Log("setCountdownToZero");
        countdown = 0f;
    }

    IEnumerator spawnWave()
    {
        isDoneSpawning = false;
        yield return new WaitForSeconds(1f);
        PlayerStatistics.waves++;
        currentWave = waves[waveIndex];

        for (int i = 0; i < currentWave.count; i++)
        {
            spawnEnemy(currentWave);
            yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
        }

        waveIndex++;
        isDoneSpawning = true;
    }

    public Enemy spawnEnemy(
        Wave wave,
        GameObject enemyMotherCell = null,
        int reward = 0,
        float health = 0f,
        float startHealth = 0f,
        int waypointIndex = 0
        )
    {
        //Debug.Log(string.Format("WaveSpawner::spawnEnemy({0}, {1}, {2}, {3})"
        //    //,wave
        //    ,reward
        //    ,health
        //    ,startHealth
        //    ,waypointIndex
        //    //,location
        //));

        bool divisionMode = (null != enemyMotherCell);
        Enemy enemy = null;

        if (enemiesAlive < wave.maxEnemyCount)
        {
            Vector3 spawnPointPosition = spawnPoints[0].position;
            Quaternion spawnPointRotation = spawnPoints[0].rotation;

            if (!divisionMode)
            {
                switch (spawnMode)
                {
                    case SpawnMode.RANDOMCONTINUOUS:
                        foreach (Transform spawnPoint in spawnPoints)
                        {
                            spawnPointPosition += Random.Range(0f, 1f) * (spawnPoint.position - spawnPoints[0].position);
                        }
                        break;

                    case SpawnMode.RANDOMDISCRETE:
                    default:
                        int randomIndex = Random.Range(0, spawnPoints.Length);
                        spawnPointPosition = spawnPoints[randomIndex].position;
                        spawnPointRotation = spawnPoints[randomIndex].rotation;
                        break;
                }
            }
            else
            {
                spawnPointPosition = enemyMotherCell.transform.position;
            }

            if (!divisionMode)
            {
                enemyMotherCell = wave.enemyPrefab;
            }
            GameObject instantiatedEnemy = (GameObject)Instantiate(enemyMotherCell, spawnPointPosition, spawnPointRotation);

            enemy = instantiatedEnemy.GetComponent<Enemy>();
            enemy.initialize(wave, reward, health, startHealth, waypointIndex);

            enemiesAlive++;
        }
        return enemy;
    }
}

