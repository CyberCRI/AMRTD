using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FastForwardButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKFASTFORWARD, new CustomData(CustomDataTag.OUTCOME, CustomDataValue.ON));

        if (!FocusMaskManager.instance.isDisplaying())
        {
            GameManager.instance.setHighSpeed();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKFASTFORWARD, new CustomData(CustomDataTag.OUTCOME, CustomDataValue.OFF));

        if (!FocusMaskManager.instance.isDisplaying())
        {
            GameManager.instance.setNormalSpeed();
        }
    }
}
