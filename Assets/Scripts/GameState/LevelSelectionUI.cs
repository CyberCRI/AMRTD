//#define VERBOSEDEBUG
//#define DEVMODE
//#define UNLOCKALLLEVELS
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    public static LevelSelectionUI instance = null;

    public const string sceneName = "LevelSelectionMenu_simple";

    [SerializeField]
    private Transform levelButtonsRoot = null;
    private Button[] levelButtons = null;

    public const string lastScene = "Level4_resistance";

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
        }
    }

    void Start()
    {
        updateInteractables();
    }

    private void updateInteractables()
    {
        int levelReached = GameConfiguration.instance.furthestLevel;

        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " levelReached=" + levelReached);
        #endif

        levelButtons[0].interactable = true;
        for (int i = 0; i < levelButtons.Length; i++)
        {
#if DEVMODE || UNLOCKALLLEVELS
            levelButtons[i].interactable = true;
#else
            levelButtons[i].interactable = (i + GameConfiguration.tutorialLevelsCount<= levelReached);

            #if VERBOSEDEBUG
            Debug.Log(this.GetType() + " " + GameConfiguration.instance.getSceneName(i + GameConfiguration.tutorialLevelsCount) + " =" + levelButtons[i].interactable);
            #endif
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
