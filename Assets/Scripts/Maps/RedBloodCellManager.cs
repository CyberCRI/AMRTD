//#define DETRIMENTALOPTIMIZATION
#define VERBOSEDEBUG

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
    private Transform bloodWayPointsRoot = null;
    public Transform[] bloodWayPoints = null;
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
    public bool isWaypointsBased = false;

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

            isWaypointsBased = ((null != bloodWayPointsRoot) && bloodWayPointsRoot.gameObject.activeSelf);

#if DETRIMENTALOPTIMIZATION
            rbcYetToSpawnCount = rbcSpawnCount;
#endif

            if (isWaypointsBased)
            {
                CommonUtilities.fillArrayFromRoot(bloodWayPointsRoot, ref bloodWayPoints);
                float[] distances = new float[bloodWayPoints.Length];

                // spawn all along the waypoints
                topToBottom = 0f;

                for (int i = 0; i < bloodWayPoints.Length-1; i++)
                {
                    distances[i] = (bloodWayPoints[i].position - bloodWayPoints[i+1].position).magnitude;
                    topToBottom += distances[i];
                }
                
                // spatial spawn - find between which waypoints to spawn
                float rbcSpawnSpatialPeriod = topToBottom / (rbcSpawnCount + 1);
                float passedPath = 0f;
                int passedPathIndex = 0;
                int rbcPositionsFound = 0;
                bool done = false;
#if VERBOSEDEBUG
                Debug.Log(this.GetType() + " entering spatial spawn loop...");
#endif
                while (rbcPositionsFound < rbcSpawnCount)
                //for (int i = 1; i < rbcSpawnCount+1; i++)
                {
#if VERBOSEDEBUG
                    Debug.Log(string.Format("{0} > spatial spawn loop: rbcPositionsFound=={1} < rbcSpawnCount=={2}", this.GetType(),rbcPositionsFound, rbcSpawnCount));
#endif
                    while (passedPath <= rbcPositionsFound * rbcSpawnSpatialPeriod)
                    {
#if VERBOSEDEBUG
                        Debug.Log(string.Format("{0} > > spatial spawn loop: passedPath=={1} <= rbcPositionsFound * rbcSpawnSpatialPeriod=={2}; passedPathIndex=={3}"
                            , this.GetType(), passedPath, rbcPositionsFound * rbcSpawnSpatialPeriod, passedPathIndex));
#endif
                        passedPath += distances[passedPathIndex++];
                    }
#if VERBOSEDEBUG
                        Debug.Log(string.Format("{0} > spatial spawn loop: interloop: passedPathIndex=={1}"
                            , this.GetType(), passedPathIndex));
#endif
                    while (rbcPositionsFound * rbcSpawnSpatialPeriod < passedPath)
                    {
#if VERBOSEDEBUG
                        Debug.Log(string.Format("{0} > > spatial spawn loop: rbcPositionsFound * rbcSpawnSpatialPeriod=={1} < passedPath=={2}; rbcPositionsFound=={3}"
                            , this.GetType(), rbcPositionsFound * rbcSpawnSpatialPeriod, passedPath, rbcPositionsFound));
#endif
                        float t = ((rbcPositionsFound+1) * rbcSpawnSpatialPeriod) / distances[passedPathIndex-1];
                        Vector3 spawnPosition =  t * bloodWayPoints[passedPathIndex-1].position + (1-t) * bloodWayPoints[passedPathIndex].position;
                        innerSpawnRBCPosition(spawnPosition);
                        rbcPositionsFound++;
                    }
#if VERBOSEDEBUG
                        Debug.Log(string.Format("{0} > spatial spawn loop: end of loop: rbcPositionsFound=={1}"
                            , this.GetType(), rbcPositionsFound));
#endif
                }
#if VERBOSEDEBUG
                Debug.Log(this.GetType() + " ...spatial spawn loop done!");
                Debug.Log(string.Format("{0} spatial spawn loop: rbcPositionsFound=={1}, rbcSpawnCount=={2}"
                    , this.GetType(),rbcPositionsFound, rbcSpawnCount));
#endif
            }
            else
            {
                topToBottom = (bloodEnd1.position - bloodOrigin1.position).z;

                // spatial spawn
                Vector3 rbcSpawnSpatialPeriod = topToBottom / (rbcSpawnCount + 1) * Vector3.forward;
                for (int i = 0; i < rbcSpawnCount; i++)
                {
                    innerSpawnRBC((i + 1) * rbcSpawnSpatialPeriod);
                }
            }
                
            rbcSpawnTimePeriod = Mathf.Abs(topToBottom / (rbcSpawnCount * rbcPrefabs[0].GetComponent<RedBloodCellMovement>().baseSpeed));

#if !DETRIMENTALOPTIMIZATION
            // temporal spawn
            for (int i = 0; i < rbcSpawnCount; i++)
            {
                // temporal spawn for DETRIMENTALOPTIMIZATION mode if no spatial spawn
                //Invoke("spawnRBC", (i + rbcSpawnPeriodVariationRatio * Random.Range(-1f, 1f)) * rbcSpawnPeriod);

                InvokeRepeating("randomSpawnRBC", i * rbcSpawnTimePeriod, rbcSpawnCount * rbcSpawnTimePeriod);
            }
#endif
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
        float t = Random.Range(0f, 1f);
        Vector3 spawnPointPosition = t * bloodOrigin1.position + (1 - t) * bloodOrigin2.position + zOffset;
        innerSpawnRBCPosition(spawnPointPosition);
    }

    private void innerSpawnRBCPosition(Vector3 spawnPointPosition)
    {
        GameObject rbcPrefab = rbcPrefabs[Random.Range(0, rbcPrefabs.Length)];
        Instantiate(rbcPrefab, spawnPointPosition, rbcPrefab.transform.rotation);
    }

    public Transform[] getBloodPositions()
    {
        return new Transform[4] { bloodOrigin1, bloodOrigin2, bloodEnd1, bloodEnd2 };
    }
}
