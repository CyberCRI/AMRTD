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
    private GameObject[] wbcPrefabs;

    private Transform bloodOrigin1 = null;
    private Transform bloodOrigin2 = null;
    private Transform bloodEnd1 = null;
    private Transform bloodEnd2 = null;

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
            whiteBloodCells = new WhiteBloodCellMovement[wbcSpawnCount];
            whiteBloodCellsTarget = new Enemy[whiteBloodCells.Length];
            availableWBCs = new WhiteBloodCellMovement[wbcSpawnCount];

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
        Transform[] positions = RedBloodCellManager.instance.getBloodPositions();
        bloodOrigin1 = positions[0];
        bloodOrigin2 = positions[1];
        bloodEnd1 = positions[2];
        bloodEnd2 = positions[3];

        Vector3 diff = (bloodEnd1.position - bloodOrigin1.position);
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
        int busyWBCs = 0;
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
        GameObject wbcPrefab = wbcPrefabs[UnityEngine.Random.Range(0, wbcPrefabs.Length)];
        float t = UnityEngine.Random.Range(0f, 1f);
        Vector3 spawnPointPosition = t * bloodOrigin1.position + (1 - t) * bloodOrigin2.position;
        GameObject newWBC = (GameObject)Instantiate(wbcPrefab, spawnPointPosition, wbcPrefab.transform.rotation);

        int index = getFirstNullWBCIndex();
        WhiteBloodCellMovement wbcm = newWBC.GetComponent<WhiteBloodCellMovement>();
        whiteBloodCells[index] = wbcm;
        Vector3 idlePosition = bloodOrigin1.position + (index + 1) * wbcSpawnSpatialPeriod;
        wbcm.initialize(idlePosition);
    }
}
