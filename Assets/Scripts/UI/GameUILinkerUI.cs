using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUILinkerUI : MonoBehaviour
{
    [SerializeField]
    private Text waveCountdownText = null;
    [SerializeField]
    private Text levelDurationCountdownText = null;
    [SerializeField]
    private NodeUI nodeUI = null;
    [SerializeField]
    private GameObject gameOverUI = null;
    [SerializeField]
    private GameObject completeLevelUI = null;
    [SerializeField]
    private GameObject menuUI = null;
    [SerializeField]
    private GameObject pauseUI = null;
    [SerializeField]
    private Toggle pauseToggle = null;
    [SerializeField]
    private GameObject retryUI = null;

    // Start is called before the first frame update
    void Start()
    {
        CompleteLevel.instance.linkUI(completeLevelUI);
        GameManager.instance.linkUI(
            levelDurationCountdownText
            ,gameOverUI
            );
        BuildManager.instance.linkUI(nodeUI);
        MenuUI.instance.linkUI(menuUI);
        PauseUI.instance.linkUI(pauseUI, pauseToggle);
        RetryUI.instance.linkUI(retryUI);
        WaveSpawner.instance.linkUI(waveCountdownText);
    }
}
