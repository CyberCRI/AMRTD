//#define VERBOSEDEBUG
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusMaskClickInterceptor : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
#if VERBOSEDEBUG
        Debug.Log("FocusMaskClickInterceptor OnPointerDown");
#endif
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKFOCUSHOLE);
        FocusMaskManager.instance.click();
    }
}
