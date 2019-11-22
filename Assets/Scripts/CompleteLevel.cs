﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLevel : MonoBehaviour
{
    [SerializeField]
    private SceneFader sceneFader = null;
    private string nextLevelName = "";
    private int nextLevelIndex = 0;
    [SerializeField]
    private string overrideNextLevelName = "";
    [SerializeField]
    private int overrideNextLevelIndex = -1;

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
        }
        else
        {
            // level numbers are not indexes
            // after scene0 = Level1, scene1 = Level2 (index 0, number 1)
            // after scene1 = Level2, scene2 = Level3 (index 1, number 2)
            // after scene4 = Level5, scene0 = Level1 (index 4, number 5)
            string currentScene = SceneManager.GetActiveScene().name;
            int currentLevelNumber = int.Parse(currentScene.Substring(currentScene.Length - 1));
            nextLevelIndex = currentLevelNumber % (LevelSelector.maxLevelIndex + 1);
            nextLevelName = "Level" + (nextLevelIndex + 1).ToString();
        }
    }

    public void pressContinue()
    {
        int previous = PlayerPrefs.GetInt(LevelSelector.levelReachedKey);
        int newLevelReached = Mathf.Max(previous, nextLevelIndex);
        PlayerPrefs.SetInt(LevelSelector.levelReachedKey, newLevelReached);
        sceneFader.fadeTo(nextLevelName);
    }

    public void pressMenu()
    {
        sceneFader.menu();
    }
}
