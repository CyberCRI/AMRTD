using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpableElementParent : MonoBehaviour
{
    [SerializeField]
    private string codeStem = "";

    protected void click()
    {
#if DEVMODE
        Debug.Log("HelpableElementParent: click " + this.gameObject.name);
#endif

        if (HelpButtonUI.instance.isHelpModeOn())
        {
            string code = string.IsNullOrEmpty(codeStem)? this.gameObject.name.ToUpper() : codeStem;
            code = "GAME." + code + ".HELP";
#if DEVMODE
            Debug.Log("HelpableElementParent: code: " + code);
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
