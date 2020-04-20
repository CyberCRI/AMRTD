using UnityEngine;

//TODO: set version number
public enum TrackingEvent
{
    /////////////////////////////////////////////////////////
    // standard events
    DEFAULT, //[ ]
    CREATEPLAYER, //[x]
    START, //[x]              // launched the game; parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...
    END, //[x]                // quit the game; parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...

    /////////////////////////////////////////////////////////
    // specific events
    /////////////////////////////////////////////////////////
    
    /////////////////////////////////////////////////////////
    // all screens
    CLICKFEEDBACKFORM, //[x]      // parameter: GAMELEVEL

    // all screens - dev
    // DEVPRESS = using the dev keyboard shortcut
    // DEVCLICK = using the dev UI button
    DEVPRESSLANGUAGE, //[x]          // parameter: LANGUAGE
    DEVPRESSCURRENCY, //[x]          // 
    DEVPRESSLOSE, //[x]              // winLevel();
    DEVPRESSWIN, //[x]               // loseLevel();
    DEVPRESSINJUREALL, //[x]         // injureAllEnemies();
    DEVPRESSKILLALLBUTONE, //[x]     // killAllButOneEnemy();
    DEVPRESSDESTROYALLTURRETS, //[x] // destroyAllTurrets();
    DEVPRESSDELETEPLAYERPREFS, //[x] //
    
    DEVCLICKLEVELSLOCK, //[x]
    DEVPRESSLEVELSLOCK, //[x]
    DEVCLICKLEVELSUNLOCK, //[x]
    DEVPRESSLEVELSUNLOCK, //[x]
    
    DEVCUSTOMDEBUG, //[ ]            // to send manual custom messages
    DEVSWITCHFROMGAMEVERSION, //[x]  // parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...
    DEVSWITCHTOGAMEVERSION, //[x]    // parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...

    // alternative configuration routes - website-side feature not developed yet
    WEBCONFIGURE, //[ ]
    ADMINCONFIGURE, //[ ]

    /////////////////////////////////////////////////////////
    // non-playing screens
    CLICKLANGUAGE, //[x]          // using the UI button; parameter: OPTION language clicked on

    /////////////////////////////////////////////////////////
    // main screen
    CLICKPLAY, //[x]
    CLICKQUIT, //[x]

    /////////////////////////////////////////////////////////
    // level selection screen
    CLICKLEVEL, //[x]             // parameter: OPTION = level clicked on
    CLICKBACK, //[x]

    /////////////////////////////////////////////////////////
    // any level ////////////////////////////////////////////
    /////////////////////////////////////////////////////////

    LEVELSTARTS, //[x]

    // menu screen
    CLICKMENU, //[x]
    CLICKMENURESUME, //[x]
    CLICKMENUMENU, //[x]

    // pause screen
    CLICKPAUSE, //[x]
    CLICKPAUSERESUME, //[x]

    // retry screen
    CLICKRETRY, //[x]
    CLICKRETRYRESUME, //[x]
    CLICKRETRYRETRY, //[x]           // parameters: cf. getLevelEndContext

    // complete screen
    COMPLETELEVEL, //[x]             // event auto-sent when completing; parameters: cf. getLevelEndContext
    CLICKCOMPLETECOMPLETE, //[x]
    CLICKCOMPLETERETRY, //[x]
    CLICKCOMPLETEMENU, //[x]

    // game over screen
    GAMEOVER, //[x]                  // event auto-sent when losing a level; parameters: cf. getLevelEndContext
    CLICKGAMEOVERRETRY, //[x]
    CLICKGAMEOVERMENU, //[x]

    // help functionality
    CLICKHELP, //[x]                 // parameter: OUTCOME: ON or OFF depending on desired state of button
    CLICKHELPON, //[x]               // parameters: GAMEOBJECT: GameObject clicked on, ELEMENT: code string used for translation purposes

    // tutorial functionality
    CLICKNEXT, //[x]
    CLICKFOCUSHOLE, //[x]
    
    // tiles
    CLICKTILE, //[x]          // parameter: GAMEOBJECT that specifies tile's GameObject name; parameter: HELPMODE: true if Help button was active

    // towers
    CLICKTOWER, //[x]         // parameters: GAMEOBJECT: tower's GameObject name
    CLICKTOWERSELL, //[x]     // parameters: GAMEOBJECT: tower's GameObject name; COST: funds earned back from selling
    CLICKTOWERUPGRADE, //[x]  // parameters: GAMEOBJECT: tower's GameObject name; COST: cost of upgrade
    CLICKTOWERBUTTON, //[x]   // parameters: ELEMENT: tower's Blueprint name; OUTCOME: final state of button ON or OFF; COST: cost of construction
    CLICKTOWERBUILD, //[x]    // parameters: ELEMENT/GAMEOBJECT: tower's Blueprint or tower built depending on outcome; OUTCOME: SUCCESS/FAILURE (if not enough funds)

    // pathogens - pathogen context: life points, resistance level, pathogen name, position on map
    PATHOGENESCAPES, //[x]    // parameter: GAMEOBJECT pathogen
    PATHOGENKILLEDBYWBC, //[x]  // a pathogen was swallowed by a white blood cell; parameter: VIRUS or BACTERIUM: pathogen - either an Enemy (bacterium) or a Virus
    PATHOGENKILLEDBYAB, //[x]   // killed pathogen with antibiotics; parameter: GAMEOBJECT: pathogen
    PATHOGENDIVIDES, //[x]
    PATHOGENSPAWNS, //[x]
    PATHOGENINBLOOD, //[x]

    // WBCs
    WBCSPAWNS, //[x]
    WBCLEAVES, //[x]

    COMPLETEGAME, //[x]       // successfully finished the game; parameters: TIMESINCEGAMELOADED, TIMEGAMEPLAYEDNOPAUSE, TIMESINCELEVELLOADED
    NEWFURTHEST, //[x]        // reached a new furthest level; parameter: GAMELEVEL: level completed
    NEWOWNRECORD, //[ ]       // beat own best completion time on a level - Feature not developed yet

    TUTORIALIMAGE, //[x]      // a hint message was displayed in front of a grey background; parameters: MESSAGE: the text, ELEMENT: the image
    TUTORIALFOCUSON, //[x]    // a tutorial message was displayed with focus arrow and mask; parameters: MESSAGE: the text, GAMEOBJECT: the GameObject the system focuses on

}
