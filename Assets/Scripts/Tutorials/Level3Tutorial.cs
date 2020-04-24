//#define QUICKTEST
//#define VERBOSEDEBUG
using UnityEngine;

#if QUICKTEST

public class Level3Tutorial : FakeStepByStepTutorial { }

#else

public class Level3Tutorial : StepByStepTutorial
{
    private const string _level3Tutorial = "Level3Tutorial";
    
    [SerializeField]
    private GameObject toHide1 = null;

    private const string _textKeyPrefix = genericTextKeyPrefix + "LEVEL3.";
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

    private TutorialStep[] _steps = new TutorialStep[2] {
        new TutorialStep(_level3Tutorial, TUTORIALACTION.SETGREYBACKGROUND),
        new TutorialStep(_level3Tutorial, TUTORIALACTION.SETGREYBACKGROUND)
        };
    protected override TutorialStep[] steps
    {
        get
        {
            return _steps;
        }
    }

    protected override void prepareStep(int step)
    {
        if (step == 1)
        {
            toHide1.SetActive(false);
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