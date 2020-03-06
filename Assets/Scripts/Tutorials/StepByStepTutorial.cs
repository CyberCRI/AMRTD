//#define DEVMODE
using UnityEngine;

public abstract class StepByStepTutorial : MonoBehaviour
{
    private GameObject foundObject = null;

    public enum TUTORIALACTION
    {
        NONE,
        FOCUSON,
        SETGREYBACKGROUND
    }

    public struct TutorialStep
    {
        public string gameObjectName;
        public TUTORIALACTION action;
        public string textHint;

        public TutorialStep(
            string _gameObjectName
            , TUTORIALACTION _action = TUTORIALACTION.FOCUSON
            , string _textHint = "_"
            )
        {
            gameObjectName = _gameObjectName;
            action = _action;
            textHint = _textHint;
        }

        public string getDebugString()
        {
            return string.Format("[goName: {0}; action:{1}; textHint:{2}]", gameObjectName, action, textHint);
        }
    }

    private int stepIndex = 0;
    private bool prepared = false;
    private float waited = 0;
    private const float waitedThreshold = 0f;

    protected const string genericTextKeyPrefix = "TUTORIAL.";
    protected const string cloneSuffix = "(Clone)";

    protected abstract string textKeyPrefix { get; }
    protected abstract int stepCount { get; }
    protected abstract TutorialStep[] steps { get; }

    private Vector3 manualScale = new Vector3(440, 77, 1);
    private static FocusMaskManager focusMaskManager;

    private static bool _isPlaying = false;
    public static bool isPlaying()
    {
        return _isPlaying;
    }
    public static void reset()
    {
        _isPlaying = false;
    }

    public void next()
    {
#if DEVMODE
        Debug.Log(this.GetType() + " next");
        printDebug("next1");
#endif

        prepared = false;
        waited = 0f;
        stepIndex++;

#if DEVMODE
        printDebug("next2");
#endif
    }

    public string getStepsDebugString()
    {
        string result = "steps[";
        for (int i = 0; i < steps.Length; i++)
        {
            result += steps[i].getDebugString();
            if (i != steps.Length - 1)
            {
                result += "; ";
            }
        }
        result += "]";
        return result;
    }

    protected void Awake()
    {
#if DEVMODE
        //Debug.Log(this.GetType() + "Awake");
        printDebug("Awake");
#endif
        StepByStepTutorial._isPlaying = true;
        for (int index = 0; index < steps.Length; index++)
        {
            steps[index].textHint = textKeyPrefix + index;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + "Start");
        focusMaskManager = FocusMaskManager.instance;
    }

    protected virtual void printDebug(string callOrigin = "")
    {
        Debug.Log(this.GetType() + " " + callOrigin + " printDebug "
       + " step=" + stepIndex
       + " prepared=" + prepared
       + " waited=" + waited
       + " textKeyPrefix=" + textKeyPrefix
       + " stepCount=" + stepCount
       + " steps=" + getStepsDebugString()
       );
    }

    protected virtual bool skipStep(int step)
    {
        return false;
    }

    protected virtual void prepareStep(int step) { }

    // Update is called once per frame
    void Update()
    {
#if DEVMODE
        if (Input.GetKeyDown(KeyCode.D))
        {
            printDebug("Update");
        }
#endif

        if (stepIndex < steps.Length)
        {
            if (waited >= waitedThreshold)
            {
                if (!prepared)
                {
                    prepareStep(stepIndex);

                    if (skipStep(stepIndex))
                    {
                        next();
                    }
                    else
                    {
                        // Debug.Log(this.GetType() + " preparing step " + _step + " searching for " + steps[_step].gameObjectName);
                        GameObject go = GameObject.Find(steps[stepIndex].gameObjectName);
                        foundObject = go;
                        if (go == null)
                        {
                            Debug.LogError(this.GetType() + " GameObject not found at step " + stepIndex + ": " + steps[stepIndex].gameObjectName);
                            next();
                        }
                        else
                        {
                            // Debug.Log(this.GetType() + " go != null at step=" + _step + ", go.transform.position=" + go.transform.position + " & go.transform.localPosition=" + go.transform.localPosition);
                            ExternalOnPressButton target = go.GetComponent<ExternalOnPressButton>();
                            if (null != target)
                            {
                                // Debug.Log(this.GetType() + " target != null at step=" + _step
                                //+ " with text=" + steps[_step].textHint
                                //);
                                focusMaskManager.focusOn(target, next, steps[stepIndex].textHint, true, true, steps[stepIndex].action);
                            }
                            else
                            {
                                // Debug.Log(this.GetType() + " target == null at step=" + _step
                                //+ " with text=" + steps[_step].textHint
                                //);
                                focusMaskManager.focusOn(go, next, steps[stepIndex].textHint, true, true, steps[stepIndex].action);
                            }
                            // Debug.Log(this.GetType() + " prepared step=" + _step);
                            prepared = true;
                        }
                    }
                }
#if DEVMODE
                else
                {
                    if (Input.GetKeyUp(KeyCode.RightArrow))
                    {
                        next();
                    }
                }
#endif
            }
            waited += Time.unscaledDeltaTime;
        }
        else
        {
            end();
        }
    }

    protected virtual void end()
    {
#if DEVMODE
        Debug.Log(this.GetType() + " virtual end");
#endif
        focusMaskManager.stopFocusOn();
        StepByStepTutorial._isPlaying = false;
        Destroy(this);
    }
}
