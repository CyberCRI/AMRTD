//#define QUICKTEST
//#define VERBOSEDEBUG
using UnityEngine;

#if QUICKTEST

public class Level6Tutorial : FakeStepByStepTutorial { }

#else

public class Level6Tutorial : StepByStepTutorial
{
    private const string _level6Tutorial = "Level6Tutorial";
    private const string _turretButton = FocusMaskManager.laserBeamerItemGOName;

    private const string _textKeyPrefix = genericTextKeyPrefix + "LEVEL6.";
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
        new TutorialStep(_turretButton)
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
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " override end");
#endif
        base.end();
        Destroy(this.transform.parent.gameObject);
    }
}
#endif