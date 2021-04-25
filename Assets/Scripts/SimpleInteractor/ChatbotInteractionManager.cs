//#define VERBOSEDEBUG
//#define TRACKSENTMESSAGES
#define TRACKCENSOREDSENTMESSAGES
//#define LOGWWW
//#define LOGINPUTS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ChatbotInteractionManager : MonoBehaviour
{
    public static ChatbotInteractionManager instance = null;
    
    [SerializeField]
    private InputField chatbotInputField = null;
    [SerializeField]
    private InputField chatbotURLInputField = null;
    [SerializeField]
    private InputField senderIdInputField = null;
    [SerializeField]
    private RectTransform chatListRoot = null;
    [SerializeField]
    private GameObject chatbotMessagePrefab = null;
    [SerializeField]
    private GameObject playerMessagePrefab = null;
    [SerializeField]
    private GameObject genericInChatButton = null;

    private System.Guid _senderGUID = System.Guid.NewGuid();
    private string _senderId = "";
    
    public const string chatbotURL = "http://arya.cri-paris.tech:5005/webhooks/rest/webhook";

    // url regex
    private const string startBoundary = @"(?<prefix>(\(|<br/>|\n| )*<a.*?href="")";
    private const string endBoundary = @"(?<suffix>""(target=""_blank""| )*>.*?Link.*?</a>(\)|,|<br/>|\n| )*)";
    private const string regexPattern = startBoundary + ".+?" + endBoundary;
    private List<string> extractedUrls = new List<string>();
    
    private string[] pathogensStrings = new string[17] {
        "acinetobacter baumannii (ab)",
        "pseudomonas aeruginosa",
        "enterobacteriaceae",
        "enterococcus faecium",
        "helicobacter pylori",
        "campylobacter spp",
        "salmonellae",
        "neisseria gonorrhoeae",
        "streptococcus pneumoniae",
        "haemophilus influenzae",
        "shigella spp",
        "mycobacterium tuberculosis (mtb)",
        "escherichia coli",
        "klebsiella pneumoniae",
        "vibrio cholerae",
        "staphylococcus aureus",
        "yersinia pestis",
    };

    [Serializable]
    public class JSONMessage
    {
        public string message = "";
        public string senderId = "";

        public JSONMessage(string __message, string __senderId)
        {
            message = __message;
            senderId = __senderId;
        }

        public string getJSON()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class JSONAnswers
    {
        public JSONAnswer[] answers;
    }

    [Serializable]
    public class JSONAnswer
    {
        public string text = "";
        public string recipient_id = "";
        public ChatbotButton[] buttons = null;
    }

    [Serializable]
    public class ChatbotButton
    {
        public string payload = "";
        public string title = "";

        public InChatButton generateButton()
        {
            #if VERBOSEDEBUG
            Debug.Log("ChatbotButton generateButton title=" + title + "; payload: [" + payload + "]");
            #endif
            GameObject icbGO = Instantiate(ChatbotInteractionManager.instance.genericInChatButton);
            InChatButton icb = icbGO.GetComponent<InChatButton>();
            icb.label.text = string.IsNullOrEmpty(title) ? 
                ChatbotInteractionManager.instance.pathogensStrings[UnityEngine.Random.Range(0, ChatbotInteractionManager.instance.pathogensStrings.Length)]
                : title;
            return icb;
        }
    }
    
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        chatbotURLInputField.text = chatbotURL;
        generateSenderId();
        senderIdInputField.text = _senderId;
        chatbotInputField.ActivateInputField();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            #if LOGINPUTS || VERBOSEDEBUG
            Debug.Log("Update GetKeyDown Return/Enter: text=" + chatbotInputField.text + "; senderId=" + _senderId);
            #endif
            StartCoroutine(unityDirectSend());            
        }
    }

    private InChatButton generateURLButton(string url, int index)
    {
        #if LOGINPUTS || VERBOSEDEBUG
        Debug.Log("setUpURLButton generateButton url=" + url);
        #endif
        GameObject icbGO = Instantiate(ChatbotInteractionManager.instance.genericInChatButton);
        InChatButton icb = icbGO.GetComponent<InChatButton>();
        icb.label.text = "Link" + index;
        icb.url = url;
        icbGO.tag = InChatButton.inChatURLLinkTag;
        return icb;
    }

    private string extractUrls(string input)
    {
        #if LOGINPUTS || VERBOSEDEBUG
		Debug.Log("extractUrls: input=" + input);
        #endif
        extractedUrls.Clear();
        return Regex.Replace(input, regexPattern, replacer);
    }

    private static string replacer(Match match)
    {
        #if LOGINPUTS || VERBOSEDEBUG
		Debug.Log("replacer: " + match.Value);
        #endif
        string url = match.Value.Substring(match.Groups["prefix"].Value.Length, match.Value.Length - match.Groups["prefix"].Value.Length - match.Groups["suffix"].Value.Length);
        #if VERBOSEDEBUG
		Debug.Log("url= " + url);
        #endif
        instance.extractedUrls.Add(url);
        return "";
    }

    private JSONAnswers getAnswersFromJsonString(string _jsonInput)
    {
        JSONAnswers answers = JsonUtility.FromJson<JSONAnswers>("{\"answers\":" + _jsonInput + "}");
        #if LOGWWW
        foreach(JSONAnswer answer in answers.answers)
        {
            Debug.Log("Answer: " + answer.text);
        }
        #endif
        return answers;
    }

    void generateSenderId()
    {
        _senderGUID = System.Guid.NewGuid();
        _senderId = _senderGUID.ToString("N");
    }

    public void clickUnityDirectSend()
    {
        #if LOGINPUTS || VERBOSEDEBUG
        Debug.Log("clickUnityDirectSend: text=" + chatbotInputField.text + "; senderId=" + senderIdInputField.text);
        #endif
        StartCoroutine(unityDirectSend());
    }

    // dev tools /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void clickUnityDirectSendHCTS1()
    {
        #if LOGINPUTS || VERBOSEDEBUG
        Debug.Log("clickUnityDirectSendHCTS1");
        #endif
        StartCoroutine(unityDirectSend(hardcodedTestString1));
    } 

    public void clickUnityDirectSendHCTS2() { StartCoroutine(unityDirectSend(hardcodedTestString2)); }
    public void clickUnityDirectSendHCTS3() { StartCoroutine(unityDirectSend(hardcodedTestString3)); }
    public void clickUnityDirectSendHCTS4() { StartCoroutine(unityDirectSend(hardcodedTestString4)); }
    public void clickUnityDirectSendHCTS5() { StartCoroutine(unityDirectSend(hardcodedTestString5)); }
    public void clickUnityDirectSendHCTS6() { StartCoroutine(unityDirectSend(hardcodedTestString6)); }

    //private const string hardcodedTestString1 = "[{\"recipient_id\":\"default\",\"text\":\"Very good!\"},{\"recipient_id\":\"default\",\"text\":\"Here is the list of bacterium species I can help you with.\",\"buttons\":[{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"acinetobacter baumannii (ab)\\\"}\",\"title\":\"acinetobacter baumannii (ab)\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"pseudomonas aeruginosa\\\"}\",\"title\":\"pseudomonas aeruginosa\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"enterobacteriaceae\\\"}\",\"title\":\"enterobacteriaceae\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"enterococcus faecium\\\"}\",\"title\":\"enterococcus faecium\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"helicobacter pylori\\\"}\",\"title\":\"helicobacter pylori\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"campylobacter spp\\\"}\",\"title\":\"campylobacter spp\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"salmonellae\\\"}\",\"title\":\"salmonellae\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"neisseria gonorrhoeae\\\"}\",\"title\":\"neisseria gonorrhoeae\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"streptococcus pneumoniae\\\"}\",\"title\":\"streptococcus pneumoniae\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"haemophilus influenzae\\\"}\",\"title\":\"haemophilus influenzae\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"shigella spp\\\"}\",\"title\":\"shigella spp\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"mycobacterium tuberculosis (mtb)\\\"}\",\"title\":\"mycobacterium tuberculosis (mtb)\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"escherichia coli\\\"}\",\"title\":\"escherichia coli\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"klebsiella pneumoniae\\\"}\",\"title\":\"klebsiella pneumoniae\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"vibrio cholerae\\\"}\",\"title\":\"vibrio cholerae\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"staphylococcus aureus\\\"}\",\"title\":\"staphylococcus aureus\"},{\"payload\":\"\\/set_pathogen{\\\"pathogen\\\": \\\"yersinia pestis\\\"}\",\"title\":\"yersinia pestis\"}]}]";
    private const string hardcodedTestString1 = "[{\"recipient_id\":\"default\",\"text\":\"Are you aware of this recent series of articles in the New Yorker? It depicted how a very widespread, treatable infection now became a huge concern for public health. <br\\/><br\\/>(<a href=\\\"https:\\/\\/www.nytimes.com\\/2019\\/07\\/13\\/health\\/uti-drug-resistant-info.html\\\" target=\\\"_blank\\\">Link<\\/a>),<br\\/><br\\/> (<a href=\\\"https:\\/\\/www.nytimes.com\\/2019\\/07\\/13\\/health\\/urinary-infections-drug-resistant.html\\\" target=\\\"_blank\\\">Link<\\/a>),<br\\/><br\\/> (<a href=\\\"https:\\/\\/www.nytimes.com\\/2019\\/08\\/20\\/science\\/urinary-tract-infections-.html\\\" target=\\\"_blank\\\">Link<\\/a>)\"}]";
    private const string hardcodedTestString2 = "[{\"recipient_id\":\"default\",\"text\":\"Please choose an option below.\",\"buttons\":[{\"payload\":\"\\/inform_beginner_selection{\\\"beginner_selection\\\": \\\"1\\\"}\",\"title\":\"What's AMR\"},{\"payload\":\"\\/inform_beginner_selection{\\\"beginner_selection\\\": \\\"2\\\"}\",\"title\":\"Pathogens causing AMR\"},{\"payload\":\"\\/inform_beginner_selection{\\\"beginner_selection\\\": \\\"3\\\"}\",\"title\":\"About me\"},{\"payload\":\"\\/inform_beginner_selection{\\\"beginner_selection\\\": \\\"4\\\"}\",\"title\":\"None of the above\"}]}]";
    private const string hardcodedTestString3 = "[{\"recipient_id\":\"default\",\"text\":\"I can answer following things related to escherichia coli<br\\/><br\\/> Morphology, Size, Movement, Life cycle, Replication, Metabolism, Classification, Symptoms, Type, Treatment, Treatment duration, Recovery duration,Recovery chances, Resistances, Transmission, Prevention, Incubation, Diagnosis, Priority, Sources, Pictures <br\\/><br\\/>  or you could browse for pathogens\",\"buttons\":[{\"payload\":\"\\/browse_pathogen\",\"title\":\"Browse pathogens\"}]}]";
    private const string hardcodedTestString4 = "[{\"recipient_id\":\"default\",\"text\":\"Escherichia coli looks like this: <a target=\\\"_blank\\\" href=\\\"https:\\/\\/scopeblog.stanford.edu\\/2011\\/06\\/12\\/image-of-the-week-another-look-at-e-coli\\/original-title-0504592b-tif\\/\\\">(Link)<\\/a>\"},{\"recipient_id\":\"default\",\"text\":\"I can answer following things related to escherichia coli<br\\/><br\\/> Morphology, Size, Movement, Life cycle, Replication, Metabolism, Classification, Symptoms, Type, Treatment, Treatment duration, Recovery duration,Recovery chances, Resistances, Transmission, Prevention, Incubation, Diagnosis, Priority, Sources, Pictures <br\\/><br\\/>  or you could browse for pathogens\",\"buttons\":[{\"payload\":\"\\/browse_pathogen\",\"title\":\"Browse pathogens\"}]}]";
    private const string hardcodedTestString5 = "[{\"recipient_id\":\"default\",\"text\":\"That's not correct. But don't worry, <br\\/><br\\/>I'll explain! <br\\/><br\\/>Antibiotics won't work against cold and flu because they are caused by viruses.\"},{\"recipient_id\":\"default\",\"text\":\"It looks like you know a lot. You qualify for the Master badge. Would you like to know more about news in the field or browse pathogens?\",\"buttons\":[{\"payload\":\"\\/news_actions{\\\"read_news\\\": \\\"1\\\"}\",\"title\":\"Read news\"},{\"payload\":\"\\/news_actions{\\\"read_news\\\": \\\"0\\\"}\",\"title\":\"Browse pathogens\"}]}]";
    private const string hardcodedTestString6 = "[{\"recipient_id\":\"default\",\"text\":\"That's not correct. But don't worry, <br\\/><br\\/>I'll explain! <br\\/><br\\/>Antibiotics won't work against cold and flu because they are caused by viruses.\"},{\"recipient_id\":\"default\",\"text\":\"You qualified for the Learner category. Before we go forward, can you tell me if you know about AMR?\",\"buttons\":[{\"payload\":\"\\/know_amr{\\\"has_knowledge\\\": \\\"1\\\"}\",\"title\":\"Yes\"},{\"payload\":\"\\/know_amr{\\\"has_knowledge\\\": \\\"0\\\"}\",\"title\":\"No\"}]}]";

    //public const string jsonInput = "[{\"recipient_id\":\"default\",\"text\":\"I'm sorry, i still learning and currently i can understand only Yes or No OR True or False. I'll improve in the future!!!\"},{\"recipient_id\":\"default\",\"text\":\"Do you know what superbugs are?\"}]";

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void sendChatMessage(string input)
    {
        chatbotInputField.text = input;
        StartCoroutine(unityDirectSend());
    }

    private IEnumerator unityDirectSend(string input = null)
    {
        #if VERBOSEDEBUG
        string parameterString = input == null ? "" : input;
        Debug.Log("unityDirectSend(" + parameterString + ")");
        #endif

        // generate player message in the chat list

        createNewMessage(chatbotInputField.text, true);

        // manage POST

        JSONMessage message = new JSONMessage(chatbotInputField.text, _senderId);

        string jsonToSend = message.getJSON();

        UnityWebRequest uwr = UnityWebRequest.Post(chatbotURLInputField.text, "");
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonToSend));
        uwr.SetRequestHeader("Content-Type", "application/json");
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            #if VERBOSEDEBUG
            Debug.Log("unityDirectSend failure");
            #endif
            Debug.Log(uwr.error);
        }
        else
        {
            #if VERBOSEDEBUG
            Debug.Log("unityDirectSend success");
            #endif

        // manage chatbot answer(s)

            #if LOGWWW
            Dictionary<string,string> answerHeaders = uwr.GetResponseHeaders();
            string answerDebugString = "[";
            foreach (KeyValuePair<string, string> kvp in answerHeaders)
            {
                answerDebugString += (kvp.Key + ": " + kvp.Value + "; ");
            }
            answerDebugString +=  "]";
            Debug.Log("unityDirectSend ResponseDictionary=" + answerDebugString);
            Debug.Log("unityDirectSend DownloadHandler.text=" + uwr.downloadHandler.text);
            #endif

            JSONAnswers answers = null;
            if (string.IsNullOrEmpty(input))
            {
                answers = getAnswersFromJsonString(uwr.downloadHandler.text);
            }
            else // debug route
            {
                #if VERBOSEDEBUG
                Debug.Log("debug route '" + input + "'");
                #endif
                answers = getAnswersFromJsonString(input);
            }
            foreach (JSONAnswer answer in answers.answers)
            {
                string transcripted = answer.text.Replace("<br/>", "\n");
                transcripted = extractUrls(transcripted);

                InChatButton[] replyButtons = null;
                int replyButtonsCount = (null == answer.buttons) ? 0 : answer.buttons.Length;
                if (replyButtonsCount > 0)
                {
                    replyButtons = new InChatButton[replyButtonsCount];
                    for (int i = 0; i < replyButtonsCount; i ++)
                    {
                        replyButtons[i] = answer.buttons[i].generateButton();
                    }
                }

                InChatButton[] urlButtons = null;
                int urlCount = extractedUrls.Count;
                if (urlCount > 0)
                {
                    urlButtons = new InChatButton[urlCount];
                    for (int i = 0; i < urlCount; i ++)
                    {
                        urlButtons[i] = generateURLButton(extractedUrls[i], i+1);
                    }
                    extractedUrls.Clear();
                }
                createNewMessage(transcripted, false, urlButtons, replyButtons);
            }
        }
        
        // refocus the input field
        chatbotInputField.ActivateInputField();

        #if VERBOSEDEBUG
        Debug.Log("unityDirectSend done with json=" + jsonToSend);
        #endif
    }

    private string getMessageRepresentation(
        string text,
        int inputURLButtonCount = 0,
        int inputReplyButtonCount = 0,
        InChatButton[] urlButtons = null,
        InChatButton[] replyButtons = null
        )
    {
        string messageRepresentation = "{text: \"" + text + "\"";
        if (0 < inputURLButtonCount)
        {
            messageRepresentation += ", urls: [";
            for (int i = 0; i < urlButtons.Length ; i++)
            {
                if (i != 0)
                {
                    messageRepresentation += ", ";
                }
                messageRepresentation += "\"" + urlButtons[i].url + "\"";
            }
            messageRepresentation += "]";
        }
        if (0 < inputReplyButtonCount)
        {
            messageRepresentation += ", buttons: [";
            for (int i = 0; i < replyButtons.Length ; i++)
            {
                if (i != 0)
                {
                    messageRepresentation += ", ";
                }
                messageRepresentation += "\"" + replyButtons[i].label.text + "\"";
            }
            messageRepresentation += "]";
        }
        messageRepresentation += "}";
        #if VERBOSEDEBUG
        Debug.Log("messageRepresentation: " + messageRepresentation);
        #endif
        return messageRepresentation;
    }

    private void createNewMessage(
        string text,
        bool isPlayerMessage = false,
        InChatButton[] urlButtons = null,
        InChatButton[] replyButtons = null
        )
    {
        int inputURLButtonCount = urlButtons == null ? 0 : urlButtons.Length;
        int inputReplyButtonCount = replyButtons == null ? 0 : replyButtons.Length;
        
        if (isPlayerMessage)
        {
            #if TRACKSENTMESSAGES || TRACKCENSOREDSENTMESSAGES
            string messageRepresentation = getMessageRepresentation(text, inputURLButtonCount, inputReplyButtonCount, urlButtons, replyButtons);
            #endif

            //#if TRACKSENTMESSAGES
            //RedMetricsManager.instance.sendEvent(TrackingEvent.CHATBOTSENDMESSAGE, new CustomData(CustomDataTag.MESSAGE, messageRepresentation));
            //#elif TRACKCENSOREDSENTMESSAGES
            //RedMetricsManager.instance.sendEvent(TrackingEvent.CHATBOTSENDMESSAGE, new CustomData(CustomDataTag.LENGTH, messageRepresentation.Length));
            //#else
            //RedMetricsManager.instance.sendEvent(TrackingEvent.CHATBOTSENDMESSAGE);
            //#endif
            AudioManager.instance.play(AudioEvent.CHATBOTSENDMESSAGE);
        }
        else
        {
            string messageRepresentation = getMessageRepresentation(text, inputURLButtonCount, inputReplyButtonCount, urlButtons, replyButtons);
            //RedMetricsManager.instance.sendEvent(TrackingEvent.CHATBOTGETMESSAGE, new CustomData(CustomDataTag.MESSAGE, messageRepresentation));
            AudioManager.instance.play(AudioEvent.CHATBOTGETMESSAGE);
        }

        #if LOGINPUTS || VERBOSEDEBUG
        Debug.Log("createNewMessage text=" + text);
        Debug.Log("createNewMessage #urlButtons=" + inputURLButtonCount + ", #replyButtons=" + inputReplyButtonCount);
        #endif

        GameObject messageGO = isPlayerMessage ? Instantiate(playerMessagePrefab) : Instantiate(chatbotMessagePrefab);
        MessagePanel messagePanel = messageGO.GetComponent<MessagePanel>();
        messagePanel.messageText.text = text;
        messageGO.transform.SetParent(chatListRoot, false);

        if (!isPlayerMessage)
        {
            for (int i = 0; i < inputURLButtonCount; i++)
            {
                urlButtons[i].transform.SetParent(messagePanel.buttonList, false);
            }
            for (int i = 0; i < inputReplyButtonCount; i++)
            {
                replyButtons[i].transform.SetParent(messagePanel.buttonList, false);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(chatListRoot);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatListRoot);
    }
}
