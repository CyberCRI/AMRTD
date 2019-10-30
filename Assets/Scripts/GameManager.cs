using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isGameOver = false;

    [SerializeField]
    private GameObject gameOverUI = null;
    [SerializeField]
    private SceneFader sceneFader = null;

    private string nextLevelName = "map2";
    private int nextLevelIndex = 1;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        isGameOver = false;
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

    public void winLevel()
    {
        Debug.Log("WON");
        PlayerPrefs.SetInt(LevelSelector.levelReachedKey, nextLevelIndex);
        sceneFader.fadeTo(nextLevelName);
    }
}
