//#define VERBOSEDEBUG
//#define DEVMODE
using UnityEngine;
using UnityEngine.UI;

public class HelpButtonUI : MonoBehaviour
{
    public static HelpButtonUI instance = null;

    [SerializeField]
    private Toggle toggle = null;
    [SerializeField]
    private Texture2D helpCursorTexture = null;
    [SerializeField]
    private GameObject helpText = null;

    private bool selected = false;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    public bool isHelpModeOn()
    {
        return selected;
    }

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

#if DEVMODE
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            toggle.isOn = !toggle.isOn;
        }
    }
#endif

    public void toggleHelpMode()
    {
#if VERBOSEDEBUG
        Debug.Log("toggleHelpMode");
#endif
        // indicates what is the desired state of the button
        CustomDataValue customDataValue = selected ? CustomDataValue.OFF : CustomDataValue.ON;
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKHELP, new CustomData(CustomDataTag.OUTCOME, customDataValue));
        AudioManager.instance.play(AudioEvent.CLICKUI);

        // must be called before cursor setting, otherwise cancels it
        BuildManager.instance.deselectTurretButton();
        selected = !selected;
        setHelpCursor(selected);
        if (null != helpText)
        {
            helpText.SetActive(selected);
        }
    }

    public void hasClickedOnHelpable()
    {
#if VERBOSEDEBUG
        Debug.Log("hasClickedOnHelpable");
#endif
        toggle.isOn = false;
    }

    private void setHelpCursor(bool helpOn)
    {
        if (helpOn)
        {
            Cursor.SetCursor(helpCursorTexture, hotSpot, cursorMode);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }
    }
}
