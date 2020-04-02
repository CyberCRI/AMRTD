#define VERBOSEDEBUG

using UnityEngine;
using System;

// mostly manages PlayerPrefs
public class GameConfiguration : MonoBehaviour
{
    public static GameConfiguration instance = null;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " Awake");
#endif
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " Awake done");
#endif
    }

    void Start()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " Start");
#endif
        load();
        RedMetricsManager.instance.sendStartEvent();
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " Start done");
#endif
    }

    private const string _localPlayerGUIDPlayerPrefsKey = "localPlayerGUID";

    // language = I18n.Language.English;
    // isAbsoluteWASD = true;
    // isLeftClickToMove = true;

    private static float _baseVolume
        = -1;

    private IntConfigurationParameter _furthestLevelReached
        = new IntConfigurationParameter(0, 0, _furthestLevelReachedKey);
    private FloatConfigurationParameter[] _bestTimes
        = null;

    private void initializeBestTimesIfNecessary()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " initializeBestTimesIfNecessary");
#endif
        if (null == _bestTimes)
        {
            _bestTimes = new FloatConfigurationParameter[LevelSelectionUI.gameLevelCount];
            for (int index = 0; index < _bestTimes.Length; index++)
            {
#if VERBOSEDEBUG
                Debug.Log(this.GetType() + " initializeBestTimes index = " + index);
#endif
                _bestTimes[index] = new FloatConfigurationParameter(Mathf.Infinity, Mathf.Infinity, _bestCompletionTimeLevelStem + index);
                _bestTimes[index].initialize();
            }
        }
    }


#if UNITY_EDITOR
    // if the game is launched in the editor,
    // sets the Game Version GUID to a test GUID 
    // so that events are logged onto a test version
    // instead of the regular game version
    // to prevent data from being contaminated by tests
    private const bool _adminStartValue = true, _adminDefaultValue = true;
#else
    private const bool _adminStartValue = false, _adminDefaultValue = false;
#endif

    private BoolConfigurationParameter _isSoundOn = new BoolConfigurationParameter(true, true, _isSoundOnKey, onSoundChanged);
    public bool isSoundOn
    {
        get
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " getting sound = " + _isSoundOn.val);
#endif
            return _isSoundOn.val;
        }
        set
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " setting sound to " + value);
#endif
            _isSoundOn.val = value;
        }
    }

    private static void onSoundChanged(bool newSoundValue)
    {
#if VERBOSEDEBUG
        Debug.Log("GameConfiguration onSoundChanged");
#endif
        if (_baseVolume < 0)
        {
            if (0 == AudioListener.volume)
            {
                _baseVolume = 1f;
            }
            else
            {
                _baseVolume = AudioListener.volume;
            }
        }
        AudioListener.volume = newSoundValue ? _baseVolume : 0f;
    }

    public int furthestLevel
    {
        get
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " get furthestLevel = " + _furthestLevelReached.val);
#endif
            return _furthestLevelReached.val;
        }
        set
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " set furthestLevel to " + value);
#endif
            // if (value > _furthestLevelReached.val)
            // {
            // Debug.Log(this.GetType() + " setting furthestLevel to " + value);
            _furthestLevelReached.val = value;
            // }
            // else
            // {
            //     Debug.LogWarning(this.GetType() + " tried to update furthestLevel to " + value + " <= " + _furthestLevelReached.val);
            // }
        }
    }
    public void reachedLevel(int value)
    {
        if (value > _furthestLevelReached.val)
        {
            Debug.Log(this.GetType() + " reachedLevel setting furthestLevel to " + value);
            _furthestLevelReached.val = value;
        }
        else
        {
            Debug.LogWarning(this.GetType() + " reachedLevel tried to update furthestLevel to " + value + " <= " + _furthestLevelReached.val);
        }
    }

    private BoolConfigurationParameter _isAdmin = new BoolConfigurationParameter(_adminStartValue, _adminDefaultValue, _isAdminKey);
    public bool isAdmin
    {
        get
        {
            return _isAdmin.val;
        }
        set
        {
            if (value != _isAdmin.val)
            {
                _isAdmin.val = value;
                RedMetricsManager.instance.sendEvent(TrackingEvent.SWITCHFROMGAMEVERSION, RedMetricsManager.instance.generateCustomDataForGuidInit());
                setMetricsDestination(!value);
                RedMetricsManager.instance.sendEvent(TrackingEvent.SWITCHTOGAMEVERSION, RedMetricsManager.instance.generateCustomDataForGuidInit());
            }
        }
    }

    public float[] bestTimes
    {
        get
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " get _bestTimes");
#endif
            initializeBestTimesIfNecessary();
            float[] result = new float[_bestTimes.Length];
            for (int index = 0; index < _bestTimes.Length; index++)
            {
                result[index] = _bestTimes[index].val;
            }
            return result;
        }
        set
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " set _bestTimes");
#endif
            initializeBestTimesIfNecessary();
            for (int index = 0; index < _bestTimes.Length; index++)
            {
                if (value[index] < _bestTimes[index].val)
                {
#if VERBOSEDEBUG
                    Debug.Log(this.GetType() + " set _bestTimes[" + index + "] to " + value[index] + "(previously " + _bestTimes[index].val + ")");
#endif
                    _bestTimes[index].val = value[index];
                }
            }
        }
    }

    public void setBestTime(int index, float time, bool force = false)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setBestTime(" + index + ", " + time + ", " + force + ")");
#endif
        if (force || time < _bestTimes[index].val)
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " setBestTime updates " + index + " to " + time);
#endif
            _bestTimes[index].val = time;
        }
    }

    private const string _restartBehaviorKey = "restartBehavior";
    private const string _gameMapKey = "gameMap";
    private const string _isAbsoluteWASDKey = "isAbsoluteWASD";
    private const string _isLeftClickToMoveKey = "isLeftClickToMove";
    private const string _isSoundOnKey = "isSoundOn";
    private const string _furthestLevelReachedKey = "levelReached";
    private const string _bestCompletionTimeLevelStem = "bestCompletionTimeLevel";
    private const string _isAdminKey = "isAdmin";

    public void load()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " load");
#endif
        RedMetricsManager.instance.localPlayerGUID = playerGUID;

        _isAdmin.initialize();
        initializeGameVersionGUID();
        _isSoundOn.initialize();
        _furthestLevelReached.initialize();
  
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " load done");
#endif
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    ///REDMETRICS TRACKING ///////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////
    // redmetrics.io
    // test
    public const string testVersionGUIDString = "b5ba9b46-9c6d-43d6-8611-3b2af6afff48";
    public System.Guid testVersionGUID = new System.Guid(testVersionGUIDString);
    // temporarily: labelled := test
    public const string labelledGameVersionGUIDString = testVersionGUIDString;
    public System.Guid labelledGameVersionGUID = new System.Guid(labelledGameVersionGUIDString);

    public void reset()
    {
        Debug.Log(this.GetType() + " reset");
        furthestLevel = 0;
        setWebGUID(null);
#if UNITY_WEBGL
        Debug.Log(this.GetType() + " reset Application.ExternalCall(deleteLocalPlayerGUID);");
        Application.ExternalCall("deleteLocalPlayerGUID");
#endif
        _playerGUID = null;
        PlayerPrefs.DeleteAll();
        load();
        RedMetricsManager.instance.sendStartEvent(true);
    }

    private string _webGUID;
    public void setWebGUID(string webGUID)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setWebGUID(" + webGUID + ")");
#endif
        _webGUID = webGUID;
    }

    private string _playerGUID;
    public string playerGUID
    {
        get
        {
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " playerGUID get");
#endif
            if (string.IsNullOrEmpty(_playerGUID))
            {
#if VERBOSEDEBUG
                Debug.Log(this.GetType() + " playerGUID get string.IsNullOrEmpty(_playerGUID)");
#endif
                //TODO make it work through different versions of the game,
                //     so that memory is not erased every time a new version of the game is published
                string storedGUID = PlayerPrefs.GetString(_localPlayerGUIDPlayerPrefsKey);
                if (string.IsNullOrEmpty(storedGUID))
                {
#if UNITY_WEBGL
				    if(!string.IsNullOrEmpty(_webGUID))
                    {
#if VERBOSEDEBUG
                        Debug.Log(this.GetType() + " playerGUID get !string.IsNullOrEmpty(_webGUID)");
#endif
                        _playerGUID = _webGUID;
                        PlayerPrefs.SetString(_localPlayerGUIDPlayerPrefsKey, _playerGUID);
                    }
                    else
                    {
#endif
#if VERBOSEDEBUG
                        Debug.Log(this.GetType() + " playerGUID get string.IsNullOrEmpty(storedGUID)");
#endif
                        _playerGUID = Guid.NewGuid().ToString();
                        PlayerPrefs.SetString(_localPlayerGUIDPlayerPrefsKey, _playerGUID);
#if UNITY_WEBGL
                        Debug.Log(this.GetType() + " playerGUID get Application.ExternalCall(initializeGUIDState);");
                        Application.ExternalCall("initializeGUIDState");
                    }
#endif
                }
                else
                {
#if VERBOSEDEBUG
                    Debug.Log(this.GetType() + " playerGUID get _playerGUID = storedGUID");
#endif
                    _playerGUID = storedGUID;
                }
            }
#if VERBOSEDEBUG
            Debug.Log(this.GetType() + " playerGUID get returns " + _playerGUID);
#endif
            return _playerGUID;
        }
    }

    public void initializeGameVersionGUID()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " initializeGameVersionGUID with RedMetricsManager.gameVersion=" + RedMetricsManager.instance.getGameVersion());
#endif
        if (!RedMetricsManager.instance.isGameVersionInitialized())
        {
            setMetricsDestination(!isAdmin);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " initializeGameVersionGUID RedMetricsManager GameVersion already initialized");
        }
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " initializeGameVersionGUID done with"
        + " guid=" + RedMetricsManager.instance.getGameVersion()
        + ", isAdmin=" + isAdmin
        + ", isTestGUID=" + isTestGUID()
        );
#endif
    }

    // use: check that stored guid is correct ie either current labelled or test, not an older version
    private bool isGUIDCorrect(string guid)
    {
        // Debug.Log(this.GetType() + " initializeGameVersionGUID("+guid+")");
        return (!string.IsNullOrEmpty(guid)) && (guid == labelledGameVersionGUIDString || guid == testVersionGUIDString);
    }

    //sets the destination to which logs will be sent
    public void setMetricsDestination(bool wantToBecomeLabelledGameVersion)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setMetricsDestination(" + wantToBecomeLabelledGameVersion + ")");
#endif
        System.Guid guid = wantToBecomeLabelledGameVersion ? labelledGameVersionGUID : testVersionGUID;

        if (guid != RedMetricsManager.instance.getGameVersion())
        {
            setGameVersion(guid);
        }
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setMetricsDestination(" + wantToBecomeLabelledGameVersion + ") done");
#endif
    }

    private void setGameVersion(System.Guid guid)
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setGameVersion " + guid);
#endif
        RedMetricsManager.instance.setGameVersion(guid);
        if (isTestGUID() != isAdmin)
        {
            Debug.LogWarning(this.GetType() + " setGameVersion: incorrect status isTestGUID() != isAdmin");
        }
    }

    public bool isTestGUID()
    {
#if VERBOSEDEBUG
        Debug.Log(this.GetType() + " isTestGUID");
#endif
        return testVersionGUID == RedMetricsManager.instance.getGameVersion();
    }

    public override string ToString()
    {
        string bestTimesString = "[";
        float[] bt = bestTimes;
        for (int index = 0; index < bt.Length; index++)
        {
            if (index != 0)
            {
                bestTimesString += ", ";
            }
            bestTimesString += index + ":" + bt[index];
        }
        bestTimesString += "]";

        return string.Format("[GameConfiguration: isAdmin={0}, playerGUID={1}, isSoundOn={2}, furthestLevel={3}, bestTimes={4}]",
        isAdmin, playerGUID, isSoundOn, furthestLevel, bestTimesString);
    }
}
