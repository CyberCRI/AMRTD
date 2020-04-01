//#define VERBOSEDEBUG
//#define DEVMODE
//#define LIFEPOINTSMODE
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool isLevelLost = false;
    public static bool isLevelWon = false;

    public enum GAMEMODE
    {
        PATHS,
        DEFEND_CAPTURABLE_OBJECTIVES,
        COUNT
    }

    private GameObject gameOverUI = null;
    private GameObject completeLevelUI = null;
    private Text levelDurationCountdownText = null;
    [SerializeField]
    private float levelDuration = 0f;
    private bool levelDurationCountdownMode = false;
    private float levelDurationCountdown = 0f;

    [SerializeField]
    private GAMEMODE gameMode = GAMEMODE.PATHS;

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
            isLevelLost = false;
            isLevelWon = false;

            levelDurationCountdownMode = (levelDuration != 0f);
            if (levelDurationCountdownMode)
            {
                levelDurationCountdown = levelDuration;
            }
            else
            {
                if (null != levelDurationCountdownText)
                {
                    Destroy(levelDurationCountdownText.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (isLevelLost || isLevelWon)
        {
            this.enabled = false;
        }
        else
        {
            if ((PlayerStatistics.instance.lives <= 0)
#if LIFEPOINTSMODE
             || (PlayerStatistics.instance.lifePoints <= 0)
#endif
            )
            {
                loseLevel();
            }
            else if (levelDurationCountdownMode)
            {
                if (levelDurationCountdown <= 0f)
                {
                    loseLevel();
                }
                else
                {
                    levelDurationCountdown -= Time.deltaTime;
                    levelDurationCountdown = Mathf.Max(levelDurationCountdown, 0f);

                    levelDurationCountdownText.text = Mathf.FloorToInt(levelDurationCountdown).ToString("D");
                }
            }
#if DEVMODE
            if (Input.GetKeyDown(KeyCode.End))
            {
                loseLevel();
            }

            if (Input.GetKeyDown(KeyCode.Home))
            {
                winLevel();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                injureAllEnemies();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                killAllButOneEnemy();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                destroyAllTurrets();
            }
#endif
        }
    }

    public bool isObjectiveDefenseMode()
    {
        return gameMode == GAMEMODE.DEFEND_CAPTURABLE_OBJECTIVES;
    }

    public void linkUI(
        Text _levelDurationCountdownMode
        , GameObject _gameOverUI
        , GameObject _completeLevelUI
        )
    {
        if (levelDurationCountdownMode)
        {
            levelDurationCountdownText = _levelDurationCountdownMode;
        }
        else
        {
            Destroy(_levelDurationCountdownMode.gameObject);
        }

        gameOverUI = _gameOverUI;
        completeLevelUI = _completeLevelUI;
    }

    public void loseLevel()
    {
        if (!isLevelWon)
        {
            isLevelLost = true;
            gameOverUI.SetActive(true);
        }
    }

    public void winLevel()
    {
        if (!isLevelLost)
        {
            isLevelWon = true;
            completeLevelUI.SetActive(true);
        }
    }

    public void setPause(bool setToPause)
    {
        Time.timeScale = setToPause ? 0f : 1f;
    }

    public void togglePause()
    {
        Time.timeScale = 1f - Time.timeScale;
    }

    public bool isPaused()
    {
        return Time.timeScale == 0f;
    }

#if VERBOSEDEBUG
    private void injureAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Enemy.enemyTag);
        foreach (GameObject enemyGO in enemies)
        {
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            enemy.takeDamage(enemy.health / 2);
        }
    }

    private void killAllButOneEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Enemy.enemyTag);
        for (int i = 0; i < enemies.Length - 1; i++)
        {
            GameObject enemyGO = enemies[i];
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            enemy.takeDamage(enemy.health);
        }
    }

    private void destroyAllTurrets()
    {
        GameObject[] turrets = GameObject.FindGameObjectsWithTag(Turret.turretTag);
        for (int i = 0; i < turrets.Length; i++)
        {
            GameObject turretGO = turrets[i];
            Turret turret = turretGO.GetComponent<Turret>();
            turret.selfDestruct(Node.REMOVETOWER.DAMAGED);
        }
    }
#endif
}
