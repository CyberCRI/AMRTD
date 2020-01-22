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

    private float startValue = 0f;
    private float endValue = 0f;

    private float timeParameter = 0f;
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
        waveBar.fillAmount = 0f;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

#if DEVMODE
         currentValue += Time.deltaTime;
         waveBar.fillAmount = currentValue/maxValue;
#else

        // is a Lerp in progress?
        isLerpInProgress = (startValue != endValue);

        if (!isLerpInProgress)
        {
            previousValue = waveBar.fillAmount;
            newValue = WaveSpawner.instance.getWaveProgression();

            // is a Lerp needed?
            if (previousValue != newValue)
            {
                // set up the Lerp
                startValue = previousValue;
                endValue = newValue;
                timeParameter = 0f;
            }
        }
        else
        {
            timeParameter += (Time.deltaTime / animationDuration);
            waveBar.fillAmount = Mathf.Lerp(startValue, endValue, timeParameter);

            if (timeParameter >= animationDuration)
            {
                startValue = 0f;
                endValue = 0f;
                timeParameter = 0f;
            }

#endif
            indicator.anchorMin = new Vector2(waveBar.fillAmount, indicator.anchorMin.y);
            indicator.anchorMax = new Vector2(waveBar.fillAmount, indicator.anchorMax.y);
        }

    }
}
