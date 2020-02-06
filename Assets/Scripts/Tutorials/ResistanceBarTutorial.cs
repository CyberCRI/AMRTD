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
            return _focusObjects.Length;
        }
    }
    private string[] _focusObjects = new string[1] {
        FocusMaskManager.resistanceBarGOName
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