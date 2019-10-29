using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Text wavesText = null;
    [SerializeField]
    private SceneFader sceneFader = null;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        wavesText.text = PlayerStatistics.waves.ToString();
    }

    public void pressRetry()
    {
        sceneFader.fadeTo(SceneManager.GetActiveScene().name);
    }

    public void pressMenu()
    {
        sceneFader.fadeTo(MainMenu.sceneName);
    }
}
