#define VERBOSEDEBUG
#define VERBOSEMETRICS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using LitJson;
using System;

// author 
//    Raphael Goujet
//    Center for Research and Interdisciplinarity
//    raphael.goujet@cri-paris.org
public class RedMetricsManager : MonoBehaviour
{

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "RedMetricsManager";
    public static RedMetricsManager instance;

    void Awake()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " Awake with gameVersionGuid=" + gameVersionGuid);
#endif
        if (null == instance)
        {
            instance = this;
            initializeIfNecessary();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " OnDestroy " + (instance == this));
#endif
        if (this == instance)
        {
            instance = null;
        }
    }

    // does not work on iOS, Windows Store Apps and Windows Phone 8.1
    // see details on https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html
    void OnApplicationQuit()
    {
        CustomData guidCD = generateCustomDataForGuidInit();
        sendEvent(TrackingEvent.END, guidCD);
    }

    private bool initialized = false;
    private void initializeIfNecessary()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " initializeIfNecessary initialized=" + initialized);
#endif
        if (!initialized)
        {
            DontDestroyOnLoad(instance.gameObject);
            initialized = true;
        }
    }

#if VERBOSEDEBUG
    void Start()
    {
        Debug.Log(this.GetType() + " Start with gameVersionGuid=" + gameVersionGuid);
    }
#endif
    ////////////////////////////////////////////////////////////////////////////////////////////

    //TODO interface to automatize data extraction for data gathering through sendEvent

    // redmetrics.io
    // private const string redMetricsURL = "https://api.redmetrics.io/v1/";
    // formerly http://redmetrics.crigamelab.org/ and http://api.redmetrics.crigamelab.org/v1/
    private const string redMetricsURL = "http://api.redmetrics.io/v1/";
    private const string redMetricsPlayer = "player";
    private const string redMetricsEvent = "event";

    // AMRTD test game version
    private const string defaultGameVersion = "b5ba9b46-9c6d-43d6-8611-3b2af6afff48";
    private static System.Guid defaultGameVersionGuid = new System.Guid(defaultGameVersion);
    private System.Guid gameVersionGuid = new System.Guid(defaultGameVersionGuid.ToByteArray());

    private const string defaultGameSession = "b5ba9b46-9c6d-43d6-8611-3b2af6afff47";
    private static System.Guid defaultGameSessionGUID = new System.Guid(defaultGameSession);
    private System.Guid gameSessionGUID = new System.Guid(defaultGameSessionGUID.ToByteArray());

    private string _localPlayerGUID; //player guid stored on local computer, in PlayerPrefs
    public string localPlayerGUID
    {
        set
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " localPlayerGUID_set " + value);
#endif
            _localPlayerGUID = value;
        }
    }
    private string globalPlayerGUID; //TODO login system

    private bool isGameSessionGUIDCreated = false;
    private bool _isStartEventSent = false;
    public bool isStartEventSent
    {
        get
        {
            return _isStartEventSent;
        }
    }

    // list of events to be stacked while the player guid is not created yet, ie rmConnect's callback has not been called yet and isGameSessionGUIDCreated is still false
    private LinkedList<TrackingEventDataWithoutIDs> waitingList = new LinkedList<TrackingEventDataWithoutIDs>();

    public void setGameSessionGUID(string _gameSessionGUID)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setGameSessionGUID " + _gameSessionGUID);
#endif
        gameSessionGUID = new System.Guid(_gameSessionGUID);
    }

    public void setGlobalPlayerGUID(string _globalPlayerGUID)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setGlobalPlayerGUID " + globalPlayerGUID);
#endif
        globalPlayerGUID = _globalPlayerGUID;
    }

    public void setGameVersion(System.Guid _gameVersionGuid)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setGameVersion Guid " + gameVersionGuid + " to " + _gameVersionGuid);
#endif
        gameVersionGuid = _gameVersionGuid;
    }

    public Guid getGameVersion()
    {
        return gameVersionGuid;
    }

    public bool isGameVersionInitialized()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " isGameVersionInitialized");
#endif
        return defaultGameVersionGuid != gameVersionGuid;
    }

    public IEnumerator GET(string url, System.Action<WWW> callback)
    {
#if VERBOSEDEBUG
        Debug.Log("RedMetricsManager GET");
#endif
        WWW www = new WWW(url);
        return waitForWWW(www, callback);
    }

    // unused
    public IEnumerator POST(string url, Dictionary<string, string> post, System.Action<WWW> callback)
    {
#if VERBOSEDEBUG
        Debug.Log("RedMetricsManager POST");
#endif
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }

        WWW www = new WWW(url, form);
        return waitForWWW(www, callback);
    }

    public IEnumerator POST(string url, byte[] post, Dictionary<string, string> headers, System.Action<WWW> callback)
    {
#if VERBOSEDEBUG
        Debug.Log("RedMetricsManager POST url: " + url);
#endif
        WWW www = new WWW(url, post, headers);
        return waitForWWW(www, callback);
    }

    private IEnumerator waitForWWW(WWW www, System.Action<WWW> callback)
    {
#if VERBOSEDEBUG
        Debug.Log("RedMetricsManager waitForWWW");
#endif
        float elapsedTime = 0.0f;

        if (null == www)
        {
            Debug.LogError("RedMetricsManager waitForWWW: null www");
            yield return null;
        }

        while (!www.isDone)
        {
            elapsedTime += Time.unscaledDeltaTime;
            if (elapsedTime >= 30.0f)
            {
                Debug.LogError("RedMetricsManager waitForWWW: TimeOut!");
                break;
            }
            yield return null;
        }

        if (!www.isDone || !string.IsNullOrEmpty(www.error))
        {
            string errmsg = string.IsNullOrEmpty(www.error) ? "timeout" : www.error;
            Debug.LogError(string.Format("RedMetricsManager waitForWWW Error: Load Failed: {0}", errmsg));
            callback(null);    // Pass null result.
            yield break;
        }

#if VERBOSEDEBUG
        Debug.Log("RedMetricsManager waitForWWW: message successfully transmitted");
#endif
        callback(www); // Pass retrieved result.
    }

    private void sendData(string urlSuffix, string pDataString, System.Action<WWW> callback)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " sendData urlSuffix=" + urlSuffix + " pDataString=" + pDataString);
#endif
        string url = redMetricsURL + urlSuffix;
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        byte[] pData = System.Text.Encoding.ASCII.GetBytes(pDataString.ToCharArray());
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " sendData StartCoroutine POST with data=" + pDataString);
#endif
        StartCoroutine(POST(url, pData, headers, callback));
    }

    private void createPlayer(System.Action<WWW> callback)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " createPlayer");
#endif
        CreatePlayerData data = new CreatePlayerData();
        string json = getJsonString(data);
        sendData(redMetricsPlayer, json, callback);
    }

    private void testGet(System.Action<WWW> callback)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " testGet");
#endif
        string url = redMetricsURL + redMetricsPlayer;
        StartCoroutine(GET(url, callback));
    }

    private void wwwLogger(WWW www, string origin = "default")
    {
        if (null == www)
        {
            Debug.LogError(string.Format("{0} wwwLogger Error: {1}: {2}", this.GetType(), origin, "Null == www"));
        }
        else
        {
            if (www.error == null)
            {
#if VERBOSEDEBUG
                Debug.Log(string.Format("{0} wwwLogger Success: {1}: {2}", this.GetType(), origin, www.text));
#endif
            }
            else
            {
                Debug.LogError(string.Format("{0} wwwLogger Error: {1}: {2}", this.GetType(), origin, www.error));
            }
        }
    }

    private string extractPID(WWW www)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " extractPID");
#endif
        string result = null;
        wwwLogger(www, "extractPID");
        if (www != null && www.text != null)
        {
            string trimmed = www.text.Trim();
            string[] split1 = trimmed.Split('\n');
            foreach (string s1 in split1)
            {
#if VERBOSEDEBUG
                Debug.Log(this.GetType() + " extractPID: '" + s1 + "'");
#endif
                if (s1.Length > 5)
                {
                    string[] split2 = s1.Trim().Split(':');
                    foreach (string s2 in split2)
                    {
                        if (!s2.Equals("\"id\"") && !string.IsNullOrEmpty(s2))
                        {
                            string[] split3 = s2.Trim().Split('"');
                            foreach (string s3 in split3)
                            {

                                if (!s3.Equals("\"") && !string.IsNullOrEmpty(s3))
                                {
                                    result = s3;
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }

    private void trackStart(WWW www)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " trackStart: www =? null:" + (null == www));
#endif
        string pID = extractPID(www);
        if (null != pID)
        {
            setGameSessionGUID(pID);
            sendStartEventWithPlayerGUID();
        }
    }

    private void sendStartEventWithPlayerGUID()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " sendStartEventWithPlayerGUID with string.IsNullOrEmpty(_localPlayerGUID)=" + string.IsNullOrEmpty(_localPlayerGUID));
#endif
        if (string.IsNullOrEmpty(_localPlayerGUID))
        {
            sendEvent(TrackingEvent.START);
        }
        else
        {
            CustomData guidCD = generateCustomDataForGuidInit();
            sendEvent(TrackingEvent.START, guidCD);
        }
    }

    public CustomData generateCustomDataForGuidInit()
    {
        CustomData guidCD = CustomData.getContext(
                        new CustomDataTag[4]{
                            CustomDataTag.LOCALPLAYERGUID,
                            CustomDataTag.PLATFORM,
                            CustomDataTag.RESOLUTION, // assumes it won't be changed
                            CustomDataTag.LANGUAGE,
                            }
        );
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " generated guidCD=" + guidCD);
#endif
        return guidCD;
    }

    //////////////////////////////////////////////////
    // Should be called only after localPlayerGUID is set
    public void sendStartEvent(bool force = false)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " sendStartEvent");
#endif
        if (!_isStartEventSent || force)
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " sendStartEvent !isStartEventSent");
#endif
            // gameSessionGUID hasn't been initialized
            createPlayer(www => trackStart(www));
            _isStartEventSent = true;
        }
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " sendStartEvent done");
#endif
    }

    // TODO: store events that can't be sent, during internet outage for instance
    private void addEventToSendLater(TrackingEventDataWithoutIDs data)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " addEventToSendLater " + data);
#endif
        waitingList.AddLast(data);
    }

    private void executeAndClearAllWaitingEvents()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " executeAndClearAllWaitingEvents");
#endif
        foreach (TrackingEventDataWithoutIDs data in waitingList)
        {
            sendEvent(data);
        }
        waitingList.Clear();
    }

    private void resetConnectionVariables()
    {
        isGameSessionGUIDCreated = false;
    }

    public string getJsonString(object obj)
    {
        string dataAsJson = JsonUtility.ToJson(obj);
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " getJsonString(" + obj + ")=" + dataAsJson);
#endif
        return dataAsJson;
    }

    public void sendRichEvent(TrackingEvent trackingEvent, CustomData customData = null, string userTime = null)
    {
#if VERBOSEDEBUG
        string customDataString = null == customData ? "" : ", " + customData;
        Debug.Log(this.GetType() + " sendRichEvent(" + trackingEvent + customDataString);
#endif

        CustomData context = CustomData.getEventContext();
        if (customData != null)
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " merging from trackingEvent " + trackingEvent);
#endif
            context.merge(customData);
        }
        sendEvent(trackingEvent, context, userTime);
    }

    public void sendEvent(TrackingEvent trackingEvent, CustomData customData = null, string userTime = null)
    {
#if VERBOSEDEBUG
        Debug.Log(string.Format("{0} sendEvent({1}, {2}, {3})", this.GetType(), trackingEvent, customData, userTime));
#endif
        TrackingEventDataWithIDs data = new TrackingEventDataWithIDs(gameSessionGUID, gameVersionGuid, trackingEvent, customData);
        sendEvent(data);
    }

    public void sendEvent(TrackingEventDataWithoutIDs data)
    {
#if VERBOSEDEBUG
        Debug.Log(string.Format("{0} sendEvent({1})", this.GetType(), data));
#endif
        sendEvent(new TrackingEventDataWithIDs(gameSessionGUID, gameVersionGuid, data));
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // paradigm: pragmatically, case-by-case sends action performed, action planned, state of game, or desired state of game

    public void sendEvent(TrackingEventDataWithIDs data)
    {
#if VERBOSEDEBUG
        Debug.Log(string.Format("{0} sendEvent({1})", this.GetType(), data));
#endif
        // test Application.internetReachability

        // // TODO: queue events that can't be sent during internet outage
        // TrackingEventDataWithoutIDs data = new TrackingEventDataWithoutIDs(trackingEvent, customData, userTime);
        // addEventToSendLater(data);
#if VERBOSEMETRICS
        CustomData context = CustomData.getEventContext();
#if VERBOSEDEBUG
        Debug.Log(string.Format("{0} sendEvent merging context {1} into trackingEvent {2}", this.GetType(), context, data));
#endif
        data.mergeCustomData(context);
#endif

        string json = getJsonString(data);
#if VERBOSEDEBUG
        Debug.Log(
            string.Format(
                this.GetType() + " sendEvent - _localPlayerGUID={0}, gameSessionGUID={1}, gameVersionGuid={2}, json={3}",
                _localPlayerGUID, gameSessionGUID, gameVersionGuid, json
                )
            );
#endif
        sendData(redMetricsEvent, json, value => wwwLogger(value, "sendEvent(" + data.type + ")"));
        //TODO pass data as parameter to sendDataStandalone so that it's serialized inside
    }

    public override string ToString()
    {
        return string.Format("[RedMetricsManager gameSessionGUID:{0}, gameVersionGuid:{1}, redMetricsURL:{2}]",
                          gameSessionGUID, gameVersionGuid, redMetricsURL);
    }

}
