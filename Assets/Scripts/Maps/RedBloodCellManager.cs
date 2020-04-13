//#define DETRIMENTALOPTIMIZATION
//#define VERBOSEDEBUG

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
    private GameObject[] rbcPrefabs = null;

    [SerializeField]
    private int rbcSpawnCount = 0;
    private float rbcSpawnTimePeriod = 0f;
    [SerializeField]
    private float rbcSpawnTimePeriodVariationRatio = 0f;
    [SerializeField]
    private float rbcSpawnSpatialPeriodVariation = 0f;
#if DETRIMENTALOPTIMIZATION
    private int rbcYetToSpawnCount = 0;
#endif
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

            
            float topToBottom = 0f;

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
                #if VERBOSEDEBUG
                Debug.Log(string.Format("{0} rbcSpawnSpatialPeriod=={1}", this.GetType(),rbcSpawnSpatialPeriod));
                #endif
                float passedPath = 0f;
                
                int passedPathIndex = 0;
                int rbcPositionsFound = 0;
                float remainder = 0f; // positive value
                float randomEpsilon = 0f;

                #if VERBOSEDEBUG
                Debug.Log(this.GetType() + " MAIN spatial spawn loop starts...");
                #endif

                int mainCrashController = 0;
                int subCrashController = 0;

                float distanceThisIteration = 0f;
                int rbcPositionsFoundThisIteration = 0;

                float t = 0f;

                Vector3 spawnPosition;
                string rbcIndex;
                
                while ((mainCrashController++ < 100) && (rbcPositionsFound < rbcSpawnCount))
                //for (int i = 1; i < rbcSpawnCount+1; i++)
                {
                    #if VERBOSEDEBUG
                    Debug.Log(string.Format("{0} > spatial spawn loop: (1/3-START) rbcPositionsFound=={1} < rbcSpawnCount=={2}", this.GetType(),rbcPositionsFound, rbcSpawnCount));
                    #endif
                    
                    subCrashController = 0;
                    while ((subCrashController++ < 100) && (remainder < rbcSpawnSpatialPeriod))
                    {
                        #if VERBOSEDEBUG
                        Debug.Log(
                            string.Format("{0} > > spatial spawn subloop1: remainder=={1} <= rbcSpawnSpatialPeriod=={2};"
                                            + " passedPathIndex=={3}; distances[passedPathIndex]={4}; passedPath=={5}"
                            , this.GetType(), remainder, rbcSpawnSpatialPeriod, passedPathIndex, distances[passedPathIndex], passedPath));
                        #endif
                        passedPath += distances[passedPathIndex];
                        remainder += distances[passedPathIndex];
                        passedPathIndex++;
                    }
                    
                    rbcPositionsFoundThisIteration = 0;
                    //while (rbcPositionsFound * rbcSpawnSpatialPeriod < passedPath)
                    distanceThisIteration = distances[passedPathIndex-1];
                    remainder -= distanceThisIteration;
                    
                    #if VERBOSEDEBUG
                    Debug.Log(string.Format("{0} > spatial spawn loop: (2/3-INTERLOOP) remainder=={1}, passedPath=={2}, passedPathIndex=={3}, subCrashController=={4}"
                        , this.GetType(), remainder, passedPath, passedPathIndex, subCrashController));
                    #endif

                    subCrashController = 0;
                    while ((subCrashController++ < 100) && ((rbcPositionsFoundThisIteration + 1) * rbcSpawnSpatialPeriod - remainder <= distanceThisIteration))
                    {
                        rbcPositionsFoundThisIteration++;
                        t = ((rbcPositionsFoundThisIteration * rbcSpawnSpatialPeriod) - remainder) / distanceThisIteration;
                        #if VERBOSEDEBUG
                        //Debug.Log(string.Format("{0} > > spatial spawn loop: rbcPositionsFound * rbcSpawnSpatialPeriod=={1} < passedPath=={2}; rbcPositionsFound=={3}"
                        //    , this.GetType(), rbcPositionsFound * rbcSpawnSpatialPeriod, passedPath, rbcPositionsFound));
                        Debug.Log( string.Format( "{0} > > spatial spawn subloop2: t=={1};"
                            + " (rbcPositionsFoundThisIteration + 1) * rbcSpawnSpatialPeriod - remainder=={2} "
                            + " < distanceThisIteration=={3}"
                            , this.GetType(), t, (rbcPositionsFoundThisIteration + 1) * rbcSpawnSpatialPeriod - remainder, distanceThisIteration));
                        #endif
                        //float t = ((rbcPositionsFound+1) * rbcSpawnSpatialPeriod) / distances[passedPathIndex-1];
                        randomEpsilon = rbcSpawnSpatialPeriodVariation * Random.Range(-1f, 1f);
                        t = Mathf.Clamp(t + randomEpsilon, 0f, 1f);
                        spawnPosition =  (1-t) * bloodWayPoints[passedPathIndex-1].position + t * bloodWayPoints[passedPathIndex].position;
                        rbcIndex = (rbcPositionsFound + rbcPositionsFoundThisIteration).ToString();
                        
                        innerSpawnRBCPosition(spawnPosition, passedPathIndex, "RBC" + rbcIndex);

                        #if VERBOSEDEBUG
                        createDebugObject(spawnPosition, "RBC" + rbcIndex + "copy", 3f);

                        createDebugObject(bloodWayPoints[passedPathIndex-1].position, string.Format("bwp[{0}][{1}]", rbcIndex, (passedPathIndex-1).ToString()));
                        createDebugObject(bloodWayPoints[passedPathIndex].position, string.Format("bwp[{0}][{1}]",   rbcIndex, passedPathIndex.ToString()));
                        #endif
                    }
                    rbcPositionsFound += rbcPositionsFoundThisIteration;
                    remainder = passedPath - (rbcPositionsFound * rbcSpawnSpatialPeriod);
                    if (remainder >= rbcSpawnSpatialPeriod)
                    {
                        Debug.LogError(string.Format( "remainder=={0} >= rbcSpawnSpatialPeriod=={1}", remainder, rbcSpawnSpatialPeriod));
                    }
                    #if VERBOSEDEBUG
                    Debug.Log(string.Format("{0} > spatial spawn loop (3/3-END) rbcPositionsFound=={1},"
                        + " rbcPositionsFoundThisIteration=={2},"
                        + " remainder=={3}, subCrashController=={4}"
                        , this.GetType(), rbcPositionsFound, rbcPositionsFoundThisIteration, remainder, subCrashController));
                    #endif
                }
                #if VERBOSEDEBUG
                Debug.Log(this.GetType() + " ...MAIN spatial spawn loop done!");
                Debug.Log(string.Format("{0} MAIN spatial spawn loop: rbcPositionsFound=={1}, rbcSpawnCount=={2}, mainCrashController=={3}"
                    , this.GetType(),rbcPositionsFound, rbcSpawnCount, mainCrashController));
                #endif
                Time.timeScale = 0f;
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

    private void createDebugObject(Vector3 position, string _name, float scale = 1f)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = scale * Vector3.one;
        sphere.name = _name;
    }

    private void randomSpawnRBC()
    {
        Invoke("spawnRBC", rbcSpawnTimePeriodVariationRatio * Random.value * rbcSpawnTimePeriod);
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
        float t = Random.value;
        Vector3 spawnPointPosition = t * bloodOrigin1.position + (1 - t) * bloodOrigin2.position + zOffset;
        innerSpawnRBCPosition(spawnPointPosition);
    }

    private void innerSpawnRBCPosition(Vector3 spawnPointPosition, int waypointIndex = -1, string rbcName = null)
    {
        GameObject rbcPrefab = rbcPrefabs[Random.Range(0, rbcPrefabs.Length)];
        GameObject go = Instantiate(rbcPrefab, spawnPointPosition, rbcPrefab.transform.rotation);
        #if VERBOSEDEBUG
        if (!string.IsNullOrEmpty(rbcName))
        {
            go.name = rbcName;
        }
        #endif
        if (waypointIndex > 0)
        {
            RedBloodCellMovement rbcm = go.GetComponent<RedBloodCellMovement>();
            rbcm.setTarget(waypointIndex);
            #if VERBOSEDEBUG
            Debug.Log(string.Format("{0} innerSpawnRBCPosition rbc '{1}' -> waypoint {2}", this.GetType(), rbcName, waypointIndex));
            #endif
        }
    }

    public Transform[] getBloodPositions()
    {
        return new Transform[4] { bloodOrigin1, bloodOrigin2, bloodEnd1, bloodEnd2 };
    }
}
