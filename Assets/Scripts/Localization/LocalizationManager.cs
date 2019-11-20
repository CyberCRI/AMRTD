//#define DEVMODE
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class LocalizationManager : MonoBehaviour
{

    public static LocalizationManager instance;

    public static UnityEvent languageChanged = new UnityEvent();

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    private string[] languages = new string[3] {"English", "French", "Russian"};
    private string _language;
    public string language {
        get {
            return _language;
        }
        set {
            _language = value;
            LoadLocalizedText(_language + ".json");
            languageChanged.Invoke();
#if DEVMODE
            Debug.Log("set");
#endif
        }
    }

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            language = languages[1];
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public string getNextLanguage()
    {
        int tries = 0;
        int result = 0;
        int i = 0;
        bool done = false;

        while(!done && (tries < 10))
        {
            if (languages[i] == language)
            {
                result = (i + 1) % languages.Length;
                done = true;
            }
            else
            {
                i = (i + 1) % languages.Length;
            }
            tries++;
        }
        return languages[result];
    }

    public void LoadLocalizedText(string fileName)
    {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
#if DEVMODE
        Debug.Log(filePath);
#endif

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
#if DEVMODE
            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        }
        else
        {
            Debug.LogError("Cannot find file!");
#endif
        }

        isReady = true;
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

}