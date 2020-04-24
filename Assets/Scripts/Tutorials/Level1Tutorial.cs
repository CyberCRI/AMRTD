//#define QUICKTEST
//#define VERBOSEDEBUG
using UnityEngine;

#if QUICKTEST

public class Level1Tutorial : FakeStepByStepTutorial { }

#else

public class Level1Tutorial : StepByStepTutorial
{
    private const string _level1Tutorial = "Level1Tutorial";
    private const string _turretButton = FocusMaskManager.standardTurretItemGOName;

    [SerializeField]
    private GameObject toHide1 = null;
    [SerializeField]
    private GameObject toHide2 = null;

    private const string _textKeyPrefix = genericTextKeyPrefix + "LEVEL1.";
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

    private TutorialStep[] _steps = new TutorialStep[3] {
        new TutorialStep(_level1Tutorial, TUTORIALACTION.SETGREYBACKGROUND),
        new TutorialStep(_level1Tutorial, TUTORIALACTION.SETGREYBACKGROUND),
        new TutorialStep(_turretButton)
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
        else if (step == 2)
        {
            toHide2.SetActive(false);
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