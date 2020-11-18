//#define VERBOSEDEBUG

using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public const string sceneName = "MainMenu_simple";
    public const string sceneNameAlternate = "MainMenu";

    [Tooltip("Minimum number of levels unlocked required to access the level selection screen. Otherwise, loads the furthest unlocked level.")]
    [SerializeField]
    private int minReqUnlocks = 0;
    
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void play()
    {
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKPLAY);
        AudioManager.instance.play(AudioEvent.CLICKUI);

        int levelReached = GameConfiguration.instance.furthestLevel;

#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " play levelReached=" + levelReached + ", minReqUnlocks=" + minReqUnlocks);
#endif
        if (levelReached > minReqUnlocks)
        {
            // open level selection screen
            SceneFader.instance.fadeTo(LevelSelectionUI.sceneName);
        }
        else
        {
            // open furthest unlocked level
            SceneFader.instance.fadeTo(levelReached);
        }
    }

    public void playIntroAgain()
    {
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKGAMEINTRO);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        GameConfiguration.instance.showIntro = true;
        SceneFader.instance.fadeTo(GameIntroUI.sceneName);
    }

    public void showCredits()
    {
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKCREDITS);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        SceneFader.instance.fadeTo(GameCreditsUI.sceneName);
    }

    public void quit()
    {
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKQUIT);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        Application.Quit();
    }
}
