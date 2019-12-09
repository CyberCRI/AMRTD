using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManualCarouselUI : MonoBehaviour
{
    [SerializeField]
    private Transform[] elements = null;
    [SerializeField]
    private Transform elementsRoot = null;

    public float ratioPerElement = 0f;

    public bool isManual = false;
    public float manualScroll = 0f;

    public bool autoScroll = false;

    public ScrollRect scrollRect;

    public float previousScroll = 0f;

    void Awake()
    {
        Debug.Log("Awake " + scrollRect.verticalNormalizedPosition.ToString("#.00"));
        scrollRect.onValueChanged.AddListener((Vector2 val) => onScrolled(val));

        elements = new Transform[elementsRoot.childCount];
        CommonUtilities.fillArrayFromRoot(elementsRoot, ref elements);

        ratioPerElement = 1f / elements.Length;

        previousScroll = scrollRect.verticalNormalizedPosition;
    }


    void OnBeginDrag(PointerEventData data)
    {
        Debug.Log("OnBeginDrag");
    }
    void OnDrag(PointerEventData data)
    {
        Debug.Log("OnDrag");
    }
    void OnEndDrag(PointerEventData data)
    {
        Debug.Log("OnEndDrag");
    }
    void OnScroll(PointerEventData data)
    {
        Debug.Log("OnScroll");
    }

    void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }
    void OnMouseDrag()
    {
        Debug.Log("OnMouseDrag");
    }
    void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
    }
    void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
    }
    void OnMouseOver()
    {
        Debug.Log("OnMouseOver");
    }
    void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
    }
    void OnMouseUpAsButton()
    {
        Debug.Log("OnMouseUpAsButton");
    }

    void onScrolled(Vector2 value)
    {
        if (autoScroll)
        {
            /*
            float n = Mathf.Round(scrollRect.verticalNormalizedPosition / ratioPerElement);
            n = Mathf.Clamp(n, 0, elements.Length-1);
            float newScroll = n * ratioPerElement;
            Debug.Log(
                "v1=" + scrollRect.verticalNormalizedPosition.ToString("#.00")
                + " ; n=" + n.ToString()
                + " ; v2=" + newScroll.ToString("#.00")
                + "\nevent: " + value.ToString()
                );

            scrollRect.verticalNormalizedPosition = Mathf.Lerp(
                scrollRect.verticalNormalizedPosition
                , newScroll
                , 1f
                );
                */

                float previousn = 1 + Mathf.Round(scrollRect.verticalNormalizedPosition * (elements.Length - 1));
                previousn = Mathf.Clamp(previousn, 1, elements.Length);
                float newn = previousn;

                if (scrollRect.verticalNormalizedPosition > previousScroll)
                // user tries to go up
                {
                    newn++;
                }
                else
                // user tries to go down
                {
                    newn--;
                }
                newn = Mathf.Clamp(newn, 1, elements.Length);
                float newScroll = (newn - 1) / (elements.Length - 1);

                scrollRect.verticalNormalizedPosition = Mathf.Lerp(
                scrollRect.verticalNormalizedPosition
                , newScroll
                , 1f
                );

                previousScroll = scrollRect.verticalNormalizedPosition;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log(scrollRect.verticalNormalizedPosition.ToString("#.00"));
        }

        if (isManual)
        {
            scrollRect.verticalNormalizedPosition = manualScroll;
            isManual = false;
        }
    }
}
