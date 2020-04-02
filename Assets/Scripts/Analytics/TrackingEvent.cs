using UnityEngine;

//TODO: set version number
public enum TrackingEvent
{
    /////////////////////////////////////////////////////////
    // standard events
    DEFAULT, //[ ]
    CREATEPLAYER, //[x]
    START, //[x]              // launched the game
    END, //[x]                // quit the game

    /////////////////////////////////////////////////////////
    // specific events
    /////////////////////////////////////////////////////////
    
    /////////////////////////////////////////////////////////
    // all screens
    CLICKLANGUAGE, //[ ]          // using the UI button
    // debug
    PRESSLANGUAGE, //[ ]          // using the keyboard shortcut
    PRESSCURRENCYCHEAT, //[ ]     // using the keyboard shortcut
    PRESSWINCHEAT, //[ ]          // using the keyboard shortcut
    PRESSLOSECHEAT, //[ ]         // using the keyboard shortcut
    CLICKRESET, //[ ]
    CLICKUNLOCK, //[ ]
    CUSTOMDEBUG, //[ ]            // to send manual custom messages

    /////////////////////////////////////////////////////////
    // main screen
    CLICKPLAY, //[x]
    CLICKQUIT, //[x]

    /////////////////////////////////////////////////////////
    // level selection screen
    CLICKLEVEL, //[x]
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
    COMPLETELEVEL, //[x]             // event auto-sent when completing
    CLICKCOMPLETECOMPLETE, //[x]
    CLICKCOMPLETERETRY, //[x]
    CLICKCOMPLETEMENU, //[x]

    // help functionality
    CLICKHELP, //[x]                 // parameter ON or OFF depending on state of game _when called_
    CLICKHELPON, //[x]

    // tutorial functionality
    CLICKNEXT, //[x]
    CLICKFOCUSHOLE, //[x]
    
    // tiles
    CLICKTILE, //[x]          // parameter element that specifies GameObject name

    // towers
    CLICKTOWER, //[x]         
    CLICKTOWERSELL, //[x]     // parameters: element: tower's GameObject name; cost: funds earned back from selling
    CLICKTOWERUPGRADE, //[x]  // parameters: element: tower's GameObject name; cost: cost of upgrade
    CLICKTOWERBUTTON, //[x]   // parameters: element: tower's Blueprint name; cost: cost of construction
    CLICKTOWERBUILD, //[x]    // parameters: element: tower built; outcome: success/failure (if not enough funds)

    // pathogens - pathogen context: life points, resistance level, pathogen name, position on map
    PATHOGENESCAPES, //[ ]
    PATHOGENSWALLOWED, //[ ]
    SHOTPATHOGEN, //[ ]
    PATHOGENDIVIDED, //[ ]
    PATHOGENSPAWNED, //[ ]

    COMPLETEGAME, //[x]       // successfully finished the game
    NEWFURTHEST, //[x]        // reached a new furthest level
    NEWOWNRECORD, //[ ]       // beat own best completion time on a level

    HINT, //[ ]               // a hint message was displayed
    TUTORIAL, //[ ]           // a tutorial message was displayed

    // main menu
    SELECTMENU, //[ ]
    CONFIGURE, //[ ]
    GOTOMOOC, //[ ]
    GOTOSTUDY, //[ ]
    GOTOURL, //[ ]

    // alternative configuration routes
    WEBCONFIGURE, //[ ]
    ADMINCONFIGURE, //[ ]

    // backend events
    SWITCHFROMGAMEVERSION, //[ ]
    SWITCHTOGAMEVERSION  //[ ]
}
