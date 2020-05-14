//#define DEVMODE
using UnityEngine;

public class RetryUI : MonoBehaviour
{
    public static RetryUI instance = null;
    private GameObject retryUI = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
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

    public void linkUI(GameObject _retryUI)
    {
        retryUI = _retryUI;
    }
}
