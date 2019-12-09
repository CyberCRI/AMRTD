using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public static Waypoints instance = null;

    [SerializeField]
    private Transform waypointsDiscreteRoot = null;
    // procedurally filled list of waypoints
    private Transform[] waypointsDiscreteList = null;
    [SerializeField]
    private Transform waypointsContinuousRoot = null;
    // procedurally filled list of waypoints
    private Transform[] waypointsContinuousList = null;

    public enum WaypointsMode
    {
        CONTINUOUS,
        DISCRETE
    };

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

            CommonUtilities.fillArrayFromRoot(waypointsDiscreteRoot, ref waypointsDiscreteList);
            CommonUtilities.fillArrayFromRoot(waypointsContinuousRoot, ref waypointsContinuousList);
        }
    }

    public Vector3 getWaypoint(int depth, WaypointsMode waypointsMode = WaypointsMode.CONTINUOUS)
    {
        Vector3 waypoint = Vector3.zero;
        switch (waypointsMode)
        {
            case WaypointsMode.CONTINUOUS:
                if (depth >= waypointsContinuousList.Length)
                {
                    waypoint = Vector3.positiveInfinity;
                }
                else
                {
                    float t = Random.Range(0f, 1f);
                    waypoint = t * waypointsContinuousList[depth].GetChild(0).position
                    + (1 - t) * waypointsContinuousList[depth].GetChild(1).position;
                }
                break;
            case WaypointsMode.DISCRETE:
            default:
                if (depth >= waypointsDiscreteList.Length)
                {
                    waypoint = Vector3.positiveInfinity;
                }
                else
                {
                    waypoint = waypointsDiscreteList[depth].position;
                }
                break;
        }
        return waypoint;
    }
}
