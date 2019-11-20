#define DEVMODE
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isGameOver = false;
    public static bool isLevelCompleted = false;

    [SerializeField]
    private GameObject gameOverUI = null;
    [SerializeField]
    private GameObject completeLevelUI = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        isGameOver = false;
        isLevelCompleted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            if (PlayerStatistics.lives <= 0)
            {
                endGame();
            }
#if DEVMODE
            if (Input.GetKeyDown(KeyCode.End))
            {
                completeLevel();
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

    private void endGame()
    {
        isGameOver = true;
        gameOverUI.SetActive(true);
    }

    public void completeLevel()
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
