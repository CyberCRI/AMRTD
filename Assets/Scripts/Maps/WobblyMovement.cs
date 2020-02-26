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

    protected Vector3 displacement = Vector3.zero;
    private Vector3 sinusoidalShiftVector = Vector3.zero;
    private Vector3 wobbledDisplacement = Vector3.zero;
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
        if (0 != Time.deltaTime)
        {
            wobble();

            onWobbleDone();

            resetSpeed();
        }
    }

    protected virtual void onWobbleDone() { }

    private void resetSpeed()
    {
        speed = startSpeed;
        slowDownRatioFactor = 1f;
    }

    public void setWobbleParameters(
        Vector3 _initialScale
        , Vector3 _initialRotation
        )
    {
        initialScale = _initialScale;
        initialRotation = _initialRotation;
    }

    public void transferWobbleParametersTo(WobblyMovement otherWM)
    {
        otherWM.setWobbleParameters(initialScale, initialRotation);
    }

    public void setHoldingPosition(bool value)
    {
        isHoldingPosition = value;
    }

    protected virtual void setDisplacement()
    {
        displacement = isHoldingPosition ? Vector3.zero : (target - this.transform.position).normalized * speed * Time.deltaTime;
    }

    private void wobble()
    {
        setDisplacement();
        sinusoidalShiftVector = horizontalShift * new Vector3(
                Mathf.Cos(
                    phaseShiftX +
                    wobbleShiftSpeedX * speed * Time.timeSinceLevelLoad),
                0,
                Mathf.Cos(
                    phaseShiftZ +
                    wobbleShiftSpeedZ * speed * Time.timeSinceLevelLoad)
                    );

        wobbledDisplacement = displacement + sinusoidalShiftVector;
        this.transform.Translate(wobbledDisplacement, Space.World);

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
