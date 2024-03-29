//#define VERBOSEDEBUG
//#define QUICKTEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOutroUI : MonoBehaviour
{
    [SerializeField]
    private float loadDuration;
    [SerializeField]
    private float displayDuration;
    [SerializeField]
    private GameObject[] screenArray;

    public const string sceneName = "GameOutro";

    void Start()
    {
        for (int i = 0; i < screenArray.Length; i++)
        {
            screenArray[i].SetActive(false);
        }
        StartCoroutine(animate());
    }

    private IEnumerator animate()
    {
        #if QUICKTEST
        yield return new WaitForSecondsRealtime(0f);
        #else
        yield return new WaitForSecondsRealtime(loadDuration);
        for (int i = 0; i < screenArray.Length; i++)
        {
            screenArray[i].SetActive(true);
            yield return new WaitForSecondsRealtime(displayDuration);
            screenArray[i].SetActive(false);
        }
        #endif

        SceneFader.instance.fadeTo(LevelSelectionUI.sceneName);
    }
}