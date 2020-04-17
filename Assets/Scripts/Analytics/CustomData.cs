//#define VERBOSEDEBUG

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CustomData : Dictionary<string, string>
{
    public CustomData()
    {
    }

    public CustomData(CustomData data) : base()
    {
        foreach (KeyValuePair<string, string> pair in data)
        {
            this.Add(pair.Key, pair.Value);
        }
    }

    private CustomData(string key, string value) : base()
    {
        this.Add(key, value);
    }

    public CustomData(CustomDataTag tag, Vector3 value) : this(tag.ToString().ToLowerInvariant(), value.ToString())
    {
    }

    public CustomData(CustomDataTag tag, int value) : this(tag.ToString().ToLowerInvariant(), value.ToString())
    {
    }

    public CustomData(CustomDataTag tag, bool value) : this(tag.ToString().ToLowerInvariant(), value.ToString().ToLowerInvariant())
    {
    }

    public CustomData(CustomDataTag tag, string value) : this(tag.ToString().ToLowerInvariant(), value.ToLowerInvariant())
    {
    }

    public CustomData(CustomDataTag tag, CustomDataValue value) : this(tag.ToString().ToLowerInvariant(), value.ToString().ToLowerInvariant())
    {
    }

    public CustomData(CustomDataTag tag, GameObject value) : this(tag.ToString().ToLowerInvariant(), value.name.ToLowerInvariant())
    {
    }

    public CustomData add(CustomDataTag tag, Vector3 value)
    {
        this.Add(tag.ToString().ToLowerInvariant(), value.ToString());
        return this;
    }

    public CustomData add(CustomDataTag tag, int value)
    {
        this.Add(tag.ToString().ToLowerInvariant(), value.ToString());
        return this;
    }

    public CustomData add(CustomDataTag tag, bool value)
    {
        this.Add(tag.ToString().ToLowerInvariant(), value.ToString().ToLowerInvariant());
        return this;
    }

    public CustomData add(CustomDataTag tag, string value)
    {
        this.Add(tag.ToString().ToLowerInvariant(), value.ToLowerInvariant());
        return this;
    }

    public CustomData add(CustomDataTag tag, CustomDataValue value)
    {
        this.Add(tag.ToString().ToLowerInvariant(), value.ToString().ToLowerInvariant());
        return this;
    }

    public CustomData add(CustomDataTag tag, GameObject value)
    {
        this.Add(tag.ToString().ToLowerInvariant(), value.name.ToLowerInvariant());
        return this;
    }

    public static CustomData getContext(CustomDataTag tag)
    {
        CustomData data = new CustomData();
        data.addContext(tag);
        return data;
    }

    public CustomData addContext(CustomDataTag cdTag)
    {
        switch (cdTag)
        {
            case CustomDataTag.LOCALPLAYERGUID:
                add(CustomDataTag.LOCALPLAYERGUID, GameConfiguration.instance.playerGUID);
                break;
            case CustomDataTag.PLATFORM:
                add(CustomDataTag.PLATFORM, Application.platform.ToString());
                break;
            case CustomDataTag.RESOLUTION:
                add(CustomDataTag.RESOLUTION, Screen.currentResolution.ToString());
                break;
            case CustomDataTag.LIVES:
                if (null != PlayerStatistics.instance)
                {
                    add(CustomDataTag.LIVES, PlayerStatistics.instance.lives);
                }
                break;
            case CustomDataTag.FUNDS:
                if (null != PlayerStatistics.instance)
                {
                    add(CustomDataTag.FUNDS, PlayerStatistics.instance.money);
                }
                break;
            case CustomDataTag.RESISTANCE:
                if (null != PlayerStatistics.instance)
                {
                    add(CustomDataTag.RESISTANCE, PlayerStatistics.instance.resistancePoints.ToString("000.0"));
                }
                break;
            case CustomDataTag.WAVES:
                if (null != PlayerStatistics.instance)
                {
                    add(CustomDataTag.WAVES, PlayerStatistics.instance.waves);
                }
                break;
            case CustomDataTag.PATHOGENSALIVE:
                if (null != WaveSpawner.instance)
                {
                    add(CustomDataTag.PATHOGENSALIVE, WaveSpawner.instance.enemiesAliveCount);
                }
                break;
            case CustomDataTag.MAXPATHOGENCOUNT:
                if (null != WaveSpawner.instance)
                {
                    add(CustomDataTag.MAXPATHOGENCOUNT, WaveSpawner.instance.enemiesAlive.Length);
                }
                break;
            case CustomDataTag.TURRETCOUNT:
                if (null != PlayerStatistics.instance)
                {
                    add(CustomDataTag.TURRETCOUNT, PlayerStatistics.instance.turretCount);
                }
                break;
            case CustomDataTag.GAMELEVEL:
                add(CustomDataTag.GAMELEVEL, SceneManager.GetActiveScene().name);
                break;
            case CustomDataTag.LANGUAGE:
                add(CustomDataTag.LANGUAGE, LocalizationManager.instance.getLanguageString());
                break;
            case CustomDataTag.HELPMODE:
                if (null != HelpButtonUI.instance)
                {
                    add(CustomDataTag.HELPMODE, HelpButtonUI.instance.isHelpModeOn());
                }
                break;
            case CustomDataTag.TIMESINCEGAMELOADED:
                add(CustomDataTag.TIMESINCEGAMELOADED, Time.realtimeSinceStartup.ToString());
                // add(CustomDataTag.TIMESINCEGAMELOADED, Time.unscaledTime.ToString());
                break;
            case CustomDataTag.TIMEGAMEPLAYEDNOPAUSE:
                add(CustomDataTag.TIMEGAMEPLAYEDNOPAUSE, Time.time.ToString());
                break;
            case CustomDataTag.TIMESINCELEVELLOADED:
                add(CustomDataTag.TIMESINCELEVELLOADED, Time.timeSinceLevelLoad.ToString());
                break;
            /*
            case CustomDataTag.TIMELEVELPLAYEDNOPAUSE:
                add(CustomDataTag.TIMELEVELPLAYEDNOPAUSE, Time.?????.ToString());   
                break;    
            */
            default:
                Debug.LogError(this.GetType() + " addContext unexpected CustomDataTag " + cdTag);
                break;
        }
        return this;
    }

    public static CustomData getContext(CustomDataTag[] tags)
    {
        CustomData result = new CustomData();
        if (null != tags)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                result.addContext(tags[i]);
            }
        }
        return result;
    }

    public static CustomData getEventContext()
    {
        return getContext(
                new CustomDataTag[6]{
                    CustomDataTag.GAMELEVEL,
                    CustomDataTag.LIVES,
                    CustomDataTag.FUNDS,
                    CustomDataTag.RESISTANCE,
                    CustomDataTag.WAVES,
                    CustomDataTag.TIMESINCELEVELLOADED
                    }
            );
    }

    public static CustomData getGameLevelContext()
    {
        return getContext(CustomDataTag.GAMELEVEL);
    }

    public static CustomData getGameObjectContext(MonoBehaviour behaviour, CustomDataTag _tag = CustomDataTag.GAMEOBJECT)
    {
        return getGameObjectContext(behaviour.gameObject, _tag);
    }

    public static CustomData getGameObjectContext(GameObject go, CustomDataTag _tag = CustomDataTag.GAMEOBJECT)
    {
        return new CustomData(_tag, go.name).add(CustomDataTag.POSITION, go.transform.position);
    }

    public static CustomData getLevelEndContext()
    {
        return getContext(
                        new CustomDataTag[8]{
                            CustomDataTag.GAMELEVEL,
                            CustomDataTag.WAVES,
                            CustomDataTag.PATHOGENSALIVE,
                            CustomDataTag.MAXPATHOGENCOUNT,
                            CustomDataTag.TURRETCOUNT,
                            CustomDataTag.TIMESINCEGAMELOADED,
                            CustomDataTag.TIMEGAMEPLAYEDNOPAUSE,
                            CustomDataTag.TIMESINCELEVELLOADED,
                            }
                    );
    }

    public static CustomData merge(CustomData data1, CustomData data2)
    {
        CustomData result;
        if (null == data1)
        {
            result = data2;
        }
        else if (null == data2)
        {
            result = data1;
        }
        else
        {
            result = new CustomData(data1);
            result.merge(data2);
        }
        return result;
    }

    /// <summary>
    /// Merges data into this.
    /// </summary>
    public void merge(CustomData data)
    {
        // Debug.Log(this.GetType() + " merge " + data + " into " + this);
        if (null != data)
        {
            foreach (KeyValuePair<string, string> pair in data)
            {
                if (!this.ContainsKey(pair.Key))
                {
                    // new key
                    this.Add(pair.Key, pair.Value);
                }
#if VERBOSEDEBUG
                else
                {
                    // this key was already present
                    // each key-value pair type needs a specific treatment
                    Debug.LogWarning(this.GetType() + " key " + pair.Key + " present in both CustomData objects " + data + " and " + this);
                }
#endif
            }
        }
    }

    public string ToJSONString()
    {
        string content = "";
        foreach (KeyValuePair<string, string> entry in this)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content += ",";
            }
            content += "\"" + entry.Key + "\":\"" + entry.Value + "\"";
        }
        return "{" + content + "}";
    }

    public override string ToString()
    {
        string content = "";
        foreach (KeyValuePair<string, string> entry in this)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content += ",";
            }
            content += entry.Key + ":" + entry.Value;
        }
        return string.Format("[CustomData:[{0}]]", content);
    }
}
