using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public static PauseUI instance = null;
    private GameObject pauseUI = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            toggle();
        }
    }

    public void linkUI(GameObject _pauseUI)
    {
        pauseUI = _pauseUI;
    }

    public void toggle()
    {
        GameManager.instance.togglePause();
        pauseUI.SetActive(GameManager.instance.isPaused());
    }
}
