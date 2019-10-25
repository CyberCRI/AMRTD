using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Enemy enemy;
    private Transform target = null;
    private int waypointIndex = 0;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        enemy = this.GetComponent<Enemy>();
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
        Vector3 dir = target.position - this.transform.position;
        this.transform.Translate(dir.normalized * enemy.speed * Time.deltaTime, Space.World);

        if (dir.magnitude <= enemy.minimumDistance)
        {
            getNextWaypoint();
        }

        enemy.speed = enemy.startSpeed;
    }

    void getNextWaypoint()
    {
        if (waypointIndex >= Waypoints.waypoints.Length)
        {
            endPath();
        }
        else
        {
            target = Waypoints.waypoints[waypointIndex++];
        }
    }

    void endPath()
    {
        PlayerStatistics.lives--;
        Destroy(this.gameObject);
    }
}
