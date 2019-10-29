using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string levelToLoad = "";

    public void play()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
