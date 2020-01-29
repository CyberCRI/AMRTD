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
        Debug.Log("HelpableElementUI: Mouse Down: " + eventData.pointerCurrentRaycast.gameObject.name);

        if (HelpButtonUI.instance.isHelpModeOn())
        {
            string code = string.IsNullOrEmpty(codeStem)? this.gameObject.name.ToUpper() : codeStem;
            code = "GAME." + code + ".HELP";
            Debug.Log("HelpableElementUI: code: " + code);

            HelpButtonUI.instance.toggleHelpMode();

            FocusMaskManager.instance.focusOn(this.gameObject, callback, code, true, true);
        }
    }

    private void callback()
    {
        FocusMaskManager.instance.stopFocusOn();
    }
}
