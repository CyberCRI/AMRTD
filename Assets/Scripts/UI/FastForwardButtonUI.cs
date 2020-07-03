#define VERBOSEDEBUG
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FastForwardButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " OnPointerDown");
        #endif
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKFASTFORWARD, new CustomData(CustomDataTag.OUTCOME, CustomDataValue.ON));
        AudioManager.instance.doFastForward(true);

        if (!FocusMaskManager.instance.isDisplaying())
        {
            GameManager.instance.setHighSpeed();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " OnPointerUp");
        #endif
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKFASTFORWARD, new CustomData(CustomDataTag.OUTCOME, CustomDataValue.OFF));
        AudioManager.instance.doFastForward(false);

        if (!FocusMaskManager.instance.isDisplaying())
        {
            GameManager.instance.setNormalSpeed();
        }
    }
}
