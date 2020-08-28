using UnityEngine;

public class PathogenKilledByWBCTutorialActivator : TutorialActivator
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        WhiteBloodCellManager.instance.setDeathToBacteriumCallback(callback);
    }

    void callback(Transform wbcT)
    {
        activateTutorial();
    }

/*
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (
            (PlayerStatistics.instance.lives < PlayerStatistics.instance.startLives)
            &&
            (WhiteBloodCellManager.instance.isOneWBCDead())
        )
        {
            activateTutorial();
        }
    }
*/
}