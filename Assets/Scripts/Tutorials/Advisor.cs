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

    public void setUpAdvisor(Vector2 position, string text, float scaleFactor, bool showButton = false)
    {
        float offset = scaleFactor * _highlightedAreaRatio + _advisorAreaRatio;
        /*
        _advisorAnchor.relativeOffset.x = position.x - Mathf.Sign(position.x) * offset;
        _advisorAnchor.relativeOffset.x = _advisorAnchor.relativeOffset.x > 0 ? Mathf.Min(_advisorAnchor.relativeOffset.x, maxX) : Mathf.Max(_advisorAnchor.relativeOffset.x, minX);
        _advisorAnchor.relativeOffset.y = position.y - Mathf.Sign(position.y) * offset;
        _advisorAnchor.relativeOffset.y = _advisorAnchor.relativeOffset.y > 0 ? Mathf.Min(_advisorAnchor.relativeOffset.y, maxY) : Mathf.Max(_advisorAnchor.relativeOffset.y, minY);
        */
        _localize.setKey(text);
        _nextButton.SetActive(showButton);
    }
}
