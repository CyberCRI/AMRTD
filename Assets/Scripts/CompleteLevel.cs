using UnityEngine;

public class CompleteLevel : MonoBehaviour
{
    [SerializeField]
    private SceneFader sceneFader = null;
    [SerializeField]
    private string nextLevelName = "";
    [SerializeField]
    private int nextLevelIndex = 0;

    public void pressContinue()
    {
        PlayerPrefs.SetInt(LevelSelector.levelReachedKey, nextLevelIndex);
        sceneFader.fadeTo(nextLevelName);
    }

    public void pressMenu()
    {
        sceneFader.fadeTo(MainMenu.sceneName);
    }
}
