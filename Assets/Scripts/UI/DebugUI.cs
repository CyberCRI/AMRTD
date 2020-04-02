#define DEVMODE
//#define EXPERIMENTAL
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField]
    private GameObject debugCanvasRoot = null;

#if DEVMODE || EXPERIMENTAL
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            deleteAllPlayerPrefs();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            unlockAllLevels();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            switchLanguage();
        }
    }

    public void deleteAllPlayerPrefs()
    {
        Debug.LogWarning("Deleting all PlayerPrefs!");
        PlayerPrefs.DeleteAll();
    }

    public void lockAllLevels()
    {
        LevelSelectionUI.lockAllLevels();
    }

    public void unlockAllLevels()
    {
        LevelSelectionUI.unlockAllLevels();
    }

    public void switchLanguage()
    {
        LocalizationManager.instance.language = LocalizationManager.instance.getNextLanguage();
    }
#else
    void Start()
    {
        debugCanvasRoot.SetActive(false);
    }
#endif
}
