#define VERBOSEDEBUG

using UnityEngine;
using System.Collections.Generic;

public class CustomData : Dictionary<string, string>
{
    public CustomData()
    {
    }

    private CustomData(string key, string value) : base()
    {
        this.Add(key, value);
    }

    public CustomData(CustomDataTag tag, string value) : this(tag.ToString().ToLowerInvariant(), value)
    {

    }

    public void Add(CustomDataTag tag, string value)
    {
        Add(tag.ToString().ToLowerInvariant(), value);
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
                if (this.ContainsKey(pair.Key))
                {
                    // this key was already present
                    // each key-value pair type needs a specific treatment
                    Debug.LogWarning(this.GetType() + " key " + pair.Key + " present in both CustomData objects " + data + " and " + this);
                }
                else
                {
                    // new key
                    this.Add(pair.Key, pair.Value);
                }
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
