using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WavesSurvived : MonoBehaviour
{
    [SerializeField]
    private Text wavesText = null;
    [SerializeField]
    private float counterStepTime = 0.05f;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        StartCoroutine(animateText());
    }

    private IEnumerator animateText()
    {
        wavesText.text = "0";
        int waveCount = 0;

        yield return new WaitForSeconds(SceneFader.duration);

        while (waveCount < PlayerStatistics.instance.waves)
        {
            waveCount++;
            wavesText.text = waveCount.ToString();
            
            yield return new WaitForSeconds(counterStepTime);
        }
    } 
}
