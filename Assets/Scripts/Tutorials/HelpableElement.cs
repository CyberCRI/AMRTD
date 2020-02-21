using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HelpableElement : MonoBehaviour
{
    [SerializeField]
    private string codeStem = "";

    void OnMouseDown()
    {
#if DEVMODE
        Debug.Log("HelpableElement: OnMouseDown " + this.gameObject.name);
#endif

        if (HelpButtonUI.instance.isHelpModeOn())
        {
            string code = string.IsNullOrEmpty(codeStem)? this.gameObject.name.ToUpper() : codeStem;
            code = "GAME." + code + ".HELP";
#if DEVMODE
            Debug.Log("HelpableElement: code: " + code);
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
