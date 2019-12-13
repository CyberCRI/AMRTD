using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public void pressPauseButton()
    {
        PauseUI.instance.toggle();
    }

    public void pressContinueButton()
    {
        PauseUI.instance.toggle();
    }

    public void pressRetryButton()
    {
        Time.timeScale = 1f;
        SceneFader.instance.retry();
    }

    public void pressMenuButton()
    {
        Time.timeScale = 1f;
        SceneFader.instance.menu();
    }

    public void pressCompleteLevelButton()
    {
        CompleteLevel.instance.pressContinue();
    }
}
