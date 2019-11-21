using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string key;
    private Text text = null;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        onLanguageChanged();
        LocalizationManager.languageChanged.AddListener(onLanguageChanged);
    }
    
    void onLanguageChanged()
    {
        text.text = LocalizationManager.instance.GetLocalizedValue(key);
    }

    public void setKey(string _key)
    {
        key = _key;
        onLanguageChanged();
    }

}