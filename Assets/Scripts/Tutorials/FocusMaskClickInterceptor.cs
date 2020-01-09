using UnityEngine;

public class FocusMaskClickInterceptor : MonoBehaviour
{
    void OnMouseDown()
    {
        FocusMaskManager.instance.click();
    }
}
