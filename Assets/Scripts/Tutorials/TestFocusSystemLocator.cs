using UnityEngine;
using UnityEngine.UI;

public class TestFocusSystemLocator : MonoBehaviour
{
    public bool react = false;
    public RectTransform focusSystem;
    public Transform startParent;
    public RectTransform target;

/*
[Header("Manual")]
    public Vector3 absoluteLocation = Vector3.zero;
    public Vector3 relativeLocation = Vector3.zero;
    public Vector2 anchoredLocation = Vector2.zero;

    public Vector3 startRelativeFocusSystemPosition;
    public Vector3 startAnchoredFocusSystemPosition;
*/ 

[Header("Start")]
    public Vector3 startAbsoluteFocusSystemPosition;

[Header("Current")]
    public Vector3 currentAbsoluteFocusSystemPosition;
    public Vector3 currentRelativeFocusSystemPosition;
    public Vector3 currentAnchoredFocusSystemPosition;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        startParent = focusSystem.transform.parent;
        /*
        startRelativeFocusSystemPosition = focusSystem.localPosition;
        startAnchoredFocusSystemPosition = focusSystem.anchoredPosition;
        */
        startAbsoluteFocusSystemPosition = focusSystem.position;
        focusSystem.gameObject.SetActive(true);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        currentAbsoluteFocusSystemPosition = focusSystem.position;
        currentRelativeFocusSystemPosition = focusSystem.localPosition;
        currentAnchoredFocusSystemPosition = focusSystem.anchoredPosition;

        if (react)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                focusSystem.gameObject.SetActive(true);
                focusSystem.position = target.position;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("pressed C");
                center();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                recenter();
            }
            /*
            if (Input.GetKeyDown(KeyCode.Z))
            {
                focusSystem.gameObject.SetActive(true);
                focusSystem.localPosition = relativeLocation;
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                focusSystem.gameObject.SetActive(true);
                focusSystem.anchoredPosition = anchoredLocation;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                resetRelative();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                resetAnchored();
            }
            */

            if (Input.GetKeyDown(KeyCode.Q))
            {
                resetAbsolute();
            }
        }
    }

    void center()
    {
        Debug.Log("center");
        focusSystem.gameObject.SetActive(true);
        focusSystem.transform.SetParent(target.transform);
        focusSystem.anchoredPosition = Vector2.zero;
    }
    void recenter()
    {
        focusSystem.transform.SetParent(startParent);
    }

    void resetAbsolute()
    {
        focusSystem.position = startAbsoluteFocusSystemPosition;     
    }
    /*
    void resetRelative()
    {
        focusSystem.localPosition = startRelativeFocusSystemPosition;
    }
    void resetAnchored()
    {
        focusSystem.anchoredPosition = new Vector2 (
            startAnchoredFocusSystemPosition.x
            , startAnchoredFocusSystemPosition.y
            );
    }
    */
}