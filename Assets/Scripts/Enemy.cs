using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int waypointIndex = 0;
    [SerializeField]
    private float speed = 0f;
    [SerializeField]
    private float minimumDistance = 0f;

    private Transform target = null;

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
        Vector3 dir = target.position - this.transform.position;
        this.transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (dir.magnitude <= minimumDistance)
        {
            getNextWaypoint();
        }
    }

    void getNextWaypoint()
    {
        if (waypointIndex >= Waypoints.waypoints.Length - 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            target = Waypoints.waypoints[++waypointIndex];
        }
    }
}
