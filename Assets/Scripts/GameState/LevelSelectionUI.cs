//#define VERBOSEDEBUG
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    public static LevelSelectionUI instance = null;

    public const string sceneName = "LevelSelectionMenu";

    [SerializeField]
    private Transform levelButtonsRoot = null;
    private Button[] levelButtons = null;
    public const int gameLevelCount = 7;

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
        int levelReached = GameConfiguration.instance.furthestLevel;

        for (int i = 0; i < levelButtons.Length; i++)
        {
#if VERBOSEDEBUG
            levelButtons[i].interactable = true;
#else
            levelButtons[i].interactable = (i <= levelReached);
#endif
        }
    }

    public void select(string levelName)
    {
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKLEVEL, new CustomData (CustomDataTag.GAMELEVEL, levelName));
        SceneFader.instance.fadeTo(levelName);
    }
    // called from level selection screen, to go back to main menu screen
    public void pressBackButton()
    {
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKBACK);
        SceneFader.instance.goToMainMenu();
    }

    public static void lockAllLevels()
    {
        GameConfiguration.instance.furthestLevel = 0;
        if (null != instance)
        {
            instance.updateInteractables();
        }
    }

    public static void unlockAllLevels()
    {
        GameConfiguration.instance.furthestLevel = gameLevelCount - 1;
        if (null != instance)
        {
            instance.updateInteractables();
        }
    }
}
