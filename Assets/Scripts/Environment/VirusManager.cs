using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusManager : GenericEntityGroupManager<Virus>
{
    public static VirusManager derivedInstance = null;

    [SerializeField]
    private int popMax = 20;
    [SerializeField]
    private int popMin = 4;
    [SerializeField]
    private Transform _entryPoint = null;
    [SerializeField]
    private Transform _waypoint = null;
    public Vector3 waypoint {
        get
        {
            return _waypoint.position + new Vector3(0f, 0f, Random.Range(-_entryRadius , _entryRadius));
        }
    }
    [SerializeField]
    private Transform _escapePoint = null;
    public Vector3 escapePoint {
        get
        {
            return _escapePoint.position + new Vector3(0f, 0f, Random.Range(-_entryRadius , _entryRadius));
        }
    }
    [SerializeField]
    private float _entryRadius = 3f;
    [SerializeField]
    private float _escapeRadius = 3f;
    [SerializeField]
    private GameObject _virusPrefab = null;
    public GameObject virusPrefab { get { return _virusPrefab; } }

    protected override void Awake()
    {
        base.Awake();
        if (null == derivedInstance)
        {
            derivedInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (entitiesList.Count > popMax)
        {
            for (int i = entitiesList.Count-1; i >= popMax; i--)
            {
                entitiesList[i].escape(escapePoint);
            }
        }
        else if (entitiesList.Count < popMin)
        {
            int startI = (int)Mathf.Max(entitiesList.Count-1, 0);
            for (int i = startI; i < popMin-startI; i++)
            {
                Vector3 spawnPointPosition = _entryPoint.position + new Vector3(0f, 0f, Random.Range(-_entryRadius , _entryRadius));
                Instantiate(_virusPrefab, spawnPointPosition, _virusPrefab.transform.rotation);
            }
        }
    }
}
