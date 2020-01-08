using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public const string sceneName = "MainMenu";

    public void play()
    {
        SceneFader.instance.fadeTo(LevelSelector.sceneName);
    }

    public void quit()
    {
        Application.Quit();
    }
}
