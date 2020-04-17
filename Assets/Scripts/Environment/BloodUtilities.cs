//#define VERBOSEDEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodUtilities : MonoBehaviour
{
    public static BloodUtilities instance = null;

    [SerializeField]
    private Transform _bloodOrigin1 = null;
    public Transform bloodOrigin1 { get { return _bloodOrigin1; } }
    [SerializeField]
    private Transform _bloodOrigin2 = null;
    public Transform bloodOrigin2 { get { return _bloodOrigin2; } }
    [SerializeField]
    private Transform _bloodEnd1 = null;
    public Transform bloodEnd1 { get { return _bloodEnd1; } }
    [SerializeField]
    private Transform _bloodEnd2 = null;
    public Transform bloodEnd2 { get { return _bloodEnd2; } }
    [SerializeField]
    private Transform _bloodUnder = null;
    public Transform bloodUnder { get { return _bloodUnder; } }
    
    [SerializeField]
    private Transform _bloodWayPointsRoot = null;
    [SerializeField]
    private Transform[] _bloodWayPoints = null;
    public Transform[] bloodWayPoints { get { return _bloodWayPoints; } }
    public bool isWaypointsBased { get; private set; } = false;

    [SerializeField]
    private Transform _idlePositionsRoot = null;
    private Transform[] _idlePositions = null;
    public Transform[] idlePositions { get { return _idlePositions; } }
    public bool isManualIdlePositions { get; private set; } = false;


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

            if ((null != _bloodWayPointsRoot) && _bloodWayPointsRoot.gameObject.activeSelf)
            {
                isWaypointsBased = true;
                CommonUtilities.fillArrayFromRoot(_bloodWayPointsRoot, ref _bloodWayPoints);
            }

            if ((null != _idlePositionsRoot) && _idlePositionsRoot.gameObject.activeSelf)
            {
                isManualIdlePositions = true;
                CommonUtilities.fillArrayFromRoot(_idlePositionsRoot, ref _idlePositions);
            }
        }
    }
}
