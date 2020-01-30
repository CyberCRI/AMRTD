using UnityEngine;
using UnityEngine.EventSystems;

public class FocusMaskClickInterceptor : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        FocusMaskManager.instance.click();
    }
}
