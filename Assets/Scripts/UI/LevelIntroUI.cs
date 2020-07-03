#define VERBOSEDEBUG

using UnityEngine;

public class LevelIntroUI : MonoBehaviour
{
    void Awake()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " Awake");
        #endif

        GameManager.instance.setPause(true, GameManager.levelIntroUIPauserKey);
    }

    public void onClickStart()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " onClickStart");
        #endif

        SceneFader.instance.setFadeOutEndCallback(onFadeOutEnded);
        SceneFader.instance.fadeTo();
    }

    private void onFadeOutEnded()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " onFadeOutEnded");
        #endif

        GameManager.instance.setPause(false, GameManager.levelIntroUIPauserKey);
        Destroy(gameObject);
    }
}