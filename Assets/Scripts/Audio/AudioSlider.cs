//#define VERBOSEDEBUG
//#define DEVMODE

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioSlider : MonoBehaviour, IPointerUpHandler
//, IEndDragHandler // does not capture single click changes
//,  IBeginDragHandler // unnecessary
{
    [SerializeField]
    private string sliderHandleID = "";

    public void OnPointerUp (PointerEventData data)
    {
        #if VERBOSEDEBUG
        Debug.Log(" AudioSlider " + sliderHandleID + " OnPointerUp");
        #endif
        AudioManager.instance.onPointerUp(sliderHandleID);
    }
}