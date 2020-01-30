using UnityEngine;

public class TutorialActivator : MonoBehaviour
{
    [SerializeField]
    private StepByStepTutorial tutorial = null;
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
    }

    public void activateTutorial()
    {
        tutorial.gameObject.SetActive(true);
    }
}