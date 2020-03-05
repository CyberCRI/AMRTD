//#define QUICKTEST

#if QUICKTEST

public class TestTutorial : FakeStepByStepTutorial { }

#else

public class TestTutorial : StepByStepTutorial
{

    private const string _timer = FocusMaskManager.waveTimerTextGOName;
    private const string _resistanceBar = FocusMaskManager.resistanceBarGOName;
    private const string _lifeBar = FocusMaskManager.lifeBarGOName;

    private const string _textKeyPrefix = _genericTextKeyPrefix + "TEST.";
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
        new TutorialStep(_timer)
        , new TutorialStep(_resistanceBar)
        , new TutorialStep(_lifeBar)
        };
    protected override TutorialStep[] steps
    {
        get
        {
            return _steps;
        }
    }
}
#endif