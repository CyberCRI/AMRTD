public enum CustomDataValue
{
    // generic states
    ON,
    OFF,
    SUCCESS,
    FAILURE,

    // main menu entries
    START,
    RESUME,
    RESTART,
    SETTINGS,
    CONTROLS,
    LANGUAGE,
    GRAPHICS,
    SOUND,
    SCIENCE,
    LEARNMORE,
    CONTRIBUTE,
    QUIT,

    GAME,               // source of event: some events can be triggered from game or webpage
    WEBPAGE,
    QUITYES,
    QUITNO,
    CONTRIBUTEMAINMENU,
    CONTRIBUTEHUD,
    CONTRIBUTEEND,
    CONTRIBUTEQUIT,
    CONTRIBUTETOOLBAR,
    CONTRIBUTESPEECHBUBBLE,
    QUITCROSSMENU,
    QUITCROSSHUD,
    RESETCONFIGURATION,
}
