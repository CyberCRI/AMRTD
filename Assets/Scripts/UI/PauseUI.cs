//#define DEVMODE
//#define VERBOSEDEBUG
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public static PauseUI instance = null;
    private GameObject pauseUI = null;
    [SerializeField]
    private Toggle m_Toggle = null;
    private bool ignoreToggle = true;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        GameManager.instance.pauseSet.AddListener(updateToggleState);
        ignoreToggle = false;
    }

#if DEVMODE
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            toggle();
        }
    }
#endif

    public void linkUI(GameObject _pauseUI, Toggle _toggle)
    {
        pauseUI = _pauseUI;
        m_Toggle = _toggle;

        m_Toggle.onValueChanged.AddListener ( (value) => { toggle(); } );
    }

    private void updateToggleState(bool setToPause)
    {
        if (null != m_Toggle)
        {
            bool previousIgnoreToggleState = ignoreToggle;
            ignoreToggle = true;
            m_Toggle.isOn = setToPause;
            ignoreToggle = previousIgnoreToggleState;
        }
    }

    public void toggle()
    {
    #if VERBOSEDEBUG
        Debug.Log("PauseUI toggle outer");
    #endif
        if (!ignoreToggle)
        {
            #if VERBOSEDEBUG
                Debug.Log("PauseUI toggle inner");
            #endif

            CustomDataValue customDataValue = m_Toggle.isOn ? CustomDataValue.ON : CustomDataValue.OFF;
            RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKPAUSE, new CustomData(CustomDataTag.OUTCOME, customDataValue));
            AudioManager.instance.play(AudioEvent.CLICKUI);

            GameManager.instance.setPause(m_Toggle.isOn, GameManager.PAUSER.PAUSEUI);
        }
    }
}
