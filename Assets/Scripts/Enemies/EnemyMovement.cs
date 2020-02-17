//#define ENEMIES_NEVER_LEAVE

using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Enemy enemy;

    [Header("Movement")]
    private bool isHoldingPosition = false;
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
    private Vector3 initialScale = Vector3.zero;
    private Vector3 initialRotation = Vector3.zero;
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

    private Vector3 displacement = Vector3.zero;
    private Vector3 sinusoidalShiftVector = Vector3.zero;
    private Vector3 temporaryDisplacementVector3 = Vector3.zero;
    private Vector3 temporaryVector3 = Vector3.zero;
    private Vector3 temporaryLocalScaleVector3 = Vector3.zero;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null == wobbledTransform)
        {
            wobbledTransform = this.transform;
        }
        enemy = this.GetComponent<Enemy>();
        speed = startSpeed;

        phaseShiftX = Random.Range(0, 2 * Mathf.PI);
        phaseShiftZ = Random.Range(0, 2 * Mathf.PI);
        phaseRotatY = Random.Range(0, 2 * Mathf.PI);
        phaseScaleX = Random.Range(0, 2 * Mathf.PI);

        if (Vector3.zero == initialScale)
        {
            initialScale = wobbledTransform.localScale;
            initialRotation = wobbledTransform.localRotation.eulerAngles;
        }

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

    public void setWobbleParameters(
        Vector3 _initialScale
        , Vector3 _initialRotation
        )
    {
        initialScale = _initialScale;
        initialRotation = _initialRotation;
    }

    public void transferWobblingParametersTo(EnemyMovement otherEM)
    {
        otherEM.setWobbleParameters(initialScale, initialRotation);
    }

    public void setHoldingPosition(bool value)
    {
        isHoldingPosition = value;
    }

    private void wobble()
    {
        displacement = isHoldingPosition ? Vector3.zero : (target - this.transform.position).normalized * speed * Time.deltaTime;
        sinusoidalShiftVector = horizontalShift * new Vector3(
                Mathf.Cos(
                    phaseShiftX +
                    wobbleShiftSpeedX * speed * Time.timeSinceLevelLoad),
                0,
                Mathf.Cos(
                    phaseShiftZ +
                    wobbleShiftSpeedZ * speed * Time.timeSinceLevelLoad)
                    );

        temporaryDisplacementVector3 = displacement + sinusoidalShiftVector;
        this.transform.Translate(temporaryDisplacementVector3, Space.World);

        temporaryVector3 = new Vector3(
            initialRotation.x,
            initialRotation.y + angularWobble * Mathf.Cos(
                phaseRotatY +
                wobbleRotateSpeed * speed * Time.timeSinceLevelLoad),
            initialRotation.z);

        wobbledTransform.localRotation = Quaternion.Euler(temporaryVector3);

        temporaryLocalScaleVector3 = new Vector3(
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

        wobbledTransform.localScale = temporaryLocalScaleVector3;
    }

    public void slow(float slowRatioFactor)
    {
        speed = startSpeed * slowRatioFactor;
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

    public void debugFields()
    {
        Debug.Log(
            "debugFields on GO " + this.gameObject.name
            + "\nstartSpeed=" + startSpeed
            + "\nspeed=" + speed
            + "\nwobbleScaleSpeed=" + wobbleScaleSpeed
            + "\nwobbleRotateSpeed=" + wobbleRotateSpeed
            + "\nwobbleShiftSpeedX=" + wobbleShiftSpeedX
            + "\nwobbleShiftSpeedZ=" + wobbleShiftSpeedZ
            + "\ninitialScale=" + initialScale
            + "\ninitialRotation=" + initialRotation
            + "\nscaleFactor=" + scaleFactor
            + "\nratioFactor=" + ratioFactor
            + "\nangularWobble=" + angularWobble
            + "\nhorizontalShift=" + horizontalShift
            + "\nphaseShiftX=" + phaseShiftX
            + "\nphaseShiftZ=" + phaseShiftZ
            + "\nphaseRotatY=" + phaseRotatY
            + "\nphaseScaleX=" + phaseScaleX
            );
    }
}
