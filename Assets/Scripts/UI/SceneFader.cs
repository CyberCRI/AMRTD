﻿//#define VERBOSEDEBUG
#define PLAYMUSIC
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class SceneFader : MonoBehaviour
{
    public static SceneFader instance;
    [SerializeField]
    private Image image = null;
    [SerializeField]
    private AnimationCurve fadeCurve = null;
    public const float duration = 1f;

    private IEnumerator coroutine = null;

    public delegate void FadeEndCallback();
    private FadeEndCallback fadeInEndCallback;
    public void setFadeInEndCallback(FadeEndCallback _fadeInEndCallback)
    {
        fadeInEndCallback = _fadeInEndCallback;
    }
    private FadeEndCallback fadeOutEndCallback;
    public void setFadeOutEndCallback(FadeEndCallback _fadeOutEndCallback)
    {
        fadeOutEndCallback = _fadeOutEndCallback;
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
            SceneManager.sceneLoaded += onSceneLoaded;

            #if VERBOSEDEBUG
            Debug.Log(this.GetType() + " Awake");
            string debugScenes = "";
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                debugScenes += SceneUtility.GetScenePathByBuildIndex(i) + " ";
            }
            Debug.Log(this.GetType() + " Awake scenes: " + debugScenes);
            #endif
        }
    }

    void Start()
    {
        coroutine = fadeIn();
        StartCoroutine(coroutine);
    }

    private Regex rgx = new Regex(@"Level\d+");
    private bool isLevelScene(string str)
    {
        //return str.StartsWith(levelScenePrefix);
        return (rgx.Matches(str).Count > 0);
    }

    void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " onSceneLoaded: " + scene.name + " with mode " + mode);
        #endif  

        RedMetricsManager.instance.sendEvent(
            TrackingEvent.LEVELSTARTS,
            CustomData.getContext(
                new CustomDataTag[3]{
                    CustomDataTag.GAMELEVEL,
                    CustomDataTag.TIMESINCEGAMELOADED,
                    CustomDataTag.RESOLUTION
                    }
            )
        );
        #if PLAYMUSIC
        AudioManager.instance.playBackgroundMusic(SceneManager.GetActiveScene().name);
        #endif

        if (isLevelScene(scene.name))
        {
            #if VERBOSEDEBUG
            Debug.Log(this.GetType() + " onSceneLoaded: isLevelScene");
            #endif  
            // show loading screen
            LoadingScreenManagerUI.instance.startFakeLoad();
        }
        else
        {
            #if VERBOSEDEBUG
            Debug.Log(this.GetType() + " onSceneLoaded: isMenuScene");
            #endif  
            coroutine = fadeIn();
            StartCoroutine(coroutine);
        }

    }

    public void fadeTo(int sceneIndex)
    {
        fadeTo(GameConfiguration.instance.getSceneName(sceneIndex));
    }

    public void fadeTo(string scene = "")
    {
        coroutine = fadeOut(scene);
        StartCoroutine(coroutine);
    }

    // for external calls
    public void startFadeIn()
    {
        if (null != coroutine)
        {
            StopCoroutine(coroutine);
        }
        coroutine = fadeIn();
        StartCoroutine(coroutine);
    }

    private IEnumerator fadeIn()
    {
        float t = duration;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime;
            float a = fadeCurve.Evaluate(t);
            image.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }

        if (null != fadeInEndCallback)
        {
            #if VERBOSEDEBUG
            Debug.Log(this.GetType() + " fadeIn (null != fadeInEndCallback)");
            #endif  

            fadeInEndCallback();
            fadeInEndCallback = null;
        }
    }

    private IEnumerator fadeOut(string scene)
    {
        float t = duration;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime;
            float a = fadeCurve.Evaluate(duration - t);
            image.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }

        // the call to play stops the music
        //AudioManager.instance.stopBackgroundMusic();

        if (string.IsNullOrEmpty(scene))
        {
            if (null != fadeOutEndCallback)
            {
                #if VERBOSEDEBUG
                Debug.Log(this.GetType() + " fadeOut (null != fadeOutEndCallback)");
                #endif  

                fadeOutEndCallback();
                fadeOutEndCallback = null;
            }
            startFadeIn();
        }
        else
        {
            SceneManager.LoadScene(scene);   
        }
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
