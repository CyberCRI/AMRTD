using UnityEngine;

// derives from TrackingEvent
public enum AudioEvent
{
    /*
    /////////////////////////////////////////////////////////
    // standard events
    DEFAULT = 1, //[ ]
    CREATEPLAYER = 2, //[x]
    START = 3, //[x]              // launched the game; parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...
    END = 4, //[x]                // quit the game; parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...

    /////////////////////////////////////////////////////////
    // specific events
    /////////////////////////////////////////////////////////
    
    /////////////////////////////////////////////////////////
    // all screens
    CLICKFEEDBACKFORM = 5, //[x]      // parameter: GAMELEVEL

    // all screens - dev
    // DEVPRESS = using the dev keyboard shortcut
    // DEVCLICK = using the dev UI button
    DEVPRESSLANGUAGE = 6, //[x]          // parameter: LANGUAGE
    DEVPRESSCURRENCY = 7, //[x]          // 
    DEVPRESSLOSE = 8, //[x]              // winLevel();
    DEVPRESSWIN = 9, //[x]               // loseLevel();
    DEVPRESSINJUREALL = 10, //[x]         // injureAllEnemies();
    DEVPRESSKILLALLBUTONE = 11, //[x]     // killAllButOneEnemy();
    DEVPRESSDESTROYALLTURRETS = 12, //[x] // destroyAllTurrets();
    DEVPRESSDELETEPLAYERPREFS = 13, //[x] //
    
    DEVCLICKLEVELSLOCK = 14, //[x]
    DEVPRESSLEVELSLOCK = 15, //[x]
    DEVCLICKLEVELSUNLOCK = 16, //[x]
    DEVPRESSLEVELSUNLOCK = 17, //[x]
    
    DEVCUSTOMDEBUG = 18, //[ ]            // to send manual custom messages
    DEVSWITCHFROMGAMEVERSION = 19, //[x]  // parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...
    DEVSWITCHTOGAMEVERSION = 20, //[x]    // parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...

    // alternative configuration routes - website-side feature not developed yet
    WEBCONFIGURE = 21, //[ ]
    ADMINCONFIGURE = 22, //[ ]

    /////////////////////////////////////////////////////////
    // non-playing screens
    CLICKLANGUAGE = 23, //[x]          // using the UI button; parameter: OPTION language clicked on

    /////////////////////////////////////////////////////////
    // main screen
    CLICKPLAY = 24, //[x]
    CLICKQUIT = 25, //[x]

    /////////////////////////////////////////////////////////
    // level selection screen
    CLICKLEVEL = 26, //[x]             // parameter: OPTION = level clicked on
    CLICKBACK = 27, //[x]

    /////////////////////////////////////////////////////////
    // any level ////////////////////////////////////////////
    /////////////////////////////////////////////////////////
*/
    LEVELSTARTS = 28, //[x]
/*

    // menu screen
    CLICKMENU = 29, //[x]
    CLICKMENURESUME = 30, //[x]
    CLICKMENUMENU = 31, //[x]

    // pause screen
    CLICKPAUSE = 32, //[x]
    CLICKPAUSERESUME = 33, //[x]          // deprecated since 0.33 included

    // retry screen
    CLICKRETRY = 34, //[x]
    CLICKRETRYRESUME = 35, //[x]
    CLICKRETRYRETRY = 36, //[x]           // parameters: cf. getLevelEndContext

*/

    // complete screen
    COMPLETELEVEL = 37, //[x]             // event auto-sent when completing; parameters: cf. getLevelEndContext
/*
    CLICKCOMPLETECOMPLETE = 38, //[x]
    CLICKCOMPLETERETRY = 39, //[x]
    CLICKCOMPLETEMENU = 40, //[x]

*/
    // game over screen
    GAMEOVER = 41, //[x]                  // event auto-sent when losing a level; parameters: cf. getLevelEndContext
/*
    CLICKGAMEOVERRETRY = 42, //[x]
    CLICKGAMEOVERMENU = 43, //[x]

    // help functionality
    CLICKHELP = 44, //[x]                 // parameter: OUTCOME: ON or OFF depending on desired state of button
    */
    CLICKHELPON = 45, //[x]               // parameters: GAMEOBJECT: GameObject clicked on, ELEMENT: code string used for translation purposes
    /*

    // chatbot
    */
    CLICKCHATBOT = 46, //[x]              // parameter: OUTCOME: ON or OFF depending on desired state of button
    CHATBOTGETMESSAGE = 47, //[ ]
    CHATBOTSENDMESSAGE = 48, //[ ]
    CLICKCHATURL = 49, //[ ]
    /*
    CLICKCHATBUTTON = 50, //[x]

    // fast forward functionality
    CLICKFASTFORWARD = 51, //[x]          // parameter: OUTCOME: ON or OFF depending on desired speed of game

    // tutorial functionality
    CLICKNEXT = 52, //[x]
    CLICKFOCUSHOLE = 53, //[x]
    
    // tiles
    CLICKTILE = 54, //[x]          // parameter: GAMEOBJECT that specifies tile's GameObject name; parameter: HELPMODE: true if Help button was active

    // towers
    */
    CLICKTOWER = 55, //[x]         // parameters: GAMEOBJECT: tower's GameObject name
    CLICKTOWERSELL = 56, //[x]     // parameters: GAMEOBJECT: tower's GameObject name; COST: funds earned back from selling
    CLICKTOWERUPGRADE = 57, //[x]  // parameters: GAMEOBJECT: tower's GameObject name; COST: cost of upgrade
    CLICKTOWERBUTTON = 58, //[x]   // parameters: ELEMENT: tower's Blueprint name; OUTCOME: final state of button ON or OFF; COST: cost of construction
    CLICKTOWERBUILD = 59, //[x]    // parameters: ELEMENT/GAMEOBJECT: tower's Blueprint or tower built depending on outcome; OUTCOME: SUCCESS/FAILURE (if not enough funds)
    /*

    // pathogens - pathogen context: life points, resistance level, pathogen name, position on map
    */
    PATHOGENESCAPES = 60, //[x]      // parameter: GAMEOBJECT pathogen
    PATHOGENKILLEDBYWBC = 61, //[x]  // a pathogen was swallowed by a white blood cell; parameter: VIRUS or BACTERIUM: pathogen - either an Enemy (bacterium) or a Virus
    PATHOGENKILLEDBYAB = 62, //[x]   // killed pathogen with antibiotics; parameter: GAMEOBJECT: pathogen
    PATHOGENDIVIDES = 63, //[x]      // controlled by VERBOSEMETRICSLVL2 to avoid overburden of RedMetrics.io
    PATHOGENSPAWNS = 64, //[ ]       // controlled by VERBOSEMETRICSLVL2 to avoid overburden of RedMetrics.io
    PATHOGENINBLOOD = 65, //[x]
    /*

    // WBCs
    WBCSPAWNS = 66, //[x]            // controlled by VERBOSEMETRICSLVL2 to avoid overburden of RedMetrics.io
    WBCLEAVES = 67, //[x]            // controlled by VERBOSEMETRICSLVL2 to avoid overburden of RedMetrics.io

    COMPLETEGAME = 68, //[x]       // successfully finished the game; parameters: TIMESINCEGAMELOADED, TIMEGAMEPLAYEDNOPAUSE, TIMESINCELEVELLOADED
    NEWFURTHEST = 69, //[x]        // reached a new furthest level; parameter: GAMELEVEL: level completed
    NEWOWNRECORD = 70, //[ ]       // beat own best completion time on a level - Feature not developed yet
*/
    TUTORIALIMAGE = 71, //[x]      // a hint message was displayed in front of a grey background; parameters: MESSAGE: the text, ELEMENT: the image
    TUTORIALFOCUSON = 72, //[x]    // a tutorial message was displayed with focus arrow and mask; parameters: MESSAGE: the text, GAMEOBJECT: the GameObject the system focuses on

    // events specific to audio //////////////////////////////////////////
    PATHOGENHITBYBULLET = 73, //[x]
    PATHOGENHITBYBLAST = 74, //[x]
    PATHOGENHITBYLASER = 75, //[x]
    PATHOGENDEFLECTS = 76, //[x]
    CLICKUI = 78, //[x]
    //////////////////////////////////////////////////////////////////////

    SETVOLUMESOUND = 79, //[ ]     // parameter: float VALUE
    SETVOLUMEMUSIC = 80, //[ ]     // parameter: float VALUE
}
