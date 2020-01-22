//#define DEVMODE
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public static MenuUI instance = null;
    private GameObject menuUI = null;

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
        if (Input.GetKeyDown(KeyCode.M))
        {
            toggle();
        }
    }
#endif

    public void linkUI(GameObject _menuUI)
    {
        menuUI = _menuUI;
    }

    public void toggle()
    {
        GameManager.instance.togglePause();
        menuUI.SetActive(GameManager.instance.isPaused());
    }
}
