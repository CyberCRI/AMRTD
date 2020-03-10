using UnityEngine;

public class TutorialActivator : MonoBehaviour
{
    [SerializeField]
    private StepByStepTutorial tutorial = null;
    [Tooltip("If the delay d is not zero and the Start method is not overridden, the tutorial will be activated after d seconds.")]
    [SerializeField]
    private float delay = 0f;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (0f != delay)
        {
            Invoke("activateTutorial", delay);
        }
        else
        {
            SceneFader.instance.setFadeInEndCallback(activateTutorial);
        }
        this.enabled = false;
    }

    public void activateTutorial()
    {
        tutorial.gameObject.SetActive(true);
        this.enabled = false;
    }
}