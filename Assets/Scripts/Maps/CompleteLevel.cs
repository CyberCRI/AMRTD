//#define VERBOSEDEBUG
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLevel : MonoBehaviour
{
    public static CompleteLevel instance;
    private GameObject completeLevelUI = null;
    private string nextLevelName = "";
    private int nextLevelIndex = 0;
    [SerializeField]
    private string overrideNextLevelName = "";
    [SerializeField]
    private int overrideNextLevelIndex = -1;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if ((0 != overrideNextLevelName.Length) || (-1 != overrideNextLevelIndex))
        {
            nextLevelName = overrideNextLevelName;
            nextLevelIndex = overrideNextLevelIndex;
#if VERBOSEDEBUG
            Debug.Log(string.Format(this.GetType() + " Start read nextLevelName={0}, nextLevelIndex={1}", nextLevelName, nextLevelIndex));
#endif
        }
        else
        {
            // level numbers are not indexes
            // after scene0 = Level1, scene1 = Level2 (index 0, number 1)
            // after scene1 = Level2, scene2 = Level3 (index 1, number 2)
            // after scene4 = Level5, scene0 = Level1 (index 4, number 5)
            string currentScene = SceneManager.GetActiveScene().name;
            //int currentLevelNumber = int.Parse(currentScene.Substring(currentScene.Length - 1));
            string pattern = "%d+(?!.+)"; // searches for a final number
            Match m = Regex.Match(currentScene, pattern);
            if (m.Success)
            {
                int currentLevelNumber = int.Parse(m.Value);
                nextLevelIndex = currentLevelNumber % (GameConfiguration.gameLevelCount);
                nextLevelName = "Level" + (nextLevelIndex + 1).ToString();
                m = m.NextMatch();
#if VERBOSEDEBUG
                Debug.Log(string.Format(this.GetType() + " Start computed nextLevelName={0}, nextLevelIndex={1}", nextLevelName, nextLevelIndex));
#endif
            }
            else
            {
                Debug.LogError("could not get level index from current scene name " + currentScene);
            }
            if (m.Success)
            {
                Debug.LogError("error while processing level index from current scene name " + currentScene);
            }
        }
    }
    
    public void linkUI(GameObject _completeLevelUI)
    {
        completeLevelUI = _completeLevelUI;
    }

    public void completeLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name.ToLowerInvariant();
        RedMetricsManager.instance.sendEvent(TrackingEvent.COMPLETELEVEL, CustomData.getLevelEndContext());
        
        GameConfiguration.instance.reachedLevel(nextLevelIndex, nextLevelName);

        // TODO assumes linear unlocking of levels
        if (LevelSelectionUI.lastScene.ToLowerInvariant() == sceneName)
        {
            RedMetricsManager.instance.sendEvent(TrackingEvent.COMPLETEGAME, CustomData.getContext(
                    new CustomDataTag[3]{
                        CustomDataTag.TIMESINCEGAMELOADED,
                        CustomDataTag.TIMEGAMEPLAYEDNOPAUSE,
                        CustomDataTag.TIMESINCELEVELLOADED,
                        }
                )
            );
        }

        if (null != completeLevelUI)
        {
            completeLevelUI.SetActive(true);
        }
    }

    public void pressContinue()
    {
        SceneFader.instance.fadeTo(nextLevelName);
    }
}
