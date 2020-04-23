//#define QUICKTEST
//#define VERBOSEDEBUG
using UnityEngine;

#if QUICKTEST

public class CovidLevelTutorial : FakeStepByStepTutorial { }

#else

public class CovidLevelTutorial : StepByStepTutorial
{
    private const string _covidLevelTutorial = "CovidLevelTutorial";

    [SerializeField]
    private GameObject toHide1 = null;
    [SerializeField]
    private GameObject toHide2 = null;
    [SerializeField]
    private GameObject toHide3 = null;
    [SerializeField]
    private GameObject toHide4 = null;

    private const string _textKeyPrefix = genericTextKeyPrefix + "COVIDLEVELINTRO.";
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
        _steps = new TutorialStep[5] {
                    new TutorialStep(_covidLevelTutorial, TUTORIALACTION.SETGREYBACKGROUND),
                    new TutorialStep(_covidLevelTutorial, TUTORIALACTION.SETGREYBACKGROUND),
                    new TutorialStep(_covidLevelTutorial, TUTORIALACTION.SETGREYBACKGROUND),
                    new TutorialStep(_covidLevelTutorial, TUTORIALACTION.SETGREYBACKGROUND),
                    new TutorialStep(_covidLevelTutorial, TUTORIALACTION.SETGREYBACKGROUND),
                };
        base.Awake();
    }

    protected override void prepareStep(int step)
    {
        if (step == 1)
        {
            toHide1.SetActive(false);
        }
        else if (step == 2)
        {
            toHide2.SetActive(false);
        }
        else if (step == 3)
        {
            toHide3.SetActive(false);
        }
        else if (step == 4)
        {
            toHide4.SetActive(false);
        }
    }

    protected override void end()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " override end");
#endif
        base.end();
        Destroy(this.transform.parent.gameObject);
    }
}
#endif