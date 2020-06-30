//#define ENEMIES_NEVER_LEAVE
using System.Collections;
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
    protected SphereCollider sphereCollider = null;
    [SerializeField]
    private float absorptionImpulse = 50f;
    [SerializeField]
    private float sqrMagnitudeProximityThreshold = 3f;
    [SerializeField]
    private float jumpPeriod = .7f;
    private float jumpCountdown = 0f;

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
        AudioManager.instance.play(AudioEvent.PATHOGENESCAPES);
        PlayerStatistics.instance.lives--;
        Destroy(this.gameObject);
    }
    
    private void kickToPosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - this.transform.position).normalized;
        _rigidbody.AddForce(direction * absorptionImpulse, ForceMode.Impulse);
    }

    private IEnumerator jumpToTransform(Transform targetTransform)
    {
        this.setHoldingPosition(true);
        sphereCollider.enabled = false;
        while ((this.transform.position - targetTransform.position).sqrMagnitude > sqrMagnitudeProximityThreshold)
        {
            jumpCountdown -= Time.deltaTime;
            if (jumpCountdown <= 0)
            {
                kickToPosition(targetTransform.position);
                jumpCountdown = jumpPeriod;
            }
            yield return 0;
        }
        Destroy(this.gameObject);
    }

    public void getAbsorbed(Transform absorberTransform)
    {
        StartCoroutine(jumpToTransform(absorberTransform));
    }

    public AudioSource play(AudioEvent audioEvent, string parameter = "", bool doPlay = true, AudioClip dontReplay = null)
    {
        return enemy.play(audioEvent, parameter, doPlay, dontReplay);
    }
}
