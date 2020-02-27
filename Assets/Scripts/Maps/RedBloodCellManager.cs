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
    public Transform bloodOrigin2 = null;
    [SerializeField]
    private Transform bloodEnd1 = null;
    [SerializeField]
    private Transform bloodEnd2 = null;
    [SerializeField]
    public Transform bloodUnder = null;

    [SerializeField]
    private GameObject[] rbcPrefabs;

    [SerializeField]
    private int rbcSpawnCount = 0;
    private float rbcSpawnTimePeriod = 0f;
    [SerializeField]
    private float rbcSpawnTimePeriodVariationRatio = 0f;
    private float timer = 0f;
    private int rbcYetToSpawnCount = 0;
    private float topToBottom = 0f;

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

            topToBottom = (bloodEnd1.position - bloodOrigin1.position).z;

            rbcSpawnTimePeriod = Mathf.Abs(topToBottom / (rbcSpawnCount * rbcPrefabs[0].GetComponent<RedBloodCellMovement>().baseSpeed));

            // spatial spawn
            Vector3 rbcSpawnSpatialPeriod = topToBottom / (rbcSpawnCount + 1) * Vector3.forward;
            for (int i = 0; i < rbcSpawnCount; i++)
            {
                innerSpawnRBC((i + 1) * rbcSpawnSpatialPeriod);
            }

#if !DETRIMENTALOPTIMIZATION
            // temporal spawn
            for (int i = 0; i < rbcSpawnCount; i++)
            {
                // temporal spawn for DETRIMENTALOPTIMIZATION mode if no spatial spawn
                //Invoke("spawnRBC", (i + rbcSpawnPeriodVariationRatio * Random.Range(-1f, 1f)) * rbcSpawnPeriod);

                InvokeRepeating("randomSpawnRBC", i * rbcSpawnTimePeriod, rbcSpawnCount * rbcSpawnTimePeriod);
#endif
            }
        }
    }

    private void randomSpawnRBC()
    {
        Invoke("spawnRBC", rbcSpawnTimePeriodVariationRatio * Random.Range(0f, 1f) * rbcSpawnTimePeriod);
    }

    private void spawnRBC()
    {
        innerSpawnRBC(Vector3.zero);
    }

    private void innerSpawnRBC(Vector3 zOffset)
    {
#if DETRIMENTALOPTIMIZATION
        rbcYetToSpawnCount--;
        Debug.Log("innerSpawnRBC remaining: " + rbcYetToSpawnCount);
#endif
        GameObject rbcPrefab = rbcPrefabs[Random.Range(0, rbcPrefabs.Length)];
        float t = Random.Range(0f, 1f);
        Vector3 spawnPointPosition = t * bloodOrigin1.position + (1 - t) * bloodOrigin2.position + zOffset;
        Instantiate(rbcPrefab, spawnPointPosition, rbcPrefab.transform.rotation);
    }

    public Transform[] getBloodPositions()
    {
        return new Transform[4] { bloodOrigin1, bloodOrigin2, bloodEnd1, bloodEnd2 };
    }
}
