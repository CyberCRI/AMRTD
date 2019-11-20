using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [SerializeField]
    private Text livesText = null;
    private const string livesKey = "GAME.LIVESCOUNTER.LIVES";
    private string translatedValue;

    void Start()
    {
        onLanguageChanged();
        LocalizationManager.languageChanged.AddListener(onLanguageChanged);
    }

    // Update is called once per frame
    void Update()
    {
        livesText.text = PlayerStatistics.lives.ToString() + " " + translatedValue;
    }

    private void onLanguageChanged()
    {
        translatedValue = LocalizationManager.instance.GetLocalizedValue(livesKey);
    }
}
