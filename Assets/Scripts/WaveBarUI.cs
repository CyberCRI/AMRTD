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
    
#if DEVMODE
    [SerializeField]
    private float maxValue = 120f;
    [SerializeField]
    private float currentValue = 0f;
#endif

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
#if DEVMODE
        currentValue += Time.deltaTime;
        waveBar.fillAmount = currentValue/maxValue;
#else
        waveBar.fillAmount = WaveSpawner.instance.getWaveProgression();
#endif
        indicator.anchorMin = new Vector2(waveBar.fillAmount, indicator.anchorMin.y);
        indicator.anchorMax = new Vector2(waveBar.fillAmount, indicator.anchorMax.y);
    }
}
