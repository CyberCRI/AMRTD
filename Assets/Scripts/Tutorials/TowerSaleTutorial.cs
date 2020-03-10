//#define QUICKTEST
using UnityEngine;

#if QUICKTEST

public class TowerSaleTutorial : FakeStepByStepTutorial { }

#else

public class TowerSaleTutorial : StepByStepTutorial
{
    [SerializeField]
    private GameObject tower = null;

    [SerializeField]
    private const string _textKeyPrefix = genericTextKeyPrefix + "TOWERSALE.";
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

    private TutorialStep[] _steps = new TutorialStep[0];
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
    void Awake()
    {
        _steps = new TutorialStep[1] {
                    new TutorialStep(tower.name + cloneSuffix)
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