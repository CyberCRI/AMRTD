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
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            lockAllLevels(true);
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            unlockAllLevels(true);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            switchLanguage();
        }
    }

    public void deleteAllPlayerPrefs()
    {
        Debug.LogWarning("Deleting all PlayerPrefs!");
        RedMetricsManager.instance.sendEvent(TrackingEvent.DEVPRESSDELETEPLAYERPREFS);
        PlayerPrefs.DeleteAll();
    }

    public void lockAllLevels(bool pressed = false)
    {
        TrackingEvent e = pressed ? TrackingEvent.DEVPRESSLEVELSLOCK : TrackingEvent.DEVCLICKLEVELSLOCK;
        RedMetricsManager.instance.sendEvent(e);
        LevelSelectionUI.lockAllLevels();
    }

    public void unlockAllLevels(bool pressed = false)
    {
        TrackingEvent e = pressed ? TrackingEvent.DEVPRESSLEVELSUNLOCK : TrackingEvent.DEVCLICKLEVELSUNLOCK;
        RedMetricsManager.instance.sendEvent(e);
        LevelSelectionUI.unlockAllLevels();
    }

    public void switchLanguage()
    {
        LocalizationManager.instance.language = LocalizationManager.instance.getNextLanguage();
        RedMetricsManager.instance.sendEvent(TrackingEvent.DEVPRESSLANGUAGE, CustomData.getContext(CustomDataTag.LANGUAGE));
    }
#else
    void Start()
    {
        debugCanvasRoot.SetActive(false);
    }
#endif
}
