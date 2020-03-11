#define LIVESICON

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
        if (GameManager.instance.isObjectiveDefenseMode())
        {
            Destroy(this);
        }
#if !LIVESICON
        else
        {
            onLanguageChanged();
            LocalizationManager.languageChanged.AddListener(onLanguageChanged);
        }
    }

    // Update is called once per frame
    void Update()
    {
        livesText.text = PlayerStatistics.instance.lives.ToString() + " " + translatedValue;
    }

    private void onLanguageChanged()
    {
        translatedValue = LocalizationManager.instance.GetLocalizedValue(livesKey);
    }
#else
    }
    
    // Update is called once per frame
    void Update()
    {
        livesText.text = PlayerStatistics.instance.lives.ToString();
    }

#endif
}
