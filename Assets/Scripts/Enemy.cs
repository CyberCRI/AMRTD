using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int waypointIndex = 0;
    [SerializeField]
    private float speed = 0f;
    [SerializeField]
    private float minimumDistance = 0f;
    [SerializeField]
    private ParticleSystem deathEffect = null;
    public const string enemyTag = "EnemyTag";

    [SerializeField]
    private int health = 100;
    [SerializeField]
    private int reward = 50;

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
            endPath();
        }
        else
        {
            target = Waypoints.waypoints[++waypointIndex];
        }
    }

    void endPath()
    {
        PlayerStatistics.lives--;
        Destroy(this.gameObject);
    }

    public void takeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            die();
        }
    }

    private void die()
    {
        GameObject effect = (GameObject)Instantiate(deathEffect.gameObject, this.transform.position, Quaternion.identity);
        Destroy(effect.gameObject, deathEffect.main.duration + deathEffect.main.startLifetime.constant);
        PlayerStatistics.money += reward;
        Destroy(this.gameObject);
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        Debug.Log("Enemy destroyed");
    }
}
