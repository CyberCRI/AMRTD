//#define VERBOSEDEBUG
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
    
    // internal temporary variables
    private string goName;
    private GameObject go;
    private TUTORIALACTION action;
    private string textHint;
    private ExternalOnPressButton target;
    private bool showFocusMaskAndArrow;
    private TrackingEvent trackingEvent;

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
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " next");
        printDebug("next1");
#endif

        prepared = false;
        waited = 0f;
        stepIndex++;

#if VERBOSEDEBUG
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
#if VERBOSEDEBUG
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
#if VERBOSEDEBUG
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
                        goName = steps[stepIndex].gameObjectName;
                        go = GameObject.Find(goName);
                        foundObject = go;
                        if (go == null)
                        {
                            Debug.LogError(this.GetType() + " GameObject not found at step " + stepIndex + ": " + goName);
                            next();
                        }
                        else
                        {
                            action = steps[stepIndex].action;
                            textHint = steps[stepIndex].textHint;

                            // Debug.Log(this.GetType() + " go != null at step=" + _step + ", go.transform.position=" + go.transform.position + " & go.transform.localPosition=" + go.transform.localPosition);
                            target = go.GetComponent<ExternalOnPressButton>();
                            if (null != target)
                            {
                                // Debug.Log(this.GetType() + " target != null at step=" + _step
                                //+ " with text=" + steps[_step].textHint
                                //);
                                focusMaskManager.focusOn(target, next, textHint, true, true, action);
                            }
                            else
                            {
                                // Debug.Log(this.GetType() + " target == null at step=" + _step
                                //+ " with text=" + steps[_step].textHint
                                //);
                                focusMaskManager.focusOn(go, next, textHint, true, true, action);
                            }
                            
                            showFocusMaskAndArrow = (steps[stepIndex].action == action);
                            trackingEvent = showFocusMaskAndArrow ? TrackingEvent.TUTORIALFOCUSON : TrackingEvent.TUTORIALIMAGE;
                            RedMetricsManager.instance.sendEvent(trackingEvent, new CustomData(CustomDataTag.MESSAGE, textHint).add(CustomDataTag.ELEMENT, go));

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
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " virtual end");
#endif
        focusMaskManager.stopFocusOn();
        StepByStepTutorial._isPlaying = false;
        Destroy(this);
    }
}
