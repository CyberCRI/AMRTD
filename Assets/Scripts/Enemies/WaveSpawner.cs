//#define VERBOSEDEBUG
//#define VERBOSEMETRICSLVL2
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance;

    private int _enemiesAliveCount = 0;
    public int enemiesAliveCount { 
        get
        {
            return _enemiesAliveCount;
        }
        set
        {
            _enemiesAliveCount = value;
            if (enemiesAliveCount >= currentWave.maxEnemyCount)
            {
                divisionGreenLightCountdown = divisionGreenLightTime;
            }
        }
    }
    private float divisionGreenLightCountdown = 0f;
    public float divisionGreenLightTime = 1f;
    public Enemy[] enemiesAlive { get; private set; } = new Enemy[0];

#if VERBOSEDEBUG
    [SerializeField]
#endif
    private int waveIndex = 0;
    private float countdown = 0f;
    private bool isDoneSpawning = true;
    private Transform[] spawnPoints = null;
    private Text waveCountdownText = null;
    private LocalizedText waveCountdownLocalizedText = null;
    private const string waveCountdownSpawningString = "GAME.WAVECOUNTDOWN.NOCOUNTDOWN";

    private float divisionDistance = 1f;
    [SerializeField]
    private float resistancePointsRatioVictoryThreshold = 0f;
    [SerializeField]
    private bool isResistanceModeOn = true;
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
    private bool prepared = false;
    [SerializeField]
    private bool sendWBCMsgOnLastWave = false;

    private enum SpawnMode
    {
        RANDOMDISCRETE,
        RANDOMCONTINUOUS
    };

    public float getWaveProgression()
    {
        return ((float)waveIndex) / ((float)waves.Length);
    }

    public bool isLastWave()
    {
        return (waveIndex == (waves.Length - 1)) && (0f == countdown);
    }

    public bool isNthWaveStarting(int n)
    {
        return (waveIndex == n - 1) && (0f == countdown);
    }

    public bool isNthWaveDone(int n)
    {
        return (waveIndex == n) && (0f != countdown) && prepared;
    }

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
            divisionGreenLightCountdown = divisionGreenLightTime;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if ((null != currentWave) && (enemiesAliveCount < currentWave.maxEnemyCount))
        {
            divisionGreenLightCountdown -= Time.deltaTime;
        }

        if (
            isDoneSpawning
            &&
            (
                (GameManager.instance.isObjectiveDefenseMode())
                ||
                (enemiesAliveCount <= 0)
            )
        )
        {
            if (countdown <= 0f)
            {
                if (waveIndex < waves.Length)
                {
                    waveCountdownLocalizedText.enabled = true;
                    waveCountdownLocalizedText.setKey(waveCountdownSpawningString);
                    StartCoroutine(spawnWave());
                    // waveIndex is updated only when the coroutine ends
                    if (PlayerStatistics.instance.waves < waves.Length - 1)
                    {
                        countdown = timeBetweenWaves;
                        prepared = false;
                    }
                    else if (sendWBCMsgOnLastWave)
                    {
                        WhiteBloodCellManager.instance.triggerMassWBC();
                    }
                }
                else if (enemiesAliveCount <= 0)
                {
                    if (GameManager.instance.isObjectiveDefenseMode()
                    || (PlayerStatistics.instance.resistancePointsRatio <= resistancePointsRatioVictoryThreshold))
                    {
                        gameManager.winLevel();
                    }
                    else
                    {
                        gameManager.loseLevel();
                    }
                    this.enabled = false;
                }
            }
            else
            {
                if (!prepared)
                {
                    waveCountdownLocalizedText.enabled = false;
                    waveCountdownLocalizedText.setKey("");
                    prepared = true;
                }
                countdown -= Time.deltaTime;
                countdown = Mathf.Max(countdown, 0f);
                waveCountdownText.text = Mathf.FloorToInt(countdown).ToString("D");
            }
        }
    }

    public void linkUI(
        Text _waveCountdownText
    )
    {
        waveCountdownText = _waveCountdownText;
        waveCountdownLocalizedText = waveCountdownText.gameObject.GetComponent<LocalizedText>();
    }

    public void linkMap(
        Transform[] _spawnPoints
        )
    {
        spawnPoints = new Transform[_spawnPoints.Length];
        _spawnPoints.CopyTo(spawnPoints, 0);
    }

    public bool isEnemyDivisionAllowed()
    {
        return (0f >= divisionGreenLightCountdown);
    }

    public void setCountdownToZero()
    {
        Debug.Log("setCountdownToZero");
        countdown = 0f;
    }

    private float[] getCurrentResistances()
    {
        return Enumerable.Repeat(
            1f - PlayerStatistics.instance.resistancePointsRatio,
            (int)Attack.SUBSTANCE.COUNT
            ).ToArray();
    }

    private IEnumerator spawnWave()
    {
        isDoneSpawning = false;
        yield return new WaitForSeconds(1f);
        PlayerStatistics.instance.waves++;
        currentWave = waves[waveIndex];
        enemiesAlive = new Enemy[currentWave.maxEnemyCount];

        for (int i = 0; i < currentWave.count; i++)
        {
            float[] resistances = isResistanceModeOn ? getCurrentResistances() : null;
            spawnEnemy(currentWave, resistances);
            yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
        }

        waveIndex++;
        isDoneSpawning = true;
    }

    public Enemy spawnEnemy(
        Wave wave,
        // overrides (TODO: increases) the resistance of the instantiated enemy
        float[] resistances = null,
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

        if (enemiesAliveCount < wave.maxEnemyCount)
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
                float angle = Random.Range(0f, Mathf.PI);
                //divisionDistance
                Vector3 divisionDisplacement = new Vector3(
                    divisionDistance * Mathf.Cos(angle),
                    0f,
                    divisionDistance * Mathf.Sin(angle)
                );
                spawnPointPosition = enemyMotherCell.transform.position + divisionDisplacement;
                enemyMotherCell.transform.position = enemyMotherCell.transform.position - divisionDisplacement;
            }

            if (!divisionMode)
            {
                enemyMotherCell = wave.enemyPrefab;
            }
            GameObject instantiatedEnemy = Instantiate(enemyMotherCell, spawnPointPosition, spawnPointRotation);
            #if VERBOSEMETRICSLVL2
            if (!divisionMode)
            {
                RedMetricsManager.instance.sendEvent(TrackingEvent.PATHOGENSPAWNS, CustomData.getGameObjectContext(instantiatedEnemy));
            }
            #endif

            enemy = instantiatedEnemy.GetComponent<Enemy>();
            enemy.initialize(wave, reward, health, startHealth, waypointIndex, resistances);

            enemiesAliveCount++;
            for (int i = 0; i < enemiesAlive.Length; i++)
            {
                if (null == enemiesAlive[i])
                {
                    enemiesAlive[i] = enemy;
                    break;
                }
            }
        }
        return enemy;
    }
}

