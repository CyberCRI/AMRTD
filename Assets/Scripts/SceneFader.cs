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
        float t = 1f;
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
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            float a = fadeCurve.Evaluate(1 - t);
            image.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }

        SceneManager.LoadScene(scene);
    }
}
