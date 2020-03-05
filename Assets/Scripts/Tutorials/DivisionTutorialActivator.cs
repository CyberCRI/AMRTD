using UnityEngine;

public class DivisionTutorialActivator : TutorialActivator
{
    [SerializeField]
    private float additionalDelay = 0f;

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
        if (WaveSpawner.enemiesAliveCount == 2)
        {
            Invoke("activateTutorial", additionalDelay);
        }
    }
}