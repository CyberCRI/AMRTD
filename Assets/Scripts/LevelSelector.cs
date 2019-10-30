using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public static LevelSelector instance = null;

    [SerializeField]
    private SceneFader sceneFader = null;
    [SerializeField]
    private Button[] levelButtons = null;
    public const string levelReachedKey = "levelReached";
    public const int maxLevelIndex = 2;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        updateInteractables();
    }
    
    private void updateInteractables()
    {
        int levelReached = PlayerPrefs.GetInt(levelReachedKey, 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i <= levelReached);
        }
    }

    public void select(string levelName)
    {
        sceneFader.fadeTo(levelName);
    }

    public static void deleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        if (instance != null)
        {
            instance.updateInteractables();
        }
    }

    public static void unlockAllLevels()
    {
        PlayerPrefs.SetInt(LevelSelector.levelReachedKey, maxLevelIndex);
        if (instance != null)
        {
            instance.updateInteractables();
        }
    }
}
