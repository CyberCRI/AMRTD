using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class PlayerFeedbackFormOpener : LinkOpener
{
    protected override string getURL()
	{
        return "https://docs.google.com/forms/d/e/1FAIpQLScD1Y9VCrYYdOfJS07f0e-eKYA8ej4TkZDH-SR3OpQF3jRsaA/viewform?usp=pp_url&entry.816266961=" + GameConfiguration.instance.playerGUID;
	}

    public void clickButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKFEEDBACKFORM, CustomData.getGameLevelContext());
        openLink();
    }
}