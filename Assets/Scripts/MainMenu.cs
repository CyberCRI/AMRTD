using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string levelToLoad = "";

    public const string sceneName = "MainMenu";

    public void play()
    {
        SceneFader.instance.fadeTo(levelToLoad);
    }

    public void quit()
    {
        Application.Quit();
    }
}
