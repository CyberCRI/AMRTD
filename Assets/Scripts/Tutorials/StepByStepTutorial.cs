#define DEVMODE
using UnityEngine;

public abstract class StepByStepTutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject foundObject = null;

    private int _step = 0;
    private bool prepared = false;
    private float waited = 0;
    private const float waitedThreshold = 0f;

    protected const string _genericTextKeyPrefix = "TUTORIAL.";
    private string[] textHints;

    protected abstract string textKeyPrefix { get; }
    protected abstract int stepCount { get; }
    protected abstract string[] focusObjects { get; }

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
        // Debug.Log(this.GetType() + " next");
#endif
        prepared = false;
        waited = 0f;
        _step++;
    }

    void Awake()
    {
#if DEVMODE
        //Debug.Log(this.GetType() + "Awake");

        Debug.Log(this.GetType() + " Awake "
       + " step=" + _step
       + " prepared=" + prepared
       + " waited=" + waited
       + " textHints=" + textHints
       + " textKeyPrefix=" + textKeyPrefix
       + " stepCount=" + stepCount
       + " focusObjects=" + focusObjects
       );
#endif
        StepByStepTutorial._isPlaying = true;
        textHints = new string[stepCount];
        for (int index = 0; index < textHints.Length; index++)
        {
            textHints[index] = textKeyPrefix + index;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + "Start");
        focusMaskManager = FocusMaskManager.instance;
    }

    protected virtual bool skipStep(int step)
    {
        return false;
    }

    protected virtual void prepareStep(int step) { }

    // Update is called once per frame
    void Update()
    {
        if (_step < focusObjects.Length)
        {
            if (waited >= waitedThreshold)
            {
                if (!prepared)
                {
                    prepareStep(_step);

                    if (skipStep(_step))
                    {
                        next();
                    }
                    else
                    {
                        // Debug.Log(this.GetType() + " preparing step " + _step + " searching for " + focusObjects[_step]);
                        GameObject go = GameObject.Find(focusObjects[_step]);
                        foundObject = go;
                        if (go == null)
                        {
                            Debug.LogError(this.GetType() + " GameObject not found at step " + _step + ": " + focusObjects[_step]);
                            next();
                        }
                        else
                        {
                            // Debug.Log(this.GetType() + " go != null at step=" + _step + ", go.transform.position=" + go.transform.position + " & go.transform.localPosition=" + go.transform.localPosition);
                            ExternalOnPressButton target = go.GetComponent<ExternalOnPressButton>();
                            if (null != target)
                            {
                                // Debug.Log(this.GetType() + " target != null at step=" + _step
                                //+ " with text=" + textHints[_step]
                                //);
                                focusMaskManager.focusOn(target, next, textHints[_step], true);
                            }
                            else
                            {
                                // Debug.Log(this.GetType() + " target == null at step=" + _step
                                //+ " with text=" + textHints[_step]
                                //);
                                focusMaskManager.focusOn(go, next, textHints[_step], true);
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
            waited += Time.fixedDeltaTime;
        }
        else
        {
            // Debug.Log(this.GetType() + " end");
            end();
        }
    }

    protected virtual void end()
    {
        focusMaskManager.stopFocusOn();
        StepByStepTutorial._isPlaying = false;
        Destroy(this);
    }
}
