//#define QUICKTEST
using UnityEngine;

#if QUICKTEST

public class WaveCountdownTutorial : FakeStepByStepTutorial { }

#else

public class WaveCountdownTutorial : StepByStepTutorial
{
    [SerializeField]
    private GameObject toPointAt = null;

    private const string _textKeyPrefix = genericTextKeyPrefix + "WAVECOUNTDOWN.";
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

    private TutorialStep[] _steps = new TutorialStep[1];
    protected override TutorialStep[] steps
    {
        get
        {
            return _steps;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    new void Awake()
    {
        _steps = new TutorialStep[1] {
                    new TutorialStep(toPointAt.name)
                };
        base.Awake();
    }

    protected override void end()
    {
        base.end();
        Destroy(this.transform.parent.gameObject);
    }
}
#endif