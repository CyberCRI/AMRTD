using UnityEngine;
using UnityEngine.UI;

public class CreditLinkOpener : LinkOpener
{
    [SerializeField]
    private TrackingEvent _te = TrackingEvent.CLICKCREDITMEMBER;
    private string _code = null;
    private string _url = null;

    public void setCode(string code)
	{
        _code = code;
	}

    public void setURL(string url)
	{
        _url = url;
	}

    protected override string getURL()
	{
        return _url;
	}

    public void clickButton()
    {
        RedMetricsManager.instance.sendEvent(_te, CustomData.getGameLevelContext().add(CustomDataTag.ELEMENT, _code));
        AudioManager.instance.play(AudioEvent.CLICKUI);
        openLink();
    }
}