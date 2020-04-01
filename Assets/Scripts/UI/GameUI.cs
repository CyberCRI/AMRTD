//#define VERBOSEDEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public static GameUI instance = null;

    [SerializeField]
    private Canvas canvas = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void pressPauseButton()
    {
#if VERBOSEDEBUG
        Debug.Log("GameUI pressPauseButton");
#endif
        PauseUI.instance.toggle();
    }

    public void pressContinueButton()
    {
        PauseUI.instance.toggle();
    }

    public void pressRetryButton()
    {
        GameManager.instance.setPause(false);
        SceneFader.instance.retry();
    }

    public void pressGameRetryButton()
    {
        RetryUI.instance.toggle();
    }

    public void pressMenuButton()
    {
        GameManager.instance.setPause(false);
        SceneFader.instance.menu();
    }

    public void pressGameMenuButton()
    {
        MenuUI.instance.toggle();
    }

    public void pressCompleteLevelButton()
    {
        CompleteLevel.instance.pressContinue();
    }

    public void linkCommon(Camera camera)
    {
        canvas.worldCamera = camera;
    }
}
