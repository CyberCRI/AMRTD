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
        int previous = PlayerPrefs.GetInt(LevelSelector.levelReachedKey);
        int newLevelReached = Mathf.Max(previous, nextLevelIndex);
        PlayerPrefs.SetInt(LevelSelector.levelReachedKey, newLevelReached);
        sceneFader.fadeTo(nextLevelName);
    }

    public void pressMenu()
    {
        sceneFader.menu();
    }
}
