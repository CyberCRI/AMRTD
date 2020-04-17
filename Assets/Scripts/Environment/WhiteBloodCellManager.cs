//#define VERBOSEDEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WhiteBloodCellManager : MonoBehaviour
{
    public static WhiteBloodCellManager instance = null;

    [SerializeField]
    private bool _chaseViruses = false;
    public bool chaseViruses { get { return _chaseViruses; } }

    #if VERBOSEDEBUG
    [SerializeField] // for debug
    #endif
    private WhiteBloodCellMovement[] whiteBloodCells = null;
    #if VERBOSEDEBUG
    [SerializeField] // for debug
    #endif
    private Virus[] whiteBloodCellsTargetViruses = null;
    #if VERBOSEDEBUG
    [SerializeField] // for debug
    #endif
    private Enemy[] whiteBloodCellsTargetEnemies = null;
    private WhiteBloodCellMovement[] availableWBCs = null;

    [SerializeField]
    private GameObject[] wbcPrefabs = null;

    [SerializeField]
    private bool respawnWBCs = false;
    [SerializeField]
    private float respawnPeriod = 0f;
    private float respawnCountdown = 0f;

    private bool _isAreaConstrained = true;
    public bool isAreaConstrained { get { return _isAreaConstrained; } }
    [SerializeField]
    private Transform limitTopGO = null;
    private float _limitTop    = Mathf.Infinity;
    public float limitTop { get { return _limitTop; } }
    [SerializeField]
    private Transform limitBottomGO = null;
    private float _limitBottom = -Mathf.Infinity;
    public float limitBottom { get { return _limitBottom; } }
    [SerializeField]
    private Transform limitLeftGO = null;
    private float _limitLeft   = -Mathf.Infinity;
    public float limitLeft { get { return _limitLeft; } }
    [SerializeField]
    private Transform limitRightGO = null;
    private float _limitRight  = Mathf.Infinity;
    public float limitRight { get { return _limitRight; } }

    [SerializeField]
    private int _wbcSpawnCount = 0;
    [SerializeField]
    private float wbcSpawnTimePeriod = 0f;

    private Vector3 wbcSpawnSpatialPeriod = Vector3.zero;

    private bool _searching = true;
    [SerializeField]
    private float _detectionRadius = 0f;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;

            _isAreaConstrained = (null != limitTopGO) || (null != limitBottomGO) || (null != limitLeftGO) || (null != limitRightGO);
            _limitTop =    (null != limitTopGO)    ? limitTopGO.position.z    : _limitTop;
            _limitBottom = (null != limitBottomGO) ? limitBottomGO.position.z : _limitBottom;
            _limitLeft =   (null != limitLeftGO)   ? limitLeftGO.position.x   : _limitLeft;
            _limitRight =  (null != limitRightGO)  ? limitRightGO.position.x  : _limitRight;

            #if VERBOSEDEBUG
            Debug.Log(string.Format("{0}: {1}: Awake ", this.GetType(), this.gameObject.name));
            #endif

            whiteBloodCells = new WhiteBloodCellMovement[_wbcSpawnCount];
            whiteBloodCellsTargetViruses = new Virus[_wbcSpawnCount];
            whiteBloodCellsTargetEnemies = new Enemy[_wbcSpawnCount];
            availableWBCs = new WhiteBloodCellMovement[_wbcSpawnCount];
            respawnCountdown = respawnPeriod + _wbcSpawnCount * wbcSpawnTimePeriod;

            for (int i = 0; i < _wbcSpawnCount; i++)
            {
                // +1 otherwise Start is called too late to initialize blood points
                Invoke("spawnWBC", (i + 1) * wbcSpawnTimePeriod);
            }
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        #if VERBOSEDEBUG
        Debug.Log(string.Format("{0}: {1}: Start ", this.GetType(), this.gameObject.name));
        #endif

        Vector3 diff = (BloodUtilities.instance.bloodEnd1.position - BloodUtilities.instance.bloodOrigin1.position);
        Vector3 verticalSpatialPeriod = diff.z / (_wbcSpawnCount + 1) * Vector3.forward;
        Vector3 horizontalSpatialPeriod = diff.x / (_wbcSpawnCount + 1) * Vector3.right;
        wbcSpawnSpatialPeriod = verticalSpatialPeriod + horizontalSpatialPeriod;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // manage movement of white cells
        for (int i = 0; i < whiteBloodCells.Length; i++)
        {
            // is this WBC idle?
            if ((null != whiteBloodCells[i]) && (null == whiteBloodCellsTargetEnemies[i]) && ((!chaseViruses) || (null == whiteBloodCellsTargetViruses[i])))
            {
                _searching = true;

                if (chaseViruses)
                {
                    // first try: nearby viruses
                    List<GameObject> nearbyGOs = whiteBloodCells[i].getNearbyGameObjects(Virus.virusTag, _detectionRadius);
                    for (int j = 0; j < nearbyGOs.Count; j++)
                    {
                        if (0 > getWBCTargeting(nearbyGOs[j]))
                        {
                            #if VERBOSEDEBUG
                            Debug.Log(string.Format("Allocate WBC {0} to nearby virus {1}", whiteBloodCells[i].name, nearbyGOs[j].name));
                            #endif
                            _searching = false;
                            whiteBloodCellsTargetViruses[i] = nearbyGOs[j].GetComponent<Virus>();
                            whiteBloodCells[i].setTarget(nearbyGOs[j].transform);
                            break;
                        }
                    }

                    // second try: all viruses
                    if (_searching)
                    {
                        for (int j = 0; j < VirusManager.instance.entitiesList.Count; j++)
                        {
                            // control that no other WBC is targeting it
                            if ((null != VirusManager.instance.entitiesList[j]) && (0 > getWBCTargeting(VirusManager.instance.entitiesList[j])))
                            {
                                #if VERBOSEDEBUG
                                Debug.Log(string.Format("Allocate WBC {0} to virus {1}", whiteBloodCells[i].name, VirusManager.instance.entitiesList[j].name));
                                #endif
                                _searching = false;
                                whiteBloodCellsTargetViruses[i] = VirusManager.instance.entitiesList[j];
                                whiteBloodCells[i].setTarget(VirusManager.instance.entitiesList[j].transform);
                                break;
                            }
                        }
                    }
                }

                // third try: enemies
                if (_searching)
                {
                    for (int j = 0; j < WaveSpawner.instance.enemiesAlive.Length; j++)
                    {
                        // control that no other WBC is targeting it
                        if ((null != WaveSpawner.instance.enemiesAlive[j]) && (0 > getWBCTargeting(WaveSpawner.instance.enemiesAlive[j])))
                        {
                            #if VERBOSEDEBUG
                            Debug.Log(string.Format("Allocate WBC {0} to enemy {1}", whiteBloodCells[i].name, WaveSpawner.instance.enemiesAlive[j].name));
                            #endif
                            _searching = false;
                            whiteBloodCellsTargetEnemies[i] = WaveSpawner.instance.enemiesAlive[j];
                            whiteBloodCells[i].setTarget(WaveSpawner.instance.enemiesAlive[j].transform);
                            break;
                        }
                    }
                }
            }
        }
        if (respawnWBCs)
        {
            respawnCountdown -= Time.deltaTime;
            if (respawnCountdown <= 0)
            {
                int wbcAlive = 0;
                for (int i = 0; i < whiteBloodCells.Length; i++)
                {
                    if (null == whiteBloodCells[i])
                    {
                        spawnWBC(i);
                        break;
                    }
                }
                respawnCountdown = respawnPeriod;
            }
        }
    }

    private int getWBCTargeting(Virus virus)
    {
        int result = -1;
        if (chaseViruses && (null != virus))
        {
            for (int i = 0; i < whiteBloodCellsTargetViruses.Length; i++)
            {
                if (whiteBloodCellsTargetViruses[i] == virus)
                {
                    result = i;
                    break;
                }
            }
        }
        return result;
    }

    private int getWBCTargeting(Enemy enemy)
    {
        int result = -1;
        if (null != enemy)
        {
            for (int i = 0; i < whiteBloodCellsTargetEnemies.Length; i++)
            {
                if (whiteBloodCellsTargetEnemies[i] == enemy)
                {
                    result = i;
                    break;
                }
            }
        }
        return result;
    }

    private int getWBCTargeting(GameObject go)
    {
        int result = -1;
        if (null != go)
        {
            if (go.tag == Virus.virusTag)
            {
                result = getWBCTargeting(go.GetComponent<Virus>());
            }
            else if (go.tag == Enemy.enemyTag)
            {
                result = getWBCTargeting(go.GetComponent<Enemy>());
            }
        }
        return result;
    }

    private int getFirstNullWBCIndex()
    {
        int result = 0;
        for (result = 0; result < _wbcSpawnCount; result++)
        {
            if (null == whiteBloodCells[result])
            {
                break;
            }
        }
        return result;
    }

    private void spawnWBC()
    {
        spawnWBC(-1);
    }

    private void spawnWBC(int _index)
    {
        #if VERBOSEDEBUG
        Debug.Log(string.Format("{0}: {1}: spawnWBC({2}) ", this.GetType(), this.gameObject.name, _index));
        #endif

        int index = _index == -1 ? getFirstNullWBCIndex() : _index;
        if (index >= whiteBloodCells.Length)
        {
            Debug.LogWarning("could not spawn WBC; index=" + _index);
        }
        else
        {
            GameObject wbcPrefab = wbcPrefabs[UnityEngine.Random.Range(0, wbcPrefabs.Length)];
            float t = UnityEngine.Random.Range(0f, 1f);
            Vector3 spawnPointPosition = t * BloodUtilities.instance.bloodOrigin1.position + (1 - t) * BloodUtilities.instance.bloodOrigin2.position;
            GameObject newWBC = (GameObject)Instantiate(wbcPrefab, spawnPointPosition, wbcPrefab.transform.rotation);
            newWBC.name = "WBC" + index;

            WhiteBloodCellMovement wbcm = newWBC.GetComponent<WhiteBloodCellMovement>();
            whiteBloodCells[index] = wbcm;
            
            Vector3 idlePosition = Vector3.zero;
            if (BloodUtilities.instance.isManualIdlePositions)
            {
                idlePosition = BloodUtilities.instance.idlePositions[index].position;
            }
            else
            {
                idlePosition = BloodUtilities.instance.bloodOrigin1.position + (index + 1) * wbcSpawnSpatialPeriod;
            }
            wbcm.initialize(index, idlePosition);
        }
    }

    public void reportDeath(int wbcIndex)
    {
        whiteBloodCells[wbcIndex] = null;
        whiteBloodCellsTargetEnemies[wbcIndex] = null;
        whiteBloodCellsTargetViruses[wbcIndex] = null;
    }

    public void triggerMassWBC()
    {    
        respawnPeriod /= 10f;
        respawnCountdown = 0f;
    }
}
