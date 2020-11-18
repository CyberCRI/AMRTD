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
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " Start");
        #endif
        displayScreen(currentScreen);
        if (autoSequentialDisplay)
        {
            StartCoroutine(animate());
        }
    }

    private void hideAll()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " hideAll");
        #endif
        for (int i = 0; i < screenArray.Length; i++)
        {
            screenArray[i].SetActive(false);
        }
    }

    public void displayScreen(SCREEN index)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " displayScreen(" + (int)index + ")");
        #endif
        hideAll();
        screenArray[(int)index].SetActive(true);
    }

    public void pressNavigationArrow(int stepIncrease)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " pressNavigationArrow(" + stepIncrease + ")");
        #endif
        AudioManager.instance.play(AudioEvent.CLICKUI);
        currentScreen = (SCREEN) ((((int) currentScreen) + stepIncrease) % screenArray.Length);
        displayScreen(currentScreen);

    }

    public void nextScreen()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " nextScreen");
        #endif
        pressNavigationArrow(1);
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKCREDITSNEXT);
    }

    public void previousScreen()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " previousScreen");
        #endif
        pressNavigationArrow(screenArray.Length - 1);
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKCREDITSPREV);
    }

    private IEnumerator animate()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " animate");
        #endif
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

    public void back()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " back");
        #endif
        RedMetricsManager.instance.sendEvent (TrackingEvent.CLICKCREDITSBACK);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        SceneFader.instance.goToMainMenu();
    }
}