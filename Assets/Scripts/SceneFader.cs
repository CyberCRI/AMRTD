using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private float fadeSpeed = 1f;

    void Start ()
    {
        StartCoroutine(fadeIn());
    }

    private IEnumerator fadeIn()
    {
        float t = 1f;

        // animate over time
        while (t > 0f)
        {
            t -= Time.deltaTime * fadeSpeed;
            image.color = new Color(0f, 0f, 0f, t);
            yield return 0;
        }
    }
}
