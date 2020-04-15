#define VERBOSEDEBUG
using UnityEngine;
using System.Collections;

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

    private float startImpulse = 20f;
    private float destroyY = -5f;

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
    }

    private void setTarget()
    {
        if ((null == targetPneumocyte) || (targetPneumocyte.status != Pneumocyte.STATUS.HEALTHY))
        {
            // find closest healthy pneumocyte
            sqrMagnitudeToClosestPC = Mathf.Infinity;

            for (int i = pcIndex; i < pcIndex + PneumocyteManager.instance.pneumocytes.Length; i++)
            {
                int j = i % PneumocyteManager.instance.pneumocytes.Length;
                if (PneumocyteManager.instance.pneumocytes[j].status == Pneumocyte.STATUS.HEALTHY)
                {
                    _sqrMagnitude =  (PneumocyteManager.instance.pneumocytes[j].transform.position - this.transform.position).sqrMagnitude;
                    if ((null == closestPC) || (_sqrMagnitude < sqrMagnitudeToClosestPC))
                    {
                        closestPC = PneumocyteManager.instance.pneumocytes[j];
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
                setTarget(PneumocyteManager.instance.pneumocytes[Random.Range(0, PneumocyteManager.instance.pneumocytes.Length)]);
            }
        }
    }

    protected override void onWobbleDone()
    {
        setTarget();
        if (hasReachedTarget && (null != targetPneumocyte) && (targetPneumocyte.status == Pneumocyte.STATUS.HEALTHY))
        {
            targetPneumocyte.infect(this);
            StartCoroutine(infectionCoroutine());
        }
    }

    private IEnumerator infectionCoroutine()
    {
        this.setHoldingPosition(true);
        sphereCollider.enabled = false;
        _rigidbody.AddForce(Vector3.down * startImpulse, ForceMode.Impulse);
        while (this.transform.position.y > destroyY)
        {
            yield return 0;
        }
        Destroy(this.gameObject);
    }
}