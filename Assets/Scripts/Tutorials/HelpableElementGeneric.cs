using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpableElementGeneric : MonoBehaviour
{
    [SerializeField]
    private string codeStem = "";

    protected void click()
    {
#if VERBOSEDEBUG
        Debug.Log("HelpableElementGeneric: click " + this.gameObject.name);
#endif

        if (HelpButtonUI.instance.isHelpModeOn())
        {
            string code = string.IsNullOrEmpty(codeStem)? this.gameObject.name.ToUpper() : codeStem;
            code = "GAME." + code + ".HELP";
#if VERBOSEDEBUG
            Debug.Log("HelpableElementGeneric: code: " + code);
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
