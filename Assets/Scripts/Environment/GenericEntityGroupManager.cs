﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEntityGroupManager<T> : MonoBehaviour
{
    public static GenericEntityGroupManager<T> instance = null;
    [SerializeField]
    private T[] _entities;
    public T[] entities
    {
        get
        {
            if (_hasNewRegistrations || (null == _entities))
            {
                _entities = entitiesList.ToArray();
                #if VERBOSEDEBUG
                Debug.Log(" entitiesList.ToArray() with _entities.Length=" + _entities.Length);
                #endif
                _hasNewRegistrations = false;
            }
            return _entities;
        }
    }
    [SerializeField]
    public List<T> _entitiesList = new List<T>();
    public List<T> entitiesList { get { return _entitiesList;} private set { _entitiesList = value; } }
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
        entitiesList.Add(entity);
    }

    public void unregister(T entity)
    {
        _hasNewRegistrations = true;
        entitiesList.Remove(entity);
    }

    private void resetStatics()
    {
        _entities = null;
        entitiesList.Clear();
        entityIndex = 0;
    }

    void OnDestroy()
    {
        resetStatics();
    }
}
