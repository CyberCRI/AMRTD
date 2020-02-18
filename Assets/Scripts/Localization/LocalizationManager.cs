//#define DEVMODE
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocalizationManager : MonoBehaviour
{

    public static LocalizationManager instance;

    public static UnityEvent languageChanged = new UnityEvent();

    public RectTransform uiRoot;
    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    public enum LANGUAGES
    {
        ENGLISH
        , FRENCH
        , RUSSIAN
        , HINDI
        , CHINESE
        , SPANISH
        , ARABIC
        , COUNT
    }
    private string[] languages = {
        "English"
        ,"French"
        ,"Russian"
        ,"Hindi"
        ,"Chinese"
        ,"Spanish"
        ,"Arabic"
    };
    public const LANGUAGES defaultLanguage = LANGUAGES.ENGLISH;
    private LANGUAGES _languageIndex;
    private string _languageString;
    public LANGUAGES language
    {
        get
        {
            return _languageIndex;
        }
        set
        {
            _languageIndex = value;
            _languageString = languages[(int)_languageIndex];
            StartCoroutine("loadStreamingAsset", _languageString + ".json");
            //#if DEVMODE
            //            Debug.Log("language set");
            //#endif

            refreshUIElements("language.set");
        }
    }

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            language = defaultLanguage;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += onSceneLoaded;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        refreshUIElements("Start()");
    }

    private void refreshUIElements(string debug = "")
    {
#if DEVMODE
        Debug.Log("refreshUIElements(" + debug + ")");
#endif

        Canvas.ForceUpdateCanvases();
        if (null != uiRoot)
        {
#if DEVMODE
            Debug.Log("ForceRebuildLayoutImmediate");
#endif
            LayoutRebuilder.ForceRebuildLayoutImmediate(uiRoot);
        }

        HorizontalLayoutGroup[] hlg = GameObject.FindObjectsOfType<HorizontalLayoutGroup>();
        for (int i = 0; i < hlg.Length; i++)
        {
            hlg[i].enabled = false;
        }
#if DEVMODE
        Debug.Log(" hlg.Length=" + hlg.Length);
#endif

        VerticalLayoutGroup[] vlg = GameObject.FindObjectsOfType<VerticalLayoutGroup>();
        for (int i = 0; i < vlg.Length; i++)
        {
            vlg[i].enabled = false;
        }
#if DEVMODE
        Debug.Log(" vlg.Length=" + vlg.Length);
#endif

        hlg = GameObject.FindObjectsOfType<HorizontalLayoutGroup>();
        for (int i = 0; i < hlg.Length; i++)
        {
            hlg[i].enabled = true;
        }

        vlg = GameObject.FindObjectsOfType<VerticalLayoutGroup>();
        for (int i = 0; i < vlg.Length; i++)
        {
            vlg[i].enabled = true;
        }
    }

    void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
#if DEVMODE
            Debug.Log("onSceneLoaded: " + scene.name + " with mode " + mode);
#endif        
        refreshUIElements("onSceneLoaded");
    }

    public LANGUAGES getNextLanguage()
    {
        return (LANGUAGES)(((int)language + 1) % ((int)LANGUAGES.COUNT));
    }

    public string GetLocalizedValue(string key)
    {
        string result = missingTextString;
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;
    }

    public bool GetIsReady()
    {
        return isReady;
    }

    IEnumerator loadStreamingAsset(string fileName)
    {
        localizedText = new Dictionary<string, string>();
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        //#if DEVMODE
        //        Debug.Log(filePath);
        //#endif

        string dataAsJson;
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            WWW www = new WWW(filePath);
            yield return www;
            dataAsJson = www.text;
        }
        else
        {
            dataAsJson = System.IO.File.ReadAllText(filePath);
        }

        LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

        for (int i = 0; i < loadedData.items.Length; i++)
        {
            localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
        }
        //#if DEVMODE
        //        Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        //#endif

        isReady = true;
        languageChanged.Invoke();

        refreshUIElements("loadStreamingAsset");
    }

}