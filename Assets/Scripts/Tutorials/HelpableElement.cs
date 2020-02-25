//#define DEVMODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HelpableElement : HelpableElementParent
{
    void OnMouseDown()
    {
#if DEVMODE
        Debug.Log("HelpableElement: OnMouseDown " + this.gameObject.name);
#endif
        click();
    }
}
