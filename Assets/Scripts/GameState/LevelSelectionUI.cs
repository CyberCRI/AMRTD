//#define VERBOSEDEBUG
//#define DEVMODE
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    public static LevelSelectionUI instance = null;

    public const string sceneName = "LevelSelectionMenu";

    [SerializeField]
    private Transform levelButtonsRoot = null;
    private Button[] levelButtons = null;

    public const string lastScene = "Level8_Lungs";

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

        levelButtons[0].interactable = true;
        for (int i = 1; i < levelButtons.Length; i++)
        {
#if DEVMODE
            levelButtons[i].interactable = true;
#else
            levelButtons[i].interactable = (i <= levelReached - GameConfiguration.tutorialLevelsCount);
#endif
        }
    }

    public void select(string levelName)
    {
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKLEVEL, new CustomData (CustomDataTag.OPTION, levelName));
        AudioManager.instance.play(AudioEvent.CLICKUI);
        SceneFader.instance.fadeTo(levelName);
    }
    // called from level selection screen, to go back to main menu screen
    public void pressBackButton()
    {
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKBACK);
        AudioManager.instance.play(AudioEvent.CLICKUI);
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
        GameConfiguration.instance.furthestLevel = GameConfiguration.gameLevelCount - 1;
        if (null != instance)
        {
            instance.updateInteractables();
        }
    }
}
