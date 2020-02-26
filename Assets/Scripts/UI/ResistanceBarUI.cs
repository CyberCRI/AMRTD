using UnityEngine;
using UnityEngine.UI;

// Player resistance bar
// Fills up when too many towers are used
public class ResistanceBarUI : IndicatedProgressBarUI
{
    private float highlightImageBorders = 38f;
    [SerializeField]
    private Animator alarmer = null;
    private Vector2 startHighlighterSizeDelta = Vector2.zero;
    private Vector2 startAnchoredPosition = Vector2.zero;

    [SerializeField]
    private RectTransform highlighter = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        startHighlighterSizeDelta = highlighter.sizeDelta - new Vector2(highlightImageBorders, 0f);
        startAnchoredPosition = highlighter.anchoredPosition;
    }

    public override float getLatestValue()
    {
        return PlayerStatistics.instance.resistancePointsRatio;
    }

    protected override void onSetFillAmount(float fillAmount)
    {
        alarmer.SetFloat("resistanceLevelRatio", fillAmount);
        float barLength = startHighlighterSizeDelta.x * fillAmount;
        highlighter.sizeDelta = new Vector2(
            barLength + highlightImageBorders,
            startHighlighterSizeDelta.y);
        highlighter.anchoredPosition = new Vector2(
            startAnchoredPosition.x - (startHighlighterSizeDelta.x - barLength)/2,
            startAnchoredPosition.y);
    }
}
