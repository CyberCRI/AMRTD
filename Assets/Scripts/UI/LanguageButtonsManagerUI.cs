using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButtonsManagerUI : MonoBehaviour
{
    private LanguageButtonUI[] languageButtons = null;
    [SerializeField]
    private Transform languageButtonsRoot = null;

    // Start is called before the first frame update
    void Start()
    {
        languageButtons = new LanguageButtonUI[languageButtonsRoot.childCount];
        CommonUtilities.fillArrayFromRoot<LanguageButtonUI>(languageButtonsRoot, ref languageButtons);

        updateSelected();
    }

    public void selectButton(LanguageButtonUI buttonUI, LocalizationManager.LANGUAGES target)
    {
        LocalizationManager.instance.language = target;
        updateSelected();
    }

    private void updateSelected()
    {
        LocalizationManager.LANGUAGES currentLanguage = LocalizationManager.instance.language;
        for (int i = 0; i < languageButtons.Length; i++)
        {
            languageButtons[i].setSelected(languageButtons[i].target == currentLanguage);
        }
    }
}
