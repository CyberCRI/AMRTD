//#define ENEMIES_NEVER_LEAVE

using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : WobblyMovement
{
    private Enemy enemy;

    // waypoints
    [HideInInspector]
    public int waypointIndex = 0;
    [Header("Exclusives")]
    [SerializeField]
    private Waypoints.WaypointsMode waypointsMode = Waypoints.WaypointsMode.CONTINUOUS;

    protected override void onAwakeDone()
    {
        enemy = this.GetComponent<Enemy>();
        if (null != wobbledTransform && null != sphereCollider)
        {
            sphereCollider.radius = wobbledTransform.localScale.x / 2f;
        }

        repulsers = new string[4] {Enemy.enemyTag, RedBloodCellMovement.rbcTag, Virus.virusTag, WhiteBloodCellMovement.wbcTag};
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        getNextWaypoint();
    }

    protected override void onWobbleDone()
    {
        if (hasReachedTarget)
        {
            getNextWaypoint();
        }
    }

    void getNextWaypoint()
    {
        if (null != ObjectiveDefenseMode.instance)
        {
            waypointIndex = ObjectiveDefenseMode.instance.getFreeObjectiveToDefend();
            target = ObjectiveDefenseMode.instance.getFreeSlotPosition();
        }
        else
        {
            target = Waypoints.instance.getWaypoint(waypointIndex++, waypointsMode);
        }
        if (Mathf.Infinity == target.x)
        {
#if ENEMIES_NEVER_LEAVE
            // enemies go back and forth forever
            waypointIndex = 0;
            target = Waypoints.instance.getWaypoint(waypointIndex++, waypointsMode);
#else
            endPath();
#endif
        }
        else
        {
            enemy.onReachedWaypoint(waypointIndex);
        }
    }

    void endPath()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.PATHOGENESCAPES, CustomData.getGameObjectContext(this));
        PlayerStatistics.instance.lives--;
        Destroy(this.gameObject);
    }
}
