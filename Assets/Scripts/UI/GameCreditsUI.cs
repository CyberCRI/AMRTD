//#define VERBOSEDEBUG
//#define QUICKTEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCreditsUI : MonoBehaviour
{
    [SerializeField]
    private bool autoSequentialDisplay = false;
    [SerializeField]
    private float displayDuration;
    [SerializeField]
    private GameObject[] screenArray = null;
    private SCREEN currentScreen = SCREEN.MEMBERS;

    public const string sceneName = "GameCredits";

    public enum SCREEN {
        MEMBERS = 0,
        RESOURCES1 = 1,
        RESOURCES2 = 2
    }

    void Start()
    {
        hideAll();
        displayScreen(currentScreen);
        if (autoSequentialDisplay)
        {
            StartCoroutine(animate());
        }
    }

    private void hideAll()
    {
        for (int i = 0; i < screenArray.Length; i++)
        {
            screenArray[i].SetActive(false);
        }
    }

    public void displayScreen(SCREEN index)
    {
        hideAll();
        Debug.Log((int)index);
        screenArray[(int)index].SetActive(true);
    }

    public void nextScreen()
    {
        currentScreen = (SCREEN) ((((int) currentScreen) + 1) % 3);
        displayScreen(currentScreen);
    }

    public void previousScreen()
    {
        currentScreen = (SCREEN) ((((int) currentScreen) + 2) % 3);
        displayScreen(currentScreen);
    }

    private IEnumerator animate()
    {
        #if QUICKTEST
        yield return new WaitForSeconds(0f);
        #else
        yield return new WaitForSeconds(displayDuration);
        for (int i = 0; i < screenArray.Length; i++)
        {
            screenArray[i].SetActive(true);
            yield return new WaitForSeconds(displayDuration);
            screenArray[i].SetActive(false);
        }
        #endif

        SceneFader.instance.fadeTo(LevelSelectionUI.sceneName);
    }
}