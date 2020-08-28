using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSwitchUI : MonoBehaviour
{
    public void goToOther()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string targetScene = currentScene == MainMenu.sceneName ? MainMenu.sceneNameAlternate : MainMenu.sceneName;

        SceneFader.instance.fadeTo(targetScene);
    }
}
