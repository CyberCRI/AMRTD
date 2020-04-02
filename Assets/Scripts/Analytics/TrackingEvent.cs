using UnityEngine;

//TODO: set version number
public enum TrackingEvent
{
    // standard events
    DEFAULT, //[ ]
    CREATEPLAYER, //[x]
    START, //[x]              // launched the game
    END, //[x]                // quit the game

    // specific events
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

    // main screen
    CLICKPLAY, //[x]
    CLICKQUIT, //[x]

    // level selection screen
    CLICKLEVEL, //[x]
    CLICKBACK, //[x]

    // any level
    CLICKMENU, //[x]
    CLICKMENURESUME, //[x]
    CLICKMENUMENU, //[x]

    CLICKPAUSE, //[x]
    CLICKPAUSERESUME, //[x]

    CLICKRETRY, //[x]
    CLICKRETRYRESUME, //[x]
    CLICKRETRYRETRY, //[x]

    COMPLETELEVEL, //[x]             // event auto-sent when completing
    CLICKCOMPLETECOMPLETE, //[x]
    CLICKCOMPLETERETRY, //[x]
    CLICKCOMPLETEMENU, //[x]

    CLICKHELP, //[x]                 // parameter ON or OFF depending on state of game _when called_
    CLICKHELPON, //[x]

    CLICKNEXT, //[ ]
    CLICKFOCUSHOLE, //[ ]
    
    CLICKTILE, //[ ]

    CLICKTOWER, //[ ]
    CLICKTOWERSELL, //[ ]
    CLICKTOWERUPGRADE, //[ ]
    CLICKTOWERBUTTON, //[ ]
    CLICKTOWERBUILD, //[ ]

    COMPLETEGAME, //[ ]       // successfully finished the game
    REACH, //[ ]              // reached a new level
    NEWFURTHEST, //[ ]        // reached a new furthest level
    NEWOWNRECORD, //[ ]       // beat own best completion time on a level
    NEWWORLDRECORD, //[ ]     // beat world best completion time on a level
    SWITCH, //[ ]             // changed game level

    HINT, //[ ]               // a hint message was displayed

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
