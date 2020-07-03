#define VERBOSEDEBUG
#define DEVMODE
//#define LIFEPOINTSMODE
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

// the value "true" means that pause was set to true
[System.Serializable]
public class PauseEvent : UnityEvent<bool>{}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool isLevelLost = false;
    public static bool isLevelWon = false;
    public PauseEvent pauseSet = new PauseEvent();
    
    public enum GAMEMODE
    {
        PATHS,
        DEFEND_CAPTURABLE_OBJECTIVES,
        COUNT
    }

    private GameObject gameOverUI = null;
    private Text levelDurationCountdownText = null;
    [SerializeField]
    private float levelDuration = 0f;
    private bool levelDurationCountdownMode = false;
    private float levelDurationCountdown = 0f;

    [SerializeField]
    private float normalSpeed = 1f;
    [SerializeField]
    private float highSpeed = 4f;
    #if VERBOSEDEBUG
    [SerializeField]
    private float timeScale = 0f;
    #endif

    public const string focusMaskManagerPauserKey = "FocusMaskManager";
    public const string loadingScreenManagerUIPauserKey = "LoadingScreenManagerUI";
    public const string menuUIPauserKey = "MenuUI";
    public const string pauseUIPauserKey = "PauseUI";
    public const string retryUIPauserKey = "RetryUI";
    public const string chatbotUIPauserKey = "ChatbotUI";
    public const string levelIntroUIPauserKey = "LevelIntroUI";
    public const string gameOverPauserKey = "GameOver";
    public const string completeLevelPauserKey = "CompleteLevel";

    private Dictionary<string, bool> pausers = new Dictionary<string, bool>(){
        {focusMaskManagerPauserKey, false},
        {loadingScreenManagerUIPauserKey, false},
        {menuUIPauserKey, false},
        {pauseUIPauserKey, false},
        {retryUIPauserKey, false},
        {chatbotUIPauserKey, false},
        {levelIntroUIPauserKey, false},
        {gameOverPauserKey, false},
        {completeLevelPauserKey, false},
    };

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
        #if VERBOSEDEBUG
        timeScale = Time.timeScale;
        #endif
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
                RedMetricsManager.instance.sendEvent(TrackingEvent.DEVPRESSLOSE);
                loseLevel();
            }

            if (Input.GetKeyDown(KeyCode.Home))
            {
                RedMetricsManager.instance.sendEvent(TrackingEvent.DEVPRESSWIN);
                winLevel();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                RedMetricsManager.instance.sendEvent(TrackingEvent.DEVPRESSINJUREALL);
                injureAllEnemies();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                RedMetricsManager.instance.sendEvent(TrackingEvent.DEVPRESSKILLALLBUTONE);
                killAllButOneEnemy();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                RedMetricsManager.instance.sendEvent(TrackingEvent.DEVPRESSDESTROYALLTURRETS);
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
    }

    public void loseLevel()
    {
        if (!isLevelWon)
        {
            RedMetricsManager.instance.sendEvent(TrackingEvent.GAMEOVER, CustomData.getLevelEndContext());
            AudioManager.instance.play(AudioEvent.GAMEOVER);
            isLevelLost = true;
            setPause(true, gameOverPauserKey);
            gameOverUI.SetActive(true);
        }
    }

    public void winLevel()
    {
        if (!isLevelLost)
        {
            isLevelWon = true;
            CompleteLevel.instance.completeLevel();
        }
    }

    private void debugPausers()
    {
        string debugString = null;
        foreach(string _key in pausers.Keys)
        {
            if (null != debugString)
            {
                debugString += " ,";
            }
            debugString += _key + ": " + pausers[_key].ToString();
        }
        debugString = "pausers[" + debugString + "]";
        Debug.Log(debugString);
    }

    private void resetPausers()
    {
        foreach(string _key in pausers.Keys)
        {
            pausers[_key] = false;
        }
    }

    private bool isAskedToPause()
    {
        return pausers.ContainsValue(true);
    }

    public void setPause(bool setToPause, string caller) //, bool overrideAll = false)
    {
        #if VERBOSEDEBUG
        Debug.Log("setPause(" + setToPause + ", " + caller + ")"); //", " + overrideAll + ")");
        #endif

        /*
        if (overrideAll && !setToPause)
        {
            foreach(string _key in pausers.Keys)
            {
                pausers[_key] = setToPause;
            }
        }
        else
        {
        */
        if (pausers[caller] != setToPause)
        {
            bool isPausedBefore = isPaused();
            pausers[caller] = setToPause;
            bool isPausedAfter = isAskedToPause();
            if (isPausedBefore != isPausedAfter)
            {
                Time.timeScale = isPausedAfter ? 0f : 1f;
                pauseSet.Invoke(isPausedAfter);
            }
        }
    }

    public void togglePause()
    {
        Time.timeScale = 1f - Time.timeScale;
        
        pauseSet.Invoke(isPaused());
    }

    public bool isPaused()
    {
        return (Time.timeScale == 0f);
    }

    public void toggleHighSpeed()
    {
        if (!isPaused())
        {
            Time.timeScale = highSpeed + normalSpeed - Time.timeScale;
        }
    }

    public void setHighSpeed()
    {
        setPause(false, pauseUIPauserKey);

        Time.timeScale = highSpeed;
    }

    public void setNormalSpeed()
    {
        setPause(false, pauseUIPauserKey);

        Time.timeScale = normalSpeed;
    }

#if DEVMODE
    private void injureAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Enemy.enemyTag);
        foreach (GameObject enemyGO in enemies)
        {
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            enemy.takeDamage(enemy.health / 2, Attack.SOURCE.DEVDMGHALF);
        }
    }

    private void killAllButOneEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Enemy.enemyTag);
        for (int i = 0; i < enemies.Length - 1; i++)
        {
            GameObject enemyGO = enemies[i];
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            enemy.takeDamage(enemy.health, Attack.SOURCE.DEVDMGALL);
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
