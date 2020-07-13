//#define VERBOSEDEBUG

using UnityEngine;
using System.Collections;

public class LevelIntroUI : MonoBehaviour
{
    [SerializeField]
    private Animator middlegroundAnimator = null;
    [SerializeField]
    private Animator foreground2Animator = null;
    [SerializeField]
    private Animator foreground1Animator = null;
    [SerializeField]
    private GameObject startButton = null;
    [SerializeField]
    private float timeBeforeStartUnlocks = 6f;

    void Start()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " Start");
        #endif

        GameManager.instance.setPause(true, GameManager.PAUSER.LEVELINTROUI);
        middlegroundAnimator.SetBool("hasStarted", true);
        foreground2Animator.SetBool("hasStarted", true);
        foreground1Animator.SetBool("hasStarted", true);
        StartCoroutine(waitBeforeUnlockingStart());
    }

    private IEnumerator waitBeforeUnlockingStart()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " waitBeforeUnlockingStart");
        #endif
        yield return new WaitForSecondsRealtime(timeBeforeStartUnlocks);
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " waitBeforeUnlockingStart 2");
        #endif
        startButton.SetActive(true);
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " waitBeforeUnlockingStart 3");
        #endif
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

        GameManager.instance.setPause(false, GameManager.PAUSER.LEVELINTROUI);
        Destroy(gameObject);
    }
}