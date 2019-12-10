using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUILinkerUI : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.link(
            levelDurationCountdownText
            ,gameOverUI
            ,completeLevelUI
            );
        BuildManager.instance.link(nodeUI);
        PauseUI.instance.link(pauseUI);
    }
}
