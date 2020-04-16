using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEntityGroupManager<T> : MonoBehaviour
{
    public static GenericEntityGroupManager<T> instance = null;
    private T[] _entities;
    public T[] entities
    {
        get
        {
            if (_hasNewRegistrations || (null == _entities))
            {
                _entities = tempEntities.ToArray();
                //#if VERBOSEDEBUG
                Debug.Log(" tempEntities.ToArray() with _entities.Length=" + _entities.Length);
                //#endif
                _hasNewRegistrations = false;
            }
            return _entities;
        }
    }
    private System.Collections.Generic.List<T> tempEntities = new System.Collections.Generic.List<T>();
    [HideInInspector]
    public int entityIndex = 0;
    private bool _hasNewRegistrations = false;

    protected virtual void Awake()
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

    public void register(T entity)
    {
        _hasNewRegistrations = true;
        tempEntities.Add(entity);
    }

    private void resetStatics()
    {
        _entities = null;
        tempEntities.Clear();
        entityIndex = 0;
    }

    void OnDestroy()
    {
        resetStatics();
    }
}
