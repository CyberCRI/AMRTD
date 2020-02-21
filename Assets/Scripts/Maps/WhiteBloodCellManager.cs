using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WhiteBloodCellManager : MonoBehaviour
{
    public static WhiteBloodCellManager instance = null;

    private GameObject[] whiteBloodCells = new GameObject[wbcSpawnCount];
    [SerializeField] // for debug
    private Enemy[] whiteBloodCellsTarget = null;
    
    [SerializeField]
    private GameObject[] wbcPrefabs;

    private Transform bloodOrigin1 = null;
    private Transform bloodOrigin2 = null;
    private Transform bloodEnd1 = null;
    private Transform bloodEnd2 = null;
    
    public const int wbcSpawnCount = 4;
    private float wbcSpawnPeriod = 1f;

    private Vector3 spatialPeriod = Vector3.zero;

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
            whiteBloodCellsTarget = new Enemy[whiteBloodCells.Length];

            for (int i = 0; i < wbcSpawnCount; i++)
            {
                // +1 otherwise Start is called too late to initialize blood points
                Invoke("spawnWBC", (i + 1) * wbcSpawnPeriod);
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
        spatialPeriod = verticalSpatialPeriod + horizontalSpatialPeriod;
    }

    /*
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // manage count of white blood cells
        if (
            (whiteBloodCells.Length > 0)
            && (whiteBloodCells.Length + 1 > PlayerStatistics.instance.lives)
        )
        {
            Destroy(whiteBloodCells[whiteBloodCells.Length - 1]);
            Array.Resize(ref whiteBloodCells, whiteBloodCells.Length - 1);
            Array.Resize(ref whiteBloodCellsTarget, whiteBloodCellsTarget.Length - 1);
        }

        // manage movement of white cells
        int busyWBCs = 0;
        for (int i = 0; i < whiteBloodCellsTarget.Length; i++)
        {
            if (null != whiteBloodCellsTarget[i])
            {
                busyWBCs++;
            }
        }

        while (busyWBCs < WaveSpawner.enemiesAliveCount)
        {
            for (int i = 0; i < whiteBloodCellsTarget.Length; i++)
            {
                if (null != whiteBloodCellsTarget[i])
                {
                    busyWBCs++;
                }
            }
        }
    }
    */

    private int getNonNullWBCCount()
    {
        int result = 0;
        for (int i = 0; i < wbcSpawnCount; i++)
        {
            if (null != whiteBloodCells[i])
            {
                result++;
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

        int index = getNonNullWBCCount();
        whiteBloodCells[index] = newWBC;
        WhiteBloodCellMovement wbcm = newWBC.GetComponent<WhiteBloodCellMovement>();
        Vector3 idlePosition = bloodOrigin1.position + (index + 1) * spatialPeriod;
        wbcm.initialize(idlePosition);
    }
}
