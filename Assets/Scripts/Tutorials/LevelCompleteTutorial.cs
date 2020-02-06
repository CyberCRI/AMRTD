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
            return _focusObjects.Length;
        }
    }
    private string[] _focusObjects = new string[1] {
        FocusMaskManager.waveBarGOName
        };

    protected override string[] focusObjects
    {
        get
        {
            return _focusObjects;
        }
    }

    protected override void end()
    {
        base.end();
        Destroy(this.transform.parent.gameObject);
    }
}
#endif