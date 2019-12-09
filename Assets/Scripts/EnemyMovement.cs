#define ENEMIES_NEVER_LEAVE

using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Enemy enemy;

    [Header("Movement")]
    public float startSpeed = 0f;
    private float speed = 0f;
    [SerializeField]
    private float minimumDistance = 0f;

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
    private float phaseShiftX = 0f;
    private float phaseShiftZ = 0f;
    private float phaseRotatY = 0f;
    private float phaseScaleX = 0f;
    private const float distanceSecurityRatio = 1.1f;

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
        phaseShiftX = Random.Range(0, 2 * Mathf.PI);
        phaseShiftZ = Random.Range(0, 2 * Mathf.PI);
        phaseRotatY = Random.Range(0, 2 * Mathf.PI);
        phaseScaleX = Random.Range(0, 2 * Mathf.PI);

        minimumDistance = Mathf.Max(distanceSecurityRatio * horizontalShift, minimumDistance);
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
            /*
            switch(GameManager.instance.gameMode)
            {
                case GameManager.GAMEMODE.PATHS:
            */
            wobble();

            if ((target - this.transform.position).magnitude <= minimumDistance)
            {
                getNextWaypoint();
            }
            /*

                    break;

                case GameManager.GAMEMODE.DEFEND_CAPTURABLE_OBJECTIVES:

                    break;
            }
            */

            speed = startSpeed;
        }
    }

    private void wobble()
    {
        Vector3 displacement = (target - this.transform.position).normalized * speed * Time.deltaTime;
        Vector3 sinusoidalShiftVector = horizontalShift * new Vector3(
                Mathf.Cos(
                    phaseShiftX +
                    wobbleShiftSpeedX * speed * Time.timeSinceLevelLoad),
                0,
                Mathf.Cos(
                    phaseShiftZ +
                    wobbleShiftSpeedZ * speed * Time.timeSinceLevelLoad)
                    );
        this.transform.Translate(displacement + sinusoidalShiftVector, Space.World);

        wobbledTransform.localRotation = Quaternion.Euler(new Vector3(
            initialRotation.x,
            initialRotation.y + angularWobble * Mathf.Cos(
                phaseRotatY +
                wobbleRotateSpeed * speed * Time.timeSinceLevelLoad),
            initialRotation.z));

        wobbledTransform.localScale = new Vector3(
            ratioFactor * initialScale.x * (scaleFactor + Mathf.Cos(
                phaseScaleX +
                wobbleScaleSpeed * speed * Time.timeSinceLevelLoad)),
            //initialScale.y,
            ratioFactor * initialScale.y * (scaleFactor + Mathf.Cos(
                phaseScaleX +
                wobbleScaleSpeed * speed * Time.timeSinceLevelLoad)),
            ratioFactor * initialScale.z * (scaleFactor + Mathf.Cos(
                Mathf.PI + phaseScaleX +
                wobbleScaleSpeed * speed * Time.timeSinceLevelLoad)));
    }

    public void slow(float slowRatioFactor)
    {
        speed = startSpeed * slowRatioFactor;
    }

    void getNextWaypoint()
    {
        target = Waypoints.instance.getWaypoint(waypointIndex++, waypointsMode);
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
        PlayerStatistics.lives--;
        Destroy(this.gameObject);
    }
}
