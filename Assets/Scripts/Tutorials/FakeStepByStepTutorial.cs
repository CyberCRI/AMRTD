public class FakeStepByStepTutorial : StepByStepTutorial
{
    private const string _textKeyPrefix = genericTextKeyPrefix;
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

    private TutorialStep[] _steps = new TutorialStep[0] { };
    protected override TutorialStep[] steps
    {
        get
        {
            return _steps;
        }
    }
}
