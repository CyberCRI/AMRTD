using UnityEngine;

public class DebugUI : MonoBehaviour
{
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
    }

    public void deleteAllPlayerPrefs()
    {
        LevelSelector.deleteAllPlayerPrefs();
    }

    public void unlockAllLevels()
    {
        LevelSelector.unlockAllLevels();
    }
}
