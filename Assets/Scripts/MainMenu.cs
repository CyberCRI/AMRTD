using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string levelToLoad = "";
    [SerializeField]
    private SceneFader sceneFader = null;

    public const string sceneName = "MainMenu";


    public void play()
    {
        sceneFader.fadeTo(levelToLoad);
    }

    public void quit()
    {
        Application.Quit();
    }
}
