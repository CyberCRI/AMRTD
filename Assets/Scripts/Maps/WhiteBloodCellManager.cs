using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WhiteBloodCellManager : MonoBehaviour
{
    public static WhiteBloodCellManager instance = null;

    [SerializeField]
    private GameObject[] whiteBloodCells = null;
    private Enemy[] whiteBloodCellsTarget = null;
    
    [SerializeField]
    private GameObject[] wbcPrefabs;

    private Transform bloodOrigin1 = null;
    private Transform bloodOrigin2 = null;
    private Transform bloodEnd1 = null;
    private Transform bloodEnd2 = null;
    
    private int wbcSpawnCount = 4;
    private float wbcSpawnPeriod = 1f;

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

    private void spawnWBC()
    {
        GameObject wbcPrefab = wbcPrefabs[UnityEngine.Random.Range(0, wbcPrefabs.Length)];
        float t = UnityEngine.Random.Range(0f, 1f);
        Vector3 spawnPointPosition = t * bloodOrigin1.position + (1 - t) * bloodOrigin2.position;
        Instantiate(wbcPrefab, spawnPointPosition, wbcPrefab.transform.rotation);
    }
}
