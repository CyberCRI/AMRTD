using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{

    [Header("Movement")]
    public float startSpeed = 0f;
    [HideInInspector]
    public float speed = 0f;
    public float minimumDistance = 0f;
    private Vector3 target = Vector3.zero;
    [HideInInspector]
    public int waypointIndex = 0;
    [SerializeField]
    private Waypoints.WaypointsMode waypointsMode = Waypoints.WaypointsMode.CONTINUOUS;
    private Enemy enemy;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        enemy = this.GetComponent<Enemy>();
        speed = startSpeed;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        getNextWaypoint();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Vector3 dir = target - this.transform.position;
        this.transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (dir.magnitude <= minimumDistance)
        {
            getNextWaypoint();
        }

        speed = startSpeed;
    }

    public void slow(float slowRatioFactor)
    {
        speed = startSpeed * (1f - slowRatioFactor);
    }

    void getNextWaypoint()
    {
        target = Waypoints.instance.getWaypoint(waypointIndex++, waypointsMode);
        if (Mathf.Infinity == target.x)
        {
            endPath();
        }
        else
        {
            enemy.onReachedWaypoint(waypointIndex);
        }
    }

    void endPath()
    {
        PlayerStatistics.lives--;
        Destroy(this.gameObject);
    }
}
