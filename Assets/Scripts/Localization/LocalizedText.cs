using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string key;
    [SerializeField]
    private Text text = null;

    // Use this for initialization
    void Start()
    {
        if (null == text)
        {
            text = GetComponent<Text>();
        }
        onLanguageChanged();
        LocalizationManager.languageChanged.AddListener(onLanguageChanged);
    }
    
    void onLanguageChanged()
    {
        if (null == text)
        {
            text = GetComponent<Text>();
        }
        if (!string.IsNullOrEmpty(key))
        {
            text.text = LocalizationManager.instance.getLocalizedValue(key);
        }
    }

    public void setKey(string _key)
    {
        key = _key;
        onLanguageChanged();
    }

    public bool hasKey()
    {
        return !string.IsNullOrEmpty(key);
    }
}