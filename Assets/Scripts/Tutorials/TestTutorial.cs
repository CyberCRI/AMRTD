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
    private const int _stepCount = 3;
    protected override int stepCount
    {
        get
        {
            return _stepCount;
        }
    }
    private string[] _focusObjects = new string[_stepCount] {
        _timer
        , _resistanceBar
        , _lifeBar
        };
    protected override string[] focusObjects
    {
        get
        {
            return _focusObjects;
        }
    }
}
#endif