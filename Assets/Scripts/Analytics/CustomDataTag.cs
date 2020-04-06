public enum CustomDataTag
{
    LOCALPLAYERGUID,    //for GUID stored in local PlayerPrefs
    GLOBALPLAYERGUID,   //for GUID associated to an account

    PLATFORM,           //the runtime platform on which the game is run

    SOURCE,             // source of event - webpage or game

    // event context
    LIVES,              // number of lives ie number of pathogens that can escape before losing
    FUNDS,              // money available to buy/upgrade towers
    RESISTANCE,         // resistance points accumulated when building towers
    WAVES,              // waves of pathogens that already happened
    GAMELEVEL,          // game level ie scenario, map being played

    // generic tags
    OPTION,
    STATE,
    ELEMENT,
    OUTCOME,
    DURATION,
    COST,

    CONTROLS,
    LANGUAGE,
    GRAPHICS,
    SOUND,

    NEWTAB,
    SAMETAB,

    MESSAGE,            // tutorial message that was displayed

    // elapsed times
    TIMESINCEGAMELOADED,           // Time.realtimeSinceStartup / Time.unscaledTime
    TIMEGAMEPLAYEDNOPAUSE,         // Time.time
    TIMESINCELEVELLOADED,          // Time.timeSinceLevelLoad
    TIMELEVELPLAYEDNOPAUSE,        // ?
}
