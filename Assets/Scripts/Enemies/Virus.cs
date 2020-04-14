#define VERBOSEDEBUG
using UnityEngine;

public class Virus : WobblyMovement
{
    [Header("Parameters")]
    public const string virusTag = "VirusTag";
    private static GameObject _virusPrefab = null;
    [SerializeField]
    private string prefabURI = null;
    private STATUS status = STATUS.SEARCHING_CELL;
    public int virionsSpawnCountPerLungCell = 2;

    // lungCellRecoveryProbability * 100 % chances of not dying when spawn ends
    public float lungCellRecoveryProbability = .6f;

    // damage ratios inflected; ratio of max health
    public float damageRatioPerInfection = .4f;
    public float damageRatioPerSpawn = .1f;
    public float timeBeforeSpawnStarts = 1f;
    public float timeBeforeRecoveryStarts = .5f;
    public float timeBetweenSpawns = 1f;

    public float startImpulse = 20f;

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

            for (int i = pcIndex; i < pcIndex + Pneumocyte.pneumocytes.Length; i++)
            {
                int j = i % Pneumocyte.pneumocytes.Length;
                if (Pneumocyte.pneumocytes[j].status == Pneumocyte.STATUS.HEALTHY)
                {
                    _sqrMagnitude =  (Pneumocyte.pneumocytes[j].transform.position - this.transform.position).sqrMagnitude;
                    if ((null == closestPC) || (_sqrMagnitude < sqrMagnitudeToClosestPC))
                    {
                        closestPC = Pneumocyte.pneumocytes[j];
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
                setTarget(Pneumocyte.pneumocytes[Random.Range(0, Pneumocyte.pneumocytes.Length)]);
            }
        }
    }

    protected override void onWobbleDone()
    {
        setTarget();
        if (hasReachedTarget && (null != targetPneumocyte) && (targetPneumocyte.status == Pneumocyte.STATUS.HEALTHY))
        {
            targetPneumocyte.infect(this);
            //Destroy(this.gameObject);
            this.setHoldingPosition(true);
        }
    }
}