using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class PlayerFeedbackFormOpener : LinkOpener
{
    private const string preFilledUrl = 
        // pre-filled link to the Google form - general usability / playability form
        // "https://docs.google.com/forms/d/e/1FAIpQLScD1Y9VCrYYdOfJS07f0e-eKYA8ej4TkZDH-SR3OpQF3jRsaA/viewform?usp=pp_url&entry.816266961=" + GameConfiguration.instance.playerGUID;
        // Specific for the 2020-04-24 playtest - still a general usability / playability form
        "https://docs.google.com/forms/d/e/1FAIpQLSdSidpyYHS2mz4fVggBj8AlxmlVLGRY_OBgZDKTcxNLAbcLqQ/viewform?usp=pp_url&entry.816266961=";

    protected override string getURL()
	{
        return preFilledUrl + GameConfiguration.instance.playerGUID;
	}

    public void clickButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKFEEDBACKFORM, CustomData.getGameLevelContext());
        openLink();
    }
}