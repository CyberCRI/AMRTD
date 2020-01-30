//#define QUICKTEST

#if QUICKTEST

public class Level1Tutorial : FakeStepByStepTutorial { }

#else

public class Level1Tutorial : StepByStepTutorial
{
    private const string _level1Tutorial = "Level1Tutorial";
    private const string _turretButton = FocusMaskManager.standardTurretItemGOName;

    private const string _textKeyPrefix = _genericTextKeyPrefix + "LEVEL1.";
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
    private string[] _focusObjects = new string[2] {
        _level1Tutorial
        , _turretButton
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