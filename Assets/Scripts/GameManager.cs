//#define DEVMODE
using UnityEngine;

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

    [SerializeField]
    private GameObject gameOverUI = null;
    [SerializeField]
    private GameObject completeLevelUI = null;
    
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
