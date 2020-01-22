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
    private GameObject pauseUI = null;
    [SerializeField]
    private GameObject menuUI = null;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.linkUI(
            levelDurationCountdownText
            ,gameOverUI
            ,completeLevelUI
            );
        BuildManager.instance.linkUI(nodeUI);
        PauseUI.instance.linkUI(pauseUI);
        MenuUI.instance.linkUI(menuUI);
        WaveSpawner.instance.linkUI(waveCountdownText);
    }
}
