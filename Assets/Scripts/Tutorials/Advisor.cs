using UnityEngine;
using UnityEngine.UI;

public class Advisor : MonoBehaviour
{
    [SerializeField]
    private RectTransform _advisorAnchor;
    [SerializeField]
    private GameObject _nextButton;
    [SerializeField]
    private LocalizedText _localize;
    [SerializeField]
    private float _topAnchorValue;
    [SerializeField]
    private float _bottomAnchorValue;
    [SerializeField]
    private float _highlightedAreaRatio;
    [SerializeField]
    private float _advisorAreaRatio;
    [SerializeField]
    private float minX = -0.31f, minY = -0.23f, maxX = 0.37f, maxY = 0.2f;

    public Vector3 currentPosition;
    public Vector3 currentLocalPosition;
    public Vector3 currentAnchoredPosition;

    
    [SerializeField]
    private RectTransform rectTransform;
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
        currentAnchoredPosition = this.GetComponent<RectTransform>().anchoredPosition;
    }

    public void setUpAdvisor(Vector2 position, string text, float scaleFactor, bool showButton = false)
    {
        /*
        float offset = scaleFactor * _highlightedAreaRatio + _advisorAreaRatio;
        _advisorAnchor.relativeOffset.x = position.x - Mathf.Sign(position.x) * offset;
        _advisorAnchor.relativeOffset.x = _advisorAnchor.relativeOffset.x > 0 ? Mathf.Min(_advisorAnchor.relativeOffset.x, maxX) : Mathf.Max(_advisorAnchor.relativeOffset.x, minX);
        _advisorAnchor.relativeOffset.y = position.y - Mathf.Sign(position.y) * offset;
        _advisorAnchor.relativeOffset.y = _advisorAnchor.relativeOffset.y > 0 ? Mathf.Min(_advisorAnchor.relativeOffset.y, maxY) : Mathf.Max(_advisorAnchor.relativeOffset.y, minY);
*/
        _localize.setKey(text);
        _nextButton.SetActive(showButton);
    }

    public void setSpeechBubble(FocusMaskManager.Quadrant quadrant, string text, bool showButton = false)
    {
        Debug.Log("setSpeechBubble(" + quadrant + ", " + text + ", " + showButton + ")");
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
}
