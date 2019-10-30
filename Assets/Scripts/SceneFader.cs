using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField]
    private Image image = null;
    [SerializeField]
    private AnimationCurve fadeCurve = null;
    public const float duration = 1f;

    void Start()
    {
        StartCoroutine(fadeIn());
    }

    public void fadeTo(string scene)
    {
        StartCoroutine(fadeOut(scene));
    }

    private IEnumerator fadeIn()
    {
        float t = duration;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            float a = fadeCurve.Evaluate(t);
            image.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
    }

    private IEnumerator fadeOut(string scene)
    {
        float t = duration;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            float a = fadeCurve.Evaluate(duration - t);
            image.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }

        SceneManager.LoadScene(scene);
    }
}
