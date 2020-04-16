using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WhiteBloodCellManager : MonoBehaviour
{
    public static WhiteBloodCellManager instance = null;

    [SerializeField] // for debug
    private WhiteBloodCellMovement[] whiteBloodCells = null;
    [SerializeField] // for debug
    private Enemy[] whiteBloodCellsTarget = null;
    private WhiteBloodCellMovement[] availableWBCs = null;

    [SerializeField]
    private GameObject[] wbcPrefabs = null;

    [SerializeField]
    private bool respawnWBCs = false;
    [SerializeField]
    private float respawnPeriod = 10f;
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

    public const int wbcSpawnCount = 4;
    private float wbcSpawnTimePeriod = 1f;

    private Vector3 wbcSpawnSpatialPeriod = Vector3.zero;

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
            _limitTop =    (null != limitTopGO)    ? limitTopGO.position.x    : _limitTop;
            _limitBottom = (null != limitBottomGO) ? limitBottomGO.position.x : _limitBottom;
            _limitLeft =   (null != limitLeftGO)   ? limitLeftGO.position.x   : _limitLeft;
            _limitRight =  (null != limitRightGO)  ? limitRightGO.position.x  : _limitRight;

            #if VERBOSEDEBUG
            Debug.Log(string.Format("{0}: {1}: Awake ", this.GetType(), this.gameObject.name));
            #endif

            whiteBloodCells = new WhiteBloodCellMovement[wbcSpawnCount];
            whiteBloodCellsTarget = new Enemy[wbcSpawnCount];
            availableWBCs = new WhiteBloodCellMovement[wbcSpawnCount];
            respawnCountdown = respawnPeriod;

            for (int i = 0; i < wbcSpawnCount; i++)
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
        Vector3 verticalSpatialPeriod = diff.z / (wbcSpawnCount + 1) * Vector3.forward;
        Vector3 horizontalSpatialPeriod = diff.x / (wbcSpawnCount + 1) * Vector3.right;
        wbcSpawnSpatialPeriod = verticalSpatialPeriod + horizontalSpatialPeriod;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // manage movement of white cells
        for (int i = 0; i < whiteBloodCellsTarget.Length; i++)
        {
            if ((null != whiteBloodCells[i]) && (null == whiteBloodCellsTarget[i]))
            {
                for (int j = 0; j < WaveSpawner.enemiesAlive.Length; j++)
                {
                    bool isEnemyTargetable = true;
                    if (null != WaveSpawner.enemiesAlive[j])
                    {
                        // control that no other WBC is targeting it
                        for (int k = 0; k < whiteBloodCellsTarget.Length; k++)
                        {
                            if (whiteBloodCellsTarget[k] == WaveSpawner.enemiesAlive[j])
                            {
                                isEnemyTargetable = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // enemy is null
                        isEnemyTargetable = false;
                    }
                    if (isEnemyTargetable)
                    {
                        whiteBloodCellsTarget[i] = WaveSpawner.enemiesAlive[j];
                        whiteBloodCells[i].setTarget(WaveSpawner.enemiesAlive[j].transform);
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

    private int getFirstNullWBCIndex()
    {
        int result = 0;
        for (result = 0; result < wbcSpawnCount; result++)
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

        GameObject wbcPrefab = wbcPrefabs[UnityEngine.Random.Range(0, wbcPrefabs.Length)];
        float t = UnityEngine.Random.Range(0f, 1f);
        Vector3 spawnPointPosition = t * BloodUtilities.instance.bloodOrigin1.position + (1 - t) * BloodUtilities.instance.bloodOrigin2.position;
        GameObject newWBC = (GameObject)Instantiate(wbcPrefab, spawnPointPosition, wbcPrefab.transform.rotation);

        WhiteBloodCellMovement wbcm = newWBC.GetComponent<WhiteBloodCellMovement>();
        whiteBloodCells[index] = wbcm;
        Vector3 idlePosition = BloodUtilities.instance.bloodOrigin1.position + (index + 1) * wbcSpawnSpatialPeriod;
        wbcm.initialize(index, idlePosition);
    }

    public void reportDeath(int wbcIndex)
    {
        whiteBloodCells[wbcIndex] = null;
        whiteBloodCellsTarget[wbcIndex] = null;
    }
}
