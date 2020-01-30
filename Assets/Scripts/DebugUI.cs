//#define DEVMODE
//#define EXPERIMENTAL
using UnityEngine;

public class DebugUI : MonoBehaviour
{
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
        LevelSelector.deleteAllPlayerPrefs();
    }

    public void unlockAllLevels()
    {
        LevelSelector.unlockAllLevels();
    }

    public void switchLanguage()
    {
        LocalizationManager.instance.language = LocalizationManager.instance.getNextLanguage();
    }
#else
    void Start()
    {
        this.gameObject.SetActive(false);
    }
#endif
}
