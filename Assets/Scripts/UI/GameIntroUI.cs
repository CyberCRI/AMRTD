//#define VERBOSEDEBUG
//#define QUICKTEST
#define ALWAYSSHOWINTRO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameIntroUI : MonoBehaviour
{
    [SerializeField]
    private float displayDuration;
    [SerializeField]
    private GameObject[] screenArray;

    public const string sceneName = "GameIntro";

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
        yield return new WaitForSeconds(0f);
        #else

        #if !ALWAYSSHOWINTRO
        if (GameConfiguration.instance.showIntro)
        {
        #endif
            yield return new WaitForSeconds(displayDuration);
            for (int i = 0; i < screenArray.Length; i++)
            {
                screenArray[i].SetActive(true);
                yield return new WaitForSeconds(displayDuration);
                screenArray[i].SetActive(false);
            }
            GameConfiguration.instance.showIntro = false;
        #if !ALWAYSSHOWINTRO
        }
        else
        {
            yield return new WaitForSeconds(0f);
        }
        #endif
        #endif

        SceneFader.instance.fadeTo(MainMenu.sceneName);
    }
}