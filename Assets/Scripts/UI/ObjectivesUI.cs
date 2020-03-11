#define LIVESICON

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesUI : MonoBehaviour
{
    [SerializeField]
    private Text objectivesText = null;
    private const string objectivesKey = "GAME.OBJECTIVESCOUNTER.LIVES";
    private string translatedValue;

    void Start()
    {
        if (!GameManager.instance.isObjectiveDefenseMode())
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
        objectivesText.text = ObjectiveDefenseMode.instance.getCapturedObjectivesStats() + " " + translatedValue;
    }

    private void onLanguageChanged()
    {
        translatedValue = LocalizationManager.instance.GetLocalizedValue(objectivesKey);
    }
#else
    }
    
    // Update is called once per frame
    void Update()
    {
        objectivesText.text = ObjectiveDefenseMode.instance.getCapturedObjectivesStats();
    }

#endif
}
