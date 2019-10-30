using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField]
    private SceneFader sceneFader = null;
    [SerializeField]
    private Button[] levelButtons = null;
    public const string levelReachedKey = "levelReached";  

    void Awake()
    {
        int levelReached = PlayerPrefs.GetInt(levelReachedKey, 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i > levelReached)
            levelButtons[i].interactable = false;
        }
    }   

    public void select(string levelName)
    {
        sceneFader.fadeTo(levelName);
    }
}
