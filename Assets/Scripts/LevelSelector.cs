using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [SerializeField]
    private SceneFader sceneFader = null;

    public void select(string levelName)
    {
        sceneFader.fadeTo(levelName);
    }
}
