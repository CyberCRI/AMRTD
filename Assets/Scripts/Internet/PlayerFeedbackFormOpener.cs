using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class PlayerFeedbackFormOpener : LinkOpener
{
    private const string preFilledUrl = 
        // 0.31 pre-filled link to the Google form - general usability / playability form
        //   "https://docs.google.com/forms/d/e/1FAIpQLScD1Y9VCrYYdOfJS07f0e-eKYA8ej4TkZDH-SR3OpQF3jRsaA/viewform?usp=pp_url&entry.816266961=";
        // Specific for the 0.32 2020-04-24 playtest - still a general usability / playability form
        //   "https://docs.google.com/forms/d/e/1FAIpQLSdSidpyYHS2mz4fVggBj8AlxmlVLGRY_OBgZDKTcxNLAbcLqQ/viewform?usp=pp_url&entry.816266961=";
        // Specific for the 0.33 2020-05-12 playtest - still a general usability / playability form
        //   "https://docs.google.com/forms/d/e/1FAIpQLSdm8JmZNT90FioYhXOLtfUo7hbFUrfiW5Bkl9tWojreeEvhEg/viewform?usp=pp_url&entry.816266961=";
        // 0.34
           "https://docs.google.com/forms/d/e/1FAIpQLSd9EqVR0SjuBxfbHbXm4GAACcbFMHXvw9eJQ6dPvZKZbNJsGw/viewform?usp=pp_url&entry.816266961=";

    protected override string getURL()
	{
        return preFilledUrl + GameConfiguration.instance.playerGUID;
	}

    public void clickButton()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKFEEDBACKFORM, CustomData.getGameLevelContext());
        AudioManager.instance.play(AudioEvent.CLICKUI);
        openLink();
    }
}