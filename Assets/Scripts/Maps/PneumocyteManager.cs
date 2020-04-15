//#define VERBOSEDEBUG
//#define DEVMODE

using UnityEngine;
using System.Collections;

public class PneumocyteManager : MonoBehaviour
{
    public static PneumocyteManager instance = null;
    private Pneumocyte[] _pneumocytes;
    public Pneumocyte[] pneumocytes
    {
        get
        {
            if (null == _pneumocytes)
            {
                _pneumocytes = tempPneumocytes.ToArray();
                //#if VERBOSEDEBUG
                Debug.Log(" tempPneumocytes.ToArray() with _pneumocytes.Length=" + _pneumocytes.Length);
                //#endif
            }
            return _pneumocytes;
        }
    }
    private System.Collections.Generic.List<Pneumocyte> tempPneumocytes = new System.Collections.Generic.List<Pneumocyte>();
    [HideInInspector]
    public int pneumocyteIndex = 0;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void register(Pneumocyte pneumocyte)
    {
        tempPneumocytes.Add(pneumocyte);
    }

    private void resetStatics()
    {
        _pneumocytes = null;
        tempPneumocytes.Clear();
        pneumocyteIndex = 0;
    }

    void OnDestroy()
    {
        resetStatics();
    }
}