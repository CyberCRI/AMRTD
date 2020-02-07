//#define QUICKTEST
using UnityEngine;

#if QUICKTEST

public class PathogenEscapeTutorial : FakeStepByStepTutorial { }

#else

public class PathogenEscapeTutorial : StepByStepTutorial
{
    private const string _textKeyPrefix = _genericTextKeyPrefix + "PATHOGENESCAPE.";
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
        FocusMaskManager.livesCounterGOName
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