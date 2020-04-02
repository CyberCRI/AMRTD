using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public const string sceneName = "MainMenu";
    
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
        SceneFader.instance.fadeTo(LevelSelector.sceneName);
    }

    public void quit()
    {
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKQUIT);
        Application.Quit();
    }
}
