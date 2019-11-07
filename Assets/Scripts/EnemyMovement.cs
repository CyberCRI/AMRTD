using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Enemy enemy;

    [Header("Movement")]
    public float startSpeed = 0f;
    [HideInInspector]
    public float speed = 0f;
    public float minimumDistance = 0f;

    // waypoints
    private Vector3 target = Vector3.zero;
    [HideInInspector]
    public int waypointIndex = 0;
    [SerializeField]
    private Waypoints.WaypointsMode waypointsMode = Waypoints.WaypointsMode.CONTINUOUS;

    [Header("Wobble")]
    [SerializeField]
    private Transform wobbledTransform = null;
    [SerializeField]
    private float wobbleScaleSpeed = 0f;
    [SerializeField]
    private float wobbleRotateSpeed = 0f;
    [SerializeField]
    private float wobbleShiftSpeedX = 0f;
    [SerializeField]
    private float wobbleShiftSpeedZ = 0f;
    private Vector3 initialScale;
    private Vector3 initialRotation;
    // ratioFactor = 1/(scaleFactor +1)
    // scaleFactor = -1 + 1/ratioFactor
    [SerializeField]
    private float scaleFactor = 0f;
    [SerializeField]
    private float ratioFactor = 0f;
    [SerializeField]
    private float angularWobble = 0f;
    [SerializeField]
    private float horizontalShift = 0f;
    private float phase = 0f;
    private float phase2 = 0f;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null == wobbledTransform)
        {
            wobbledTransform = this.transform;
        }
        initialScale = wobbledTransform.localScale;
        initialRotation = wobbledTransform.localRotation.eulerAngles;
        enemy = this.GetComponent<Enemy>();
        speed = startSpeed;
        phase = Random.Range(0, 2 * Mathf.PI);
        phase2 = Random.Range(0, 2 * Mathf.PI);
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
        if (0 != Time.deltaTime)
        {
            Vector3 dir = target - this.transform.position;

            Vector3 displacement = dir.normalized * speed * Time.deltaTime;
            Vector3 sinusoidalShiftVector = horizontalShift * new Vector3(
                    Mathf.Cos(
                        phase +
                        wobbleShiftSpeedX * speed * Time.timeSinceLevelLoad),
                    0,
                    Mathf.Cos(
                        phase2 +
                        wobbleShiftSpeedZ * speed * Time.timeSinceLevelLoad)
                        );
            this.transform.Translate(displacement + sinusoidalShiftVector, Space.World);

            wobbledTransform.localRotation = Quaternion.Euler(new Vector3(
                initialRotation.x,
                initialRotation.y + angularWobble * Mathf.Cos(
                    phase +
                    wobbleRotateSpeed * speed * Time.timeSinceLevelLoad),
                initialRotation.z));

            wobbledTransform.localScale = new Vector3(
                ratioFactor * initialScale.x * (scaleFactor + Mathf.Cos(
                    phase +
                    wobbleScaleSpeed * speed * Time.timeSinceLevelLoad)),
                //initialScale.y,
                ratioFactor * initialScale.y * (scaleFactor + Mathf.Cos(
                    phase +
                    wobbleScaleSpeed * speed * Time.timeSinceLevelLoad)),
                ratioFactor * initialScale.z * (scaleFactor + Mathf.Cos(
                    Mathf.PI + phase +
                    wobbleScaleSpeed * speed * Time.timeSinceLevelLoad)));

            if (dir.magnitude <= minimumDistance)
            {
                getNextWaypoint();
            }

            speed = startSpeed;
        }
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

            // to have enemies go back and forth forever
            //waypointIndex = 0;
            //target = Waypoints.instance.getWaypoint(waypointIndex++, waypointsMode);
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
