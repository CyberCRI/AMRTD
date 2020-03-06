using UnityEngine;

public class HelpButtonTutorialActivator : TutorialActivator
{
    private const int activationWaveIndex = 2;
    private WaveSpawner waveSpawner;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        waveSpawner = WaveSpawner.instance;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (waveSpawner.isNthWave(activationWaveIndex))
        {
            activateTutorial();
        }
    }
}