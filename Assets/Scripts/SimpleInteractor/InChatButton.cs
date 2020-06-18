//#define VERBOSEDEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InChatButton : LinkOpener
{
    public const string inChatButtonTag = "InChatButton";
    public const string inChatURLLinkTag = "InChatURLLink";
    public Text label = null;
    public string url = null;
    
    public void click()
    {
        if (string.IsNullOrEmpty(url))
        {
            RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKCHATBUTTON, new CustomData(CustomDataTag.MESSAGE, label.text));

            ChatbotInteractionManager.instance.sendChatMessage(label.text);

            // make all buttons inactive
            GameObject[] buttons = GameObject.FindGameObjectsWithTag(inChatButtonTag);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<Button>().interactable = false;
                buttons[i].tag = "Untagged";
            }
        }
        else
        {
            RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKCHATURL, new CustomData(CustomDataTag.ELEMENT, url));

            // TODO connect to LinkOpener
            #if VERBOSEDEBUG
            Debug.Log("InChatButton opening url " + url);
            #endif
            openLink();
        }
    }
    
    protected override string getURL()
	{
        return url;
	}
}
