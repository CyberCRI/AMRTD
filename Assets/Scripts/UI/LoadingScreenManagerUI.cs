//#define VERBOSEDEBUG
//#define QUICKTEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManagerUI : IndicatedProgressBarUI
{
    public static LoadingScreenManagerUI instance = null;

    [SerializeField]
    private GameObject root = null;
    [SerializeField]
    private Sprite[] pictures = null;
    [SerializeField]
    private Image imageHolder = null;
    private float maxLoadTime = 5f;
    private float currentLoadTime = 0f;
    private float updateBarTime = .7f;
    private float variation = 0.2f;
    private bool pauseStateBefore = false;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void startFakeLoad()
    {
#if QUICKTEST
        SceneFader.instance.startFadeIn();
    }

#else
        #if VERBOSEDEBUG
        Debug.Log("LoadingScreenManagerUI startFakeLoad starts");
        #endif
        imageHolder.sprite = pictures[Random.Range(0, pictures.Length)];
        root.SetActive(true);

        // pause the game
        pauseStateBefore = GameManager.instance.isPaused();
        GameManager.instance.setPause(true, "LoadingScreenManagerUI");
        #if VERBOSEDEBUG
        Debug.Log("LoadingScreenManagerUI startFakeLoad setPause");
        #endif

        // start fake load coroutine
        StartCoroutine(fakeLoad());
        #if VERBOSEDEBUG
        Debug.Log("LoadingScreenManagerUI startFakeLoad StartCoroutine");
        #endif
    }

    // allows for a smooth transition from resistance(turrets_count(t+1))
    // to resistance(turrets_count(t+1))
    // through the use of the variable negativePointsPool
    private IEnumerator fakeLoad()
    {
        #if VERBOSEDEBUG
        Debug.Log("LoadingScreenManagerUI fakeLoad starts");
        #endif
        
        //*
        currentLoadTime = 0f;
        while ((currentLoadTime < maxLoadTime)) // && (true))
        {
            currentLoadTime += Time.unscaledDeltaTime;
            yield return null;
            /*
            timeParameter = 0f;
            step = Mathf.Min(negativePointsStep, negativePointsPool);
            negativePointsPool -= step;
            startValue = _turretResistancePoints;
            endValue = _turretResistancePoints - step;
            while (timeParameter <= 1)
            {
                timeParameter += Time.deltaTime / animationDuration;
                lerp = Mathf.Lerp(startValue, endValue, timeParameter);
                updateResistancePoints(lerp);
                yield return null;
            }
            */
        }
        //*/
        //yield return new WaitForSecondsRealtime(maxLoadTime);
        GameManager.instance.setPause(pauseStateBefore, "LoadingScreenManagerUI");
        SceneFader.instance.startFadeIn();
        root.SetActive(false);
        currentLoadTime = 0f;
        
        #if VERBOSEDEBUG
        Debug.Log("LoadingScreenManagerUI fakeLoad done");
        #endif
    }
#endif

    public override float getLatestValue()
    {
        return currentLoadTime * 1.33f / maxLoadTime;
    }
}
