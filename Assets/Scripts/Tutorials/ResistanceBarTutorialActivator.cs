using UnityEngine;

public class ResistanceBarTutorialActivator : TutorialActivator
{
    [SerializeField]
    private ResistanceBarUI resistanceBar;
    [SerializeField]
    private float threshold = .3f;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if ((null != resistanceBar) && (resistanceBar.getLatestValue() > threshold))
        {
            activateTutorial();
        }
    }
}