//#define DEVMODE
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Collections;

public class LocalizationManager : MonoBehaviour
{

    public static LocalizationManager instance;

    public static UnityEvent languageChanged = new UnityEvent();

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    public enum LANGUAGES { ENGLISH, FRENCH, RUSSIAN, HINDI, CHINESE, COUNT }
    private string[] languages = { "English", "French", "Russian", "Hindi", "Chinese" };
    public const LANGUAGES defaultLanguage = LANGUAGES.FRENCH;
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
        }
    }

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            language = defaultLanguage;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
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
    }

}