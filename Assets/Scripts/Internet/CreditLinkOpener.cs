using UnityEngine;
using UnityEngine.UI;

public class CreditLinkOpener : LinkOpener
{
    [SerializeField]
    private TrackingEvent _te = TrackingEvent.CLICKCREDITMEMBER;
    private string _urlCode = null;
    private string _url = null;
    
    // Use this for initialization
    void Start()
    {
        onLanguageChanged();
        LocalizationManager.languageChanged.AddListener(onLanguageChanged);
    }
    
    void onLanguageChanged()
    {
        setURLCode(_urlCode);
    }

    public void setURLCode(string urlCode, string url = null)
	{
        _urlCode = urlCode;

        this.gameObject.SetActive(!string.IsNullOrEmpty(_urlCode));

        // url or translation of _urlCode
        if (!string.IsNullOrEmpty(_urlCode) && string.IsNullOrEmpty(url))
        {
            string localizedURL = LocalizationManager.instance.getLocalizedValue(_urlCode);
            if (!string.IsNullOrEmpty(localizedURL))
            {                
                _url = localizedURL;
            }
            else
            {
                _url = "";
            }
        }
        else
        {
            _url = url;
        }
	}

    public static string getURLCodeLocalization(string urlCode)
    {
        string localized = string.IsNullOrEmpty(urlCode) ? null : LocalizationManager.instance.getLocalizedValue(urlCode);
        return localized;
    }

    protected override string getURL()
	{
        return _url;
	}

    public void clickButton()
    {
        RedMetricsManager.instance.sendEvent(_te, CustomData.getGameLevelContext().add(CustomDataTag.ELEMENT, _urlCode));
        if (!string.IsNullOrEmpty(_url))
        {
            AudioManager.instance.play(AudioEvent.CLICKUI);
            openLink();
        }
    }
}