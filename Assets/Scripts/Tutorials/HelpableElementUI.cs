//#define DEVMODE
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HelpableElementUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private string codeStem = "";

    public void OnPointerDown(PointerEventData eventData)
    {
#if DEVMODE
        Debug.Log("HelpableElementUI: Mouse Down: " + eventData.pointerCurrentRaycast.gameObject.name);
#endif

        if (HelpButtonUI.instance.isHelpModeOn())
        {
            string code = string.IsNullOrEmpty(codeStem)? this.gameObject.name.ToUpper() : codeStem;
            code = "GAME." + code + ".HELP";
#if DEVMODE
            Debug.Log("HelpableElementUI: code: " + code);
#endif

            //HelpButtonUI.instance.toggleHelpMode();
            HelpButtonUI.instance.hasClickedOnHelpable();

            FocusMaskManager.instance.focusOn(this.gameObject, callback, code, true, true);
        }
    }

    private void callback()
    {
        FocusMaskManager.instance.stopFocusOn();
    }
}
