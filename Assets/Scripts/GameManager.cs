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
        }
    }

    private void endGame()
    {
        isGameOver = true;
        gameOverUI.SetActive(true);
    }

    public void completeLevel()
    {
        isLevelCompleted = true;
        completeLevelUI.SetActive(true);
    }
}
