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
        _url = (null == url) && (null != _urlCode) ? LocalizationManager.instance.getLocalizedValue(_urlCode) : url;
	}

    protected override string getURL()
	{
        return _url;
	}

    public void clickButton()
    {
        RedMetricsManager.instance.sendEvent(_te, CustomData.getGameLevelContext().add(CustomDataTag.ELEMENT, _urlCode));
        AudioManager.instance.play(AudioEvent.CLICKUI);
        openLink();
    }
}