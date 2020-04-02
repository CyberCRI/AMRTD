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

    OPTION,             // generic tag
    STATE,              // generic tag
    ELEMENT,            // generic tag

    CONTROLS,
    LANGUAGE,
    GRAPHICS,
    SOUND,

    NEWTAB,
    SAMETAB,

    MESSAGE,            // hint message that was displayed

    DURATION
}
