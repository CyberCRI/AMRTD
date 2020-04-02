using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader instance;
    [SerializeField]
    private Image image = null;
    [SerializeField]
    private AnimationCurve fadeCurve = null;
    public const float duration = 1f;

    public delegate void FadeInEndCallback();
    private FadeInEndCallback fadeInEndCallback;
    public void setFadeInEndCallback(FadeInEndCallback _fadeInEndCallback)
    {
        fadeInEndCallback = _fadeInEndCallback;
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

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

        if (null != fadeInEndCallback)
        {
            fadeInEndCallback();
            fadeInEndCallback = null;
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

    public void goToMainMenu()
    {
        fadeTo(MainMenu.sceneName);
    }

    public void retry()
    {
        fadeTo(SceneManager.GetActiveScene().name);
    }
}
