//#define DETRIMENTALOPTIMIZATION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBloodCellManager : MonoBehaviour
{
    public static RedBloodCellManager instance = null;

    [SerializeField]
    private Transform bloodOrigin1 = null;
    [SerializeField]
    private Transform bloodOrigin2 = null;
    [SerializeField]
    private Transform bloodEnd1 = null;
    [SerializeField]
    private Transform bloodEnd2 = null;
    
    [SerializeField]
    private GameObject[] rbcPrefabs;
    
    [SerializeField]
    private int rbcSpawnCount = 0;
    private float rbcSpawnPeriod = 0f;
    [SerializeField]
    private float rbcSpawnPeriodVariationRatio = 0f;
    private float timer = 0f;
    private int rbcYetToSpawnCount = 0;

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

#if DETRIMENTALOPTIMIZATION
            rbcYetToSpawnCount = rbcSpawnCount;
#endif

            rbcSpawnPeriod = (bloodEnd1.position - bloodOrigin1.position).magnitude / 
                (rbcSpawnCount * rbcPrefabs[0].GetComponent<RedBloodCellMovement>().baseSpeed);

            for (int i = 0; i < rbcSpawnCount; i++)
            {
#if DETRIMENTALOPTIMIZATION
                Invoke("spawnRBC", (i + rbcSpawnPeriodVariationRatio * Random.Range(-1f, 1f)) * rbcSpawnPeriod);
#else
                InvokeRepeating("randomSpawnRBC", i * rbcSpawnPeriod, rbcSpawnCount * rbcSpawnPeriod);
#endif
            }
        }
    }

    private void randomSpawnRBC()
    {
        Invoke("spawnRBC", rbcSpawnPeriodVariationRatio * Random.Range(0f, 1f) * rbcSpawnPeriod);
    }

    private void spawnRBC()
    {
#if DETRIMENTALOPTIMIZATION
        rbcYetToSpawnCount--;
        Debug.Log("spawnRBC remaining: " + rbcYetToSpawnCount);
#endif
        GameObject rbcPrefab = rbcPrefabs[Random.Range(0, rbcPrefabs.Length)];
        float t = Random.Range(0f, 1f);
        Vector3 spawnPointPosition = t * bloodOrigin1.position + (1 - t) * bloodOrigin2.position;
        Instantiate(rbcPrefab, spawnPointPosition, rbcPrefab.transform.rotation);
    }

    public Transform[] getBloodPositions()
    {
        return new Transform[4]{bloodOrigin1, bloodOrigin2, bloodEnd1, bloodEnd2};
    }
}
