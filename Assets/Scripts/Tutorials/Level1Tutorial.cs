//#define QUICKTEST
//#define DEVMODE
using UnityEngine;

#if QUICKTEST

public class Level1Tutorial : FakeStepByStepTutorial { }

#else

public class Level1Tutorial : StepByStepTutorial
{
    private const string _level1Tutorial = "Level1Tutorial";
    private const string _turretButton = FocusMaskManager.standardTurretItemGOName;

    [SerializeField]
    private GameObject toHide = null;

    private const string _textKeyPrefix = _genericTextKeyPrefix + "LEVEL1.";
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
        new TutorialStep(_level1Tutorial, TUTORIALACTION.SETGREYBACKGROUND)
        , new TutorialStep(_turretButton)
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