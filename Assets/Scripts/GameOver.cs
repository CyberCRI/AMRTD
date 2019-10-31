using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private SceneFader sceneFader = null;

    public void pressRetry()
    {
        sceneFader.fadeTo(SceneManager.GetActiveScene().name);
    }

    public void pressMenu()
    {
        sceneFader.menu();
    }
}
