//#define VERBOSEDEBUG

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HelpableElementUI : HelpableElementGeneric, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
#if VERBOSEDEBUG
        Debug.Log("HelpableElementUI: OnPointerDown: " + eventData.pointerCurrentRaycast.gameObject.name);
#endif
        click();
    }
}
