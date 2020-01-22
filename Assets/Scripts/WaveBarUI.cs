//#define DEVMODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Enemy wave bar
// Fills up wave after wave
public class WaveBarUI : MonoBehaviour
{
    [SerializeField]
    private Image waveBar = null;
    [SerializeField]
    private RectTransform indicator = null;

    private float previousValue = 0f;
    private float newValue = 0f;
    private float animationDuration = 1f;
    private bool isLerpInProgress = false;

    // for auto-animation of the bar, to test independently from waves
#if DEVMODE
    [SerializeField]
    private float maxValue = 120f;
    [SerializeField]
    private float currentValue = 0f;
#endif

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        setFillAmount(0f);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

#if DEVMODE
        currentValue += Time.deltaTime;
        setFillAmount(currentValue/maxValue);
#else
        if (!isLerpInProgress)
        {
            previousValue = waveBar.fillAmount;
            newValue = WaveSpawner.instance.getWaveProgression();

            // is a Lerp needed?
            if (previousValue != newValue)
            {
                StartCoroutine(smoothAnimate(previousValue, newValue));
            }
        }
#endif
    }


    private IEnumerator smoothAnimate(float startValue, float endValue)
    {
        isLerpInProgress = true;
        float timeParameter = 0f;
        while (timeParameter <= animationDuration)
        {
            timeParameter += (Time.deltaTime / animationDuration);
            setFillAmount(Mathf.Lerp(startValue, endValue, timeParameter));
            yield return null;
        }
        isLerpInProgress = false;
    }

    private void setFillAmount(float fillValue)
    {
        waveBar.fillAmount = fillValue;
        indicator.anchorMin = new Vector2(fillValue, indicator.anchorMin.y);
        indicator.anchorMax = new Vector2(fillValue, indicator.anchorMax.y);
    }
}
