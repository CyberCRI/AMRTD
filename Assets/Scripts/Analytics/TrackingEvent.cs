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
    // all screens - debug
    PRESSLANGUAGE, //[ ]          // using the keyboard shortcut
    PRESSCURRENCYCHEAT, //[ ]     // using the keyboard shortcut
    PRESSWINCHEAT, //[ ]          // using the keyboard shortcut
    PRESSLOSECHEAT, //[ ]         // using the keyboard shortcut
    CLICKRESET, //[ ]
    CLICKUNLOCK, //[ ]
    CUSTOMDEBUG, //[ ]            // to send manual custom messages
    SWITCHFROMGAMEVERSION, //[x]  // parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...
    SWITCHTOGAMEVERSION, //[x]    // parameters: LOCALPLAYERGUID and PLATFORM: webgl, windowseditor, ...
    // alternative configuration routes
    WEBCONFIGURE, //[ ]
    ADMINCONFIGURE, //[ ]

    /////////////////////////////////////////////////////////
    // non-playing screens
    CLICKLANGUAGE, //[ ]          // using the UI button

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
    CLICKRETRYRETRY, //[x]

    // complete screen
    COMPLETELEVEL, //[x]             // event auto-sent when completing; parameter: GAMELEVEL completed
    CLICKCOMPLETECOMPLETE, //[x]
    CLICKCOMPLETERETRY, //[x]
    CLICKCOMPLETEMENU, //[x]

    // game over screen
    GAMEOVER, //[x]                  // event auto-sent when losing a level; parameters: GAMELEVEL failed, WAVES survived
    CLICKGAMEOVERRETRY, //[x]
    CLICKGAMEOVERMENU, //[x]

    // help functionality
    CLICKHELP, //[x]                 // parameter: OUTCOME: ON or OFF depending on desired state of button
    CLICKHELPON, //[x]               // parameters: GAMEOBJECT: GameObject clicked on, ELEMENT: code string used for translation purposes

    // tutorial functionality
    CLICKNEXT, //[x]
    CLICKFOCUSHOLE, //[x]
    
    // tiles
    CLICKTILE, //[x]          // parameter: GAMEOBJECT that specifies tile's GameObject name

    // towers
    CLICKTOWER, //[x]         // parameters: GAMEOBJECT: tower's GameObject name
    CLICKTOWERSELL, //[x]     // parameters: GAMEOBJECT: tower's GameObject name; COST: funds earned back from selling
    CLICKTOWERUPGRADE, //[x]  // parameters: GAMEOBJECT: tower's GameObject name; COST: cost of upgrade
    CLICKTOWERBUTTON, //[x]   // parameters: GAMEOBJECT: tower's Blueprint name; COST: cost of construction
    CLICKTOWERBUILD, //[x]    // parameters: GAMEOBJECT: tower built; OUTCOME: SUCCESS/FAILURE (if not enough funds)

    // pathogens - pathogen context: life points, resistance level, pathogen name, position on map
    PATHOGENESCAPES, //[x]    // parameter: GAMEOBJECT pathogen
    PATHOGENSWALLOWED, //[x]  // a pathogen was swallowed by a white blood cell; parameter: GAMEOBJECT: pathogen
    KILLEDPATHOGENAB, //[x]   // killed pathogen with antibiotics; parameter: GAMEOBJECT: pathogen
    PATHOGENDIVIDED, //[x]
    PATHOGENSPAWNED, //[x]
    PATHOGENINBLOOD, //[x]

    COMPLETEGAME, //[x]       // successfully finished the game
    NEWFURTHEST, //[x]        // reached a new furthest level; parameter: GAMELEVEL: level completed
    NEWOWNRECORD, //[ ]       // beat own best completion time on a level - Feature not developed yet

    TUTORIALIMAGE, //[x]      // a hint message was displayed in front of a grey background; parameters: MESSAGE: the text, ELEMENT: the image
    TUTORIALFOCUSON, //[x]    // a tutorial message was displayed with focus arrow and mask; parameters: MESSAGE: the text, GAMEOBJECT: the GameObject the system focuses on

}
