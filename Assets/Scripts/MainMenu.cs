using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string levelToLoad = "";
    [SerializeField]
    private SceneFader sceneFader = null;

    public static string sceneName { get { return "MainMenu"; } }


    public void play()
    {
        sceneFader.fadeTo(levelToLoad);
    }

    public void quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
