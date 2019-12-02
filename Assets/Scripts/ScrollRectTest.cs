using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectTest : MonoBehaviour
{
    public ScrollRect scrollRect;

    void Awake()
    {
        scrollRect.onValueChanged.AddListener((Vector2 val) => onScrolled(val));
    }


    public void OnBeginDrag(PointerEventData data)
    {
        Debug.Log("OnBeginDrag");
    }
    public void OnDrag(PointerEventData data)
    {
        Debug.Log("OnDrag");
    }
    public void OnEndDrag(PointerEventData data)
    {
        Debug.Log("OnEndDrag");
    }
    public void OnScroll(PointerEventData data)
    {
        Debug.Log("OnScroll");
    }

    public void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }
    public void OnMouseDrag()
    {
        Debug.Log("OnMouseDrag");
    }
    public void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
    }
    public void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
    }
    public void OnMouseOver()
    {
        Debug.Log("OnMouseOver");
    }
    public void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
    }
    public void OnMouseUpAsButton()
    {
        Debug.Log("OnMouseUpAsButton");
    }

    void onScrolled(Vector2 value)
    {
        Debug.Log("onScrolled");
    }
}
