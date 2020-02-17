using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowElementsCounter : MonoBehaviour
{
    public int calls = 0;
    public static int sCalls = 0;

    // Start is called before the first frame update
    void Start()
    {
        browseAndCount(this.transform);
    }

    private void browseAndCount(Transform t)
    {
        sCalls++;
        calls = sCalls;
        if (null != t)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Transform tChild = t.GetChild(i);
                //Debug.Log(this.GetType() + " Inspecting " + tChild);
                countShadows(tChild);
                browseAndCount(tChild);
            }
        }
    }

    private void countShadows(Transform t)
    {
        if (null != t && null != t.gameObject)
        {
            GameObject go = t.gameObject;
            Shadow[] shadows = go.GetComponents<Shadow>();
            if (shadows.Length > 1)
            {
                Debug.LogWarning(this.GetType() + " 1+ shadows found: " + go.name);
            }
            /*
            else
            {
                Debug.Log(this.GetType() + " ok " + go.name);
            }
            */
        }
    }
}
