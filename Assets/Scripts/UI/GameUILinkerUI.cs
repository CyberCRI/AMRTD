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
        WaveSpawner.instance.linkUI(waveCountdownText);
        GameManager.instance.linkUI(levelDurationCountdownText);
        BuildManager.instance.linkUI(nodeUI);
        MenuUI.instance.linkUI(menuUI);
        PauseUI.instance.linkUI(pauseUI, pauseToggle);
        RetryUI.instance.linkUI(retryUI);
    }
}
