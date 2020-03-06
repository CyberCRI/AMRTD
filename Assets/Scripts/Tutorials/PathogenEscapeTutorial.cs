//#define QUICKTEST
using UnityEngine;

#if QUICKTEST

public class PathogenEscapeTutorial : FakeStepByStepTutorial { }

#else

public class PathogenEscapeTutorial : StepByStepTutorial
{
    private const string _textKeyPrefix = genericTextKeyPrefix + "PATHOGENESCAPE.";
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

    private TutorialStep[] _steps = new TutorialStep[1] {
        new TutorialStep(FocusMaskManager.livesCounterGOName)
        };
    protected override TutorialStep[] steps
    {
        get
        {
            return _steps;
        }
    }

    protected override void end()
    {
        base.end();
        Destroy(this.transform.parent.gameObject);
    }
}
#endif