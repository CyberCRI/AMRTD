using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [SerializeField]
    private GameObject ui = null;
    [SerializeField]
    private SceneFader sceneFader = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            toggle();
        }
    }

    public void toggle()
    {
        ui.SetActive(!ui.activeSelf);

        if (ui.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void retry()
    {
        //toggle();
        Time.timeScale = 1f;
        sceneFader.fadeTo(SceneManager.GetActiveScene().name);
    }

    public void menu()
    {
        //toggle();
        Time.timeScale = 1f;
        sceneFader.menu();
    }
}
