//#define VERBOSEDEBUG
using UnityEngine;
using System.Collections;

// TODO
// - degradation
// - propagation to other alveoli groups
// - leucocyte action

public class Virus : WobblyMovement
{
    [Header("Parameters")]
    public const string virusTag = "VirusTag";
    private STATUS status = STATUS.SEARCHING_CELL;

    [SerializeField]
    private int _virionsSpawnCountPerLungCell = 2;
    public int virionsSpawnCountPerLungCell { get { return _virionsSpawnCountPerLungCell; } }
    // lungCellRecoveryProbability * 100 % chances of not dying when spawn ends
    [SerializeField]
    private float _lungCellRecoveryProbability = .6f;
    public float lungCellRecoveryProbability { get { return _lungCellRecoveryProbability; } }
    // damage ratios inflected; ratio of max health
    [SerializeField]
    private float _damageRatioPerInfection = .4f;
    public float damageRatioPerInfection { get { return _damageRatioPerInfection; } }
    [SerializeField]
    private float _damageRatioPerSpawn = .1f;
    public float damageRatioPerSpawn { get { return _damageRatioPerSpawn; } }
    [SerializeField]
    private float _timeBeforeSpawnStarts = 1f;
    public float timeBeforeSpawnStarts { get { return _timeBeforeSpawnStarts; } }
    [SerializeField]
    private float _timeBeforeRecoveryStarts = .5f;
    public float timeBeforeRecoveryStarts { get { return _timeBeforeRecoveryStarts; } }
    [SerializeField]
    private float _timeBetweenSpawns = 1f;
    public float timeBetweenSpawns { get { return _timeBetweenSpawns; } }

    [SerializeField]
    private float jumpPeriod = .7f;
    private float jumpCountdown = 0f;
    private float startImpulse = 20f;
    [SerializeField]
    private float sqrMagnitudeProximityThreshold = 0f;
    [SerializeField]
    private float absorptionImpulse = 0f;
    [SerializeField]
    private float gainAltitudeImpulse = 10f;
    [SerializeField]
    protected SphereCollider sphereCollider = null;
    private bool isEscaping  = false;
    private bool isEscaping1 = false;
    private bool isEscaping2 = false;
    private Vector3 _escapeTarget;
    private Vector3 _waypoint;
    private bool isEntering = false;
    private float _correctAltitude = 0f;

    [SerializeField]
    private Pneumocyte targetPneumocyte = null;

    private enum STATUS
    {
        SEARCHING_CELL,
        GOING_TO_CELL,
    }
    
    // temp variables
    Pneumocyte closestPC = null;
    float sqrMagnitudeToClosestPC = Mathf.Infinity;
    int pcIndex = 0;
    int j = 0;
    float _sqrMagnitude = 0f;



    void Start()
    {
        VirusManager.instance.register(this);
        _correctAltitude = VirusManager.derivedInstance.virusPrefab.transform.position.y;

        #if VERBOSEDEBUG
        Debug.Log(this.gameObject.name + " Start");
        #endif

        _waypoint = VirusManager.derivedInstance.waypoint;
        if (_waypoint.x < this.transform.position.x)
        {
            #if VERBOSEDEBUG
            Debug.Log("Entering!");
            #endif
            isEntering = true;
            target = _waypoint;
            checkAltitude();
        }
        #if VERBOSEDEBUG
        else
        {
            Debug.Log("Spawning!");
        }
        #endif
        
        repulsers = new string[4] {Enemy.enemyTag, RedBloodCellMovement.rbcTag, Virus.virusTag, WhiteBloodCellMovement.wbcTag};
        _rigidbody.AddForce(Vector3.up * startImpulse, ForceMode.Impulse);
    }

    private void checkAltitude()
    {
        if (this.transform.position.y < _correctAltitude)
        {
            _rigidbody.AddForce(Vector3.up * gainAltitudeImpulse, ForceMode.Impulse);
        }
    }

    public GameObject getPrefab()
    {
        return VirusManager.derivedInstance.virusPrefab;
    }

    public void setTarget(Pneumocyte pneumocyte)
    {
        targetPneumocyte = pneumocyte;
        target = new Vector3(pneumocyte.transform.position.x, _correctAltitude, pneumocyte.transform.position.z);
        checkAltitude();
    }

    private void setTarget()
    {
        if (!isEscaping && !isEntering &&  ((null == targetPneumocyte) || (targetPneumocyte.status != Pneumocyte.STATUS.HEALTHY)))
        {
            // find closest healthy pneumocyte
            sqrMagnitudeToClosestPC = Mathf.Infinity;

            for (int i = pcIndex; i < pcIndex + PneumocyteManager.instance.entities.Length; i++)
            {
                int j = i % PneumocyteManager.instance.entities.Length;
                if (PneumocyteManager.instance.entities[j].status == Pneumocyte.STATUS.HEALTHY)
                {
                    _sqrMagnitude =  (PneumocyteManager.instance.entities[j].transform.position - this.transform.position).sqrMagnitude;
                    if ((null == closestPC) || (_sqrMagnitude < sqrMagnitudeToClosestPC))
                    {
                        closestPC = PneumocyteManager.instance.entities[j];
                        sqrMagnitudeToClosestPC = _sqrMagnitude;
                    }
                }
            }
            pcIndex++;

            if (null != closestPC)
            {
                setTarget(closestPC);
            }
            else if ((null == targetPneumocyte) || hasReachedTarget)
            {
                setTarget(PneumocyteManager.instance.entities[Random.Range(0, PneumocyteManager.instance.entities.Length)]);
            }
        }
    }

    protected override void onWobbleDone()
    {
        if (hasReachedTarget)
        {
            if ((null != targetPneumocyte) && (targetPneumocyte.status == Pneumocyte.STATUS.HEALTHY))
            {
                setHoldingPosition(true);
                targetPneumocyte.getInfected(this);
                infectionCoroutine();
            }
            else if (isEscaping1)
            {
                #if VERBOSEDEBUG
                Debug.Log("done going to waypoint while escaping");
                #endif
                isEscaping1 = false;
                isEscaping2 = true;
                target = _escapeTarget;
                checkAltitude();
            }
            else if (isEscaping2)
            {
                #if VERBOSEDEBUG
                Debug.Log("done escaping");
                #endif
                Destroy(this.gameObject);
            }
            else if (isEntering)
            {
                #if VERBOSEDEBUG
                Debug.Log("done going to waypoint while entering");
                #endif
                isEntering = false;
                setHoldingPosition(false);
            }
        }
        setTarget();
    }

    private void kickToPosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - this.transform.position).normalized;
        _rigidbody.AddForce(direction * absorptionImpulse, ForceMode.Impulse);
    }

    private IEnumerator jumpToTransform(Transform targetTransform)
    {
        setHoldingPosition(true);
        sphereCollider.enabled = false;
        while ((null != targetTransform) && (this.transform.position - targetTransform.position).sqrMagnitude > sqrMagnitudeProximityThreshold)
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

    private void infectionCoroutine()
    {
        VirusManager.instance.unregister(this);
        StartCoroutine(jumpToTransform(targetPneumocyte.infectionTransform));
    }

    public void getAbsorbed(Transform absorberTransform)
    {
        VirusManager.instance.unregister(this);
        StartCoroutine(jumpToTransform(absorberTransform));
    }

    public void escape(Vector3 escapeTarget)
    {
        isEscaping  = true;
        isEscaping1 = true;
        isEscaping2 = false;
        VirusManager.instance.unregister(this);
        target = _waypoint;
        checkAltitude();
        _escapeTarget = escapeTarget;
    }
}