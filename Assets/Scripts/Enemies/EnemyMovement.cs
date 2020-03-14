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
    [SerializeField]
    private SphereCollider sphereCollider = null;
    [SerializeField]
    private Rigidbody _rigidbody = null;
    private float repulsionForce = 5f;

    protected override void onAwakeDone()
    {
        enemy = this.GetComponent<Enemy>();
        if (null != wobbledTransform && null != sphereCollider)
        {
            sphereCollider.radius = wobbledTransform.localScale.x / 2f;
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        getNextWaypoint();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == Enemy.enemyTag)
        {
            Vector3 direction = (this.transform.position - collider.transform.position).normalized;
            _rigidbody.AddForce(direction * repulsionForce, ForceMode.Impulse);
        }
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
        PlayerStatistics.instance.lives--;
        Destroy(this.gameObject);
    }
}
