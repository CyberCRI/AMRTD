//#define QUICKTEST
using UnityEngine;

#if QUICKTEST

public class ResistanceBarTutorial : FakeStepByStepTutorial { }

#else

public class ResistanceBarTutorial : StepByStepTutorial
{
    [SerializeField]
    private int resistanceBarTutorialIndex = 0;
    protected override string textKeyPrefix
    {
        get
        {
            return _genericTextKeyPrefix + "RESISTANCEBAR" + resistanceBarTutorialIndex + ".";
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
        new TutorialStep(FocusMaskManager.resistanceBarGOName)
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