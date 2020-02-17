//#define DEVMODE
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public static LevelSelector instance = null;
    
    public const string sceneName = "LevelSelectionMenu";

    [SerializeField]
    private Transform levelButtonsRoot = null;
    private Button[] levelButtons = null;
    public const string levelReachedKey = "levelReached";
    public const int maxLevelIndex = 6;

    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;

            levelButtons = new Button[levelButtonsRoot.childCount];
            CommonUtilities.fillArrayFromRoot<Button>(levelButtonsRoot, ref levelButtons);

            updateInteractables();
        }
    }

    private void updateInteractables()
    {
        int levelReached = PlayerPrefs.GetInt(levelReachedKey, 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
#if DEVMODE
            levelButtons[i].interactable = true;
#else
            levelButtons[i].interactable = (i <= levelReached);
#endif
        }
    }

    public void select(string levelName)
    {
        SceneFader.instance.fadeTo(levelName);
    }

    public static void deleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        if (null != instance)
        {
            instance.updateInteractables();
        }
    }

    public static void unlockAllLevels()
    {
        PlayerPrefs.SetInt(LevelSelector.levelReachedKey, maxLevelIndex);
        if (null != instance)
        {
            instance.updateInteractables();
        }
    }
}
