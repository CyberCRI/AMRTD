﻿//#define DEVMODE
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool isGameOver = false;
    public static bool isLevelCompleted = false;

    public enum GAMEMODE
    {
        PATHS,
        DEFEND_CAPTURABLE_OBJECTIVES,
        COUNT
    }

    private GameObject gameOverUI = null;
    private GameObject completeLevelUI = null;

    [SerializeField]
    private Text levelDurationCountdownText = null;
    [SerializeField]
    private float levelDuration = 0f;
    private bool levelDurationCountdownMode = false;
    private float levelDurationCountdown = 0f;
    
    public GAMEMODE gameMode = GAMEMODE.PATHS;

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
            isGameOver = false;
            isLevelCompleted = false;

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

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            if (PlayerStatistics.lives <= 0)
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

                    if (null != levelDurationCountdownText)
                    {
                        levelDurationCountdownText.text = string.Format("{0:00.00}", levelDurationCountdown);   
                    }
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
#endif
        }
    }

    public bool isObjectiveDefenseMode()
    {
        return gameMode == GAMEMODE.DEFEND_CAPTURABLE_OBJECTIVES;
    }

    public void link(
        Text _levelDurationCountdownMode
        ,GameObject _gameOverUI
        ,GameObject _completeLevelUI
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
        isGameOver = true;
        gameOverUI.SetActive(true);
    }

    public void winLevel()
    {
        if (!isGameOver)
        {
            isLevelCompleted = true;
            completeLevelUI.SetActive(true);
        }
    }

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
        for (int i = 0; i < enemies.Length-1; i++)
        {
            GameObject enemyGO = enemies[i];
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            enemy.takeDamage(enemy.health);
        }
    }
}
