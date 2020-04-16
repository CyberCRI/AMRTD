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
    public Transform bloodWayPointsRoot { get { return _bloodWayPointsRoot; } }

    [SerializeField]
    private Transform[] _bloodWayPoints = null;
    public Transform[] bloodWayPoints { get { return _bloodWayPoints; } }

    public bool isWaypointsBased { get; private set; } = false;

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

            if ((null != bloodWayPointsRoot) && bloodWayPointsRoot.gameObject.activeSelf)
            {
                isWaypointsBased = true;
                CommonUtilities.fillArrayFromRoot(bloodWayPointsRoot, ref _bloodWayPoints);
            }
        }
    }
}
