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
    private static GameObject _virusPrefab = null;
    [SerializeField]
    private string prefabURI = null;
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

        #if VERBOSEDEBUG
        Debug.Log(this.gameObject.name + " Start");
        #endif
        if (null == _virusPrefab)
        {
            _virusPrefab = Resources.Load<GameObject>(prefabURI);
        }
        setTarget();
        repulsers = new string[4] {Enemy.enemyTag, RedBloodCellMovement.rbcTag, Virus.virusTag, WhiteBloodCellMovement.wbcTag};
        _rigidbody.AddForce(Vector3.up * startImpulse, ForceMode.Impulse);
    }

    public GameObject getPrefab()
    {
        return _virusPrefab;
    }

    public void setTarget(Pneumocyte pneumocyte)
    {
        targetPneumocyte = pneumocyte;
        target = pneumocyte.transform.position;
        if (this.transform.position.y < target.y)
        {
            _rigidbody.AddForce(Vector3.up * gainAltitudeImpulse, ForceMode.Impulse);
        }
    }

    private void setTarget()
    {
        if ((null == targetPneumocyte) || (targetPneumocyte.status != Pneumocyte.STATUS.HEALTHY))
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
        setTarget();
        if (hasReachedTarget && (null != targetPneumocyte) && (targetPneumocyte.status == Pneumocyte.STATUS.HEALTHY))
        {
            targetPneumocyte.getInfected(this);
            infectionCoroutine();
        }
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

    private void infectionCoroutine()
    {
        StartCoroutine(jumpToTransform(targetPneumocyte.infectionTransform));
    }

    public void getAbsorbed(Transform absorberTransform)
    {
        VirusManager.instance.unregister(this);
        StartCoroutine(jumpToTransform(absorberTransform));
    }
}