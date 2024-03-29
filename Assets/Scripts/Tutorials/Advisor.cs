//#define VERBOSEDEBUG
using UnityEngine;
using UnityEngine.UI;

public class Advisor : MonoBehaviour
{
    [SerializeField]
    private RectTransform _advisorAnchor;
    [SerializeField]
    private GameObject _nextButton = null;
    [SerializeField]
    private LocalizedText _localize = null;
    [SerializeField]
    private float _topAnchorValue;
    [SerializeField]
    private float _bottomAnchorValue;
    [SerializeField]
    private float _highlightedAreaRatio;
    [SerializeField]
    private float _advisorAreaRatio;

    public Vector3 currentPosition;
    public Vector3 currentLocalPosition;
    public Vector3 currentAnchoredPosition;
    public Vector3 currentAnchoredPosition2;

    
    [SerializeField]
    private RectTransform rectTransform = null;
    [SerializeField]
    private float left = 0f;
    [SerializeField]
    private float right = 0f;
    [SerializeField]
    private float top = 0f;
    [SerializeField]
    private float bottom = 0f;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        currentPosition = this.transform.position;
        currentLocalPosition = this.transform.localPosition;
        currentAnchoredPosition = _advisorAnchor.anchoredPosition;
        currentAnchoredPosition2 = this.GetComponent<RectTransform>().anchoredPosition;
    }

    public void setSpeechBubble(FocusMaskManager.Quadrant quadrant, string text, bool showButton = false)
    {
#if VERBOSEDEBUG
        Debug.Log("setSpeechBubble(" + quadrant + ", " + text + ", " + showButton + ")");
#endif
        float horizontal = 0f, vertical = 0f;
        switch (quadrant)
        {
            case FocusMaskManager.Quadrant.BOTTOM_RIGHT:
                horizontal = left;
                vertical = top;
                break;
            case FocusMaskManager.Quadrant.TOP_RIGHT:
                horizontal = left;
                vertical = bottom;
                break;
            case FocusMaskManager.Quadrant.TOP_LEFT:
                horizontal = right;
                vertical = bottom;
                break;
            case FocusMaskManager.Quadrant.BOTTOM_LEFT:
                horizontal = right;
                vertical = top;
                break;
            default:
                Debug.LogWarning("unexpected quadrant " + quadrant);
                break;
        }
        rectTransform.anchoredPosition = new Vector2(horizontal, vertical);
        _localize.setKey(text);
        _nextButton.SetActive(showButton);
    }

    public void setActive()
    {
        this.gameObject.SetActive(_localize.hasKey());
    }

    public void reset()
    {
        _localize.setKey("");
    }
}
