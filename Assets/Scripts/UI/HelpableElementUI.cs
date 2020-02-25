//#define DEVMODE
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HelpableElementUI : HelpableElementParent, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
#if DEVMODE
        Debug.Log("HelpableElementUI: OnPointerDown: " + eventData.pointerCurrentRaycast.gameObject.name);
#endif
        click();
    }
}
