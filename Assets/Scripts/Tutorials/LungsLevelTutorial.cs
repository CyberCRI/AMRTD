//#define QUICKTEST
//#define DEVMODE
using UnityEngine;

#if QUICKTEST

public class LungsLevelTutorial : FakeStepByStepTutorial { }

#else

public class LungsLevelTutorial : StepByStepTutorial
{
    [SerializeField]
    private GameObject target = null;
    private const string _lungsLevelTutorial = "LungsLevelTutorial";

    [SerializeField]
    private GameObject toHide = null;

    private const string _textKeyPrefix = genericTextKeyPrefix + "LUNGSLEVEL.";
    protected override string textKeyPrefix
    {
        get
        {
            return _textKeyPrefix;
        }
    }

    protected override int stepCount
    {
        get
        {
            return _steps.Length;
        }
    }

    private TutorialStep[] _steps = new TutorialStep[2];
    protected override TutorialStep[] steps
    {
        get
        {
            return _steps;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    new void Awake()
    {
        _steps = new TutorialStep[2] {
                    new TutorialStep(_lungsLevelTutorial, TUTORIALACTION.SETGREYBACKGROUND)
                    , new TutorialStep(target.name)
                };
        base.Awake();
    }

    protected override void prepareStep(int step)
    {
        if (step == 1)
        {
            toHide.SetActive(false);
        }
    }

    protected override void end()
    {
#if DEVMODE
        Debug.Log(this.GetType() + " override end");
#endif
        base.end();
        Destroy(this.transform.parent.gameObject);
    }
}
#endif