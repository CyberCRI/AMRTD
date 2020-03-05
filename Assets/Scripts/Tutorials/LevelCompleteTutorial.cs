//#define QUICKTEST
using UnityEngine;

#if QUICKTEST

public class LevelCompleteTutorial : FakeStepByStepTutorial { }

#else

public class LevelCompleteTutorial : StepByStepTutorial
{
    [SerializeField]
    private const string _textKeyPrefix = _genericTextKeyPrefix + "LEVELCOMPLETE.";
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
        new TutorialStep(FocusMaskManager.waveBarGOName)
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