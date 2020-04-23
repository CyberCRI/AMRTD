//#define VERBOSEDEBUG
using UnityEngine;

public class WobblyMovement : MonoBehaviour
{

    [Header("Movement")]
    protected Vector3 target = Vector3.zero;
    private bool isHoldingPosition = false;
    public float startSpeed = 0f;
    protected float speed = 0f;
    private float slowDownRatioFactor = 1f;
    [SerializeField]
    protected float minimumDistance = 0f;

    [Header("Wobble")]
    [SerializeField]
    protected Transform wobbledTransform = null;

    [Header("Wobble position")]
    [Tooltip("Position over time is δt + δ * cos(ωt + ф)")]
    [SerializeField]
    private float wobbleShiftSpeedX = 0f;
    [SerializeField]
    private float wobbleShiftSpeedZ = 0f;
    [SerializeField]
    private float horizontalShift = 0f;
    private float phaseShiftX = 0f;
    private float phaseShiftZ = 0f;

    [Header("Wobble scale")]
    [Tooltip("Scale over time is Si * (1 + Rf * cos(ωt + ф)")]
    [SerializeField]
    private float ratioFactor = 0f;
    [SerializeField]
    private float wobbleScaleSpeed = 0f;
    private float phaseScaleX = 0f;
    private Vector3 initialScale = Vector3.zero;

    [Header("Wobble rotation")]
    [Tooltip("Rotation over time is αi + α * cos(ωt + ф)")]
    [SerializeField]
    private float angularWobble = 0f;
    [SerializeField]
    private float wobbleRotateSpeed = 0f;
    private float phaseRotatY = 0f;
    private Vector3 initialRotation = Vector3.zero;
    
    [Header("Repulsion")]
    [SerializeField]
    protected Rigidbody _rigidbody = null;
    [SerializeField]
    protected float repulsionForce = 5f;
    [SerializeField]
    protected string[] repulsers;

    private const float distanceSecurityRatio = 1.1f;

    protected Vector3 displacement = Vector3.zero;
    private Vector3 sinusoidalShiftVector = Vector3.zero;
    private Vector3 wobbledDisplacement = Vector3.zero;
    private Vector3 temporaryRotationVector3 = Vector3.zero;
    private Vector3 temporaryLocalScaleVector3 = Vector3.zero;

    protected Vector3 vectorToTarget = Vector3.zero;
    protected float distanceTravelled = 0f;
    protected bool hasReachedTarget = false;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null == wobbledTransform)
        {
            wobbledTransform = this.transform;
        }
        resetSpeed();

        phaseShiftX = Random.Range(0f, 2f * Mathf.PI);
        phaseShiftZ = Random.Range(0f, 2f * Mathf.PI);
        phaseRotatY = Random.Range(0f, 2f * Mathf.PI);
        phaseScaleX = Random.Range(0f, 2f * Mathf.PI);

        if (Vector3.zero == initialScale)
        {
            initialScale = wobbledTransform.localScale;
            initialRotation = wobbledTransform.localRotation.eulerAngles;
        }

        minimumDistance = Mathf.Max(distanceSecurityRatio * horizontalShift, minimumDistance);

        onAwakeDone();
    }

    protected virtual void onAwakeDone() { }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    protected virtual void Update()
    {
        #if VERBOSEDEBUG
        Debug.Log(string.Format("{0}: {1}: Update ", this.GetType(), this.gameObject.name));
        #endif

        if (0 != Time.deltaTime)
        {
            #if VERBOSEDEBUG
            Debug.Log(string.Format("{0}: {1}: Update 0 != Time.deltaTime ", this.GetType(), this.gameObject.name));
            #endif

            onUpdateBegins();

            wobble();

            onWobbleDone();

            resetSpeed();
        }
    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        bool collides = false;
        for (int i = 0; i < repulsers.Length; i++)
        {
            if (collider.tag == repulsers[i])
            {
                #if VERBOSEDEBUG
                Debug.Log(this.gameObject.name + ": hit " + collider.tag + " compared to " + repulsers[i]);
                #endif
                collides = true;
                break;
            }
        }
        if (collides)
        {
            Vector3 direction = (this.transform.position - collider.transform.position).normalized;
            _rigidbody.AddForce(direction * repulsionForce, ForceMode.Impulse);
        }
    }

    protected virtual void onUpdateBegins()
    {
        setDisplacement();
    }

    protected virtual void setDisplacement()
    {
        vectorToTarget = (target - this.transform.position);
        distanceTravelled = Mathf.Min(vectorToTarget.magnitude, speed * Time.deltaTime);
        hasReachedTarget = (vectorToTarget.sqrMagnitude < minimumDistance);

        if (isHoldingPosition || hasReachedTarget)
        {
            displacement = Vector3.zero;
        }
        else
        {
            displacement = vectorToTarget.normalized * distanceTravelled;
        }
    }

    private void wobble()
    {
        sinusoidalShiftVector = horizontalShift * new Vector3(
                Mathf.Cos(phaseShiftX + wobbleShiftSpeedX * speed * Time.timeSinceLevelLoad),
                0,
                Mathf.Cos(phaseShiftZ + wobbleShiftSpeedZ * speed * Time.timeSinceLevelLoad)
            );

        wobbledDisplacement = displacement + sinusoidalShiftVector;
        this.transform.Translate(wobbledDisplacement, Space.World);

        temporaryRotationVector3 = new Vector3(
            initialRotation.x,
            initialRotation.y + angularWobble * Mathf.Cos(phaseRotatY + wobbleRotateSpeed * speed * Time.timeSinceLevelLoad),
            initialRotation.z);

        wobbledTransform.localRotation = Quaternion.Euler(temporaryRotationVector3);

        temporaryLocalScaleVector3 = new Vector3(
                initialScale.x * (1 + ratioFactor * Mathf.Cos(phaseScaleX + wobbleScaleSpeed * speed * Time.timeSinceLevelLoad)),
                //initialScale.y,
                initialScale.y * (1 + ratioFactor * Mathf.Cos(phaseScaleX + wobbleScaleSpeed * speed * Time.timeSinceLevelLoad)),
                initialScale.z * (1 + ratioFactor * Mathf.Cos(Mathf.PI + phaseScaleX + wobbleScaleSpeed * speed * Time.timeSinceLevelLoad))
            );

        wobbledTransform.localScale = temporaryLocalScaleVector3;
    }

    protected virtual void onWobbleDone() { }

    private void resetSpeed()
    {
        speed = startSpeed;
        slowDownRatioFactor = 1f;
    }

    public void transferWobbleParametersTo(WobblyMovement otherWM)
    {
        otherWM.setWobbleParameters(initialScale, initialRotation);
    }

    public void setWobbleParameters(
        Vector3 _initialScale
        , Vector3 _initialRotation
        )
    {
        initialScale = _initialScale;
        initialRotation = _initialRotation;
    }

    public void setHoldingPosition(bool value)
    {
        isHoldingPosition = value;
    }

    public void slow(float _slowDownRatioFactor)
    {
        if (slowDownRatioFactor > _slowDownRatioFactor)
        {
            speed = startSpeed * _slowDownRatioFactor;
            slowDownRatioFactor = _slowDownRatioFactor;
        }
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
