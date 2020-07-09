//#define VERBOSEDEBUG
//#define DEVMODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public static GameUI instance = null;

    [SerializeField]
    private Canvas canvas = null;
    [SerializeField]
    private Toggle chatToggle = null;
    [SerializeField]
    private GameObject chatbotInteractor = null;
    [SerializeField]
    private LevelIntroUI levelIntroUI = null;
    [SerializeField]
    private GameObject moneyFeedback = null;
    [SerializeField]
    private GameObject worthDisplay = null;

#if DEVMODE
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //playMoneyFeedback(Random.Range(-3, 3));
            
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Enemy.enemyTag);
            foreach (GameObject enemyGO in enemies)
            {
                Enemy enemy = enemyGO.GetComponent<Enemy>();
                enemy.displayWorth();
            }
        }
    }
#endif

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void goToMainMenu()
    {
        //GameManager.instance.setPause(false, GameManager.pauseUIPauserKey);
        SceneFader.instance.goToMainMenu();
    }

    private void retry()
    {
        //GameManager.instance.setPause(false, GameManager.pauseUIPauserKey);
        SceneFader.instance.retry();
    }

    // called from game screen, to open the pause screen
    public void pressPauseButton()
    {
        Debug.LogError(this.GetType() + " DEPRECATED CALL");
#if VERBOSEDEBUG
        Debug.Log("GameUI pressPauseButton");
#endif
        CustomDataValue customDataValue = GameManager.instance.isPaused() ? CustomDataValue.OFF : CustomDataValue.ON;
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKPAUSE, new CustomData(CustomDataTag.OUTCOME, customDataValue));
        AudioManager.instance.play(AudioEvent.CLICKUI);

        GameManager.instance.togglePause();
    }

    // called from game screen, to open retry screen
    public void pressRetryButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKRETRY);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        GameManager.instance.setPause(true, GameManager.retryUIPauserKey);
        RetryUI.instance.setActive(true);
    }

    // called from retry screen, to actually retry the level
    public void pressRetryRetryButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKRETRYRETRY, CustomData.getLevelEndContext());
        AudioManager.instance.play(AudioEvent.CLICKUI);
        //GameManager.instance.setPause(false, GameManager.retryUIPauserKey);
        retry();
    }

    // called from retry screen, to close retry screen and thus resume playing
    public void pressRetryResumeButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKRETRYRESUME);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        GameManager.instance.setPause(false, GameManager.retryUIPauserKey);
        RetryUI.instance.setActive(false);
    }

    // called from game screen, to open menu screen
    public void pressMenuButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKMENU);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        GameManager.instance.setPause(true, GameManager.menuUIPauserKey);
        MenuUI.instance.setActive(true);
    }

    // called from menu screen, to confirm and go to main menu screen
    public void pressMenuMenuButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKMENUMENU);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        //GameManager.instance.setPause(false, GameManager.menuUIPauserKey);
        goToMainMenu();
    }

    // called from menu screen, to resume playing
    public void pressMenuResumeButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKMENURESUME);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        GameManager.instance.setPause(false, GameManager.menuUIPauserKey);
        MenuUI.instance.setActive(false);
    }

    // called from complete screen, to validate completion and go on to the next level
    public void pressCompleteCompleteButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKCOMPLETECOMPLETE);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        //GameManager.instance.setPause(false, GameManager.completeLevelPauserKey);
        CompleteLevel.instance.pressContinue();
    }

    // called from complete screen, to go to the menu screen
    public void pressCompleteMenuButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKCOMPLETEMENU);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        //GameManager.instance.setPause(false, GameManager.completeLevelPauserKey);
        goToMainMenu();
    }

    // called from complete screen, to retry the level
    public void pressCompleteRetryButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKCOMPLETERETRY);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        //GameManager.instance.setPause(false, GameManager.completeLevelPauserKey);
        retry();
    }

    // called from game over screen, to retry the level
    public void pressGameOverRetryButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKGAMEOVERRETRY);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        //GameManager.instance.setPause(false, GameManager.gameOverPauserKey);
        retry();
    }

    // called from game over screen, to go to the menu screen
    public void pressGameOverMenuButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKGAMEOVERMENU);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        //GameManager.instance.setPause(false, GameManager.gameOverPauserKey);
        goToMainMenu();
    }

    public void pressChatToggle()
    {
        #if VERBOSEDEBUG
        Debug.Log("pressChatToggle");
        #endif
        // indicates what is the desired state of the button
        CustomDataValue customDataValue = CustomDataValue.OFF;
        string soundParameter = "off";
        if (chatToggle.isOn)
        {
            customDataValue = CustomDataValue.ON;
            soundParameter = "on";
        }
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKCHATBOT, new CustomData(CustomDataTag.OUTCOME, customDataValue));
        AudioManager.instance.play(AudioEvent.CLICKCHATBOT, soundParameter);
        GameManager.instance.setPause(chatToggle.isOn, GameManager.chatbotUIPauserKey);
        chatbotInteractor.SetActive(chatToggle.isOn);
    }

    // called from game start intro screen, to start playing
    public void pressStartButton(GameObject introScreen)
    {
        // unpause and hide
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKINTROSTART);
        AudioManager.instance.play(AudioEvent.CLICKUI);
        introScreen.SetActive(false);
    }

    public void playMoneyFeedback(int amount, GameObject moneySource = null)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " playMoneyFeedback(" + amount + ", go)");
        #endif

        if (null != moneySource)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(moneySource.transform.position);
            playMoneyFeedback(amount, new Vector2(screenPoint.x, screenPoint.y));
        }
        else
        {
            Debug.LogWarning(this.GetType() + " playMoneyFeedback moneySource == null");
            playMoneyFeedback(amount, Vector2.zero);
        }
    }

    public void playMoneyFeedback(int amount, Vector2 moneySourcePosition)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " playMoneyFeedback(" + amount + ", " + moneySourcePosition + ")");
        #endif

        GameObject feedback = Instantiate(moneyFeedback);
        MoneyFeedbackUI mfui = feedback.GetComponent<MoneyFeedbackUI>();
        mfui.setup(amount, canvas.transform, moneySourcePosition);
    }

    public void displayWorth(int amount, Transform worthSource1, Transform worthSource2 = null)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " displayWorth(" + amount + ", tr1, tr2)");
        #endif

        GameObject feedback = Instantiate(worthDisplay);
        WorthDisplayUI wdui = feedback.GetComponent<WorthDisplayUI>();
        wdui.setup(amount, canvas.transform, worthSource1, worthSource2);

    }

    public void linkCommon(Camera camera)
    {
        canvas.worldCamera = camera;
    }

    public void startLevelIntro()
    {
        if (null != levelIntroUI)
        {
            levelIntroUI.gameObject.SetActive(true);
        }
    }
}
