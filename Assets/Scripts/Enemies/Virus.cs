#define VERBOSEDEBUG
using UnityEngine;

public class Virus : WobblyMovement
{
    [Header("Parameters")]
    public GameObject virusPrefab = null;
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

    [SerializeField]
    private Pneumocyte targetPneumocyte = null;

    private enum STATUS
    {
        SEARCHING_CELL,
        GOING_TO_CELL,
    }

    void Start()
    {
        setTarget();
    }

    private void setTarget()
    {
        targetPneumocyte = (null == targetPneumocyte) || (targetPneumocyte.status != Pneumocyte.STATUS.HEALTHY) ? null : targetPneumocyte;
        if (null == targetPneumocyte)
        {
            #if VERBOSEDEBUG
            Debug.Log(this.gameObject.name + " setTarget start (null == targetPneumocyte)==true");
            #endif
            // find closest healthy pneumocyte
            Pneumocyte closestPC = null;
            float sqrMagnitudeToClosestPC = Mathf.Infinity;
            for (int i = 0; i < Pneumocyte.pneumocytes.Length; i++)
            {
                if (Pneumocyte.pneumocytes[i].status == Pneumocyte.STATUS.HEALTHY)
                {
                    float _sqrMagnitude =  (Pneumocyte.pneumocytes[i].transform.position - this.transform.position).sqrMagnitude;
                    if ((null == closestPC) || (_sqrMagnitude < sqrMagnitudeToClosestPC))
                    {
                        closestPC = Pneumocyte.pneumocytes[i];
                        sqrMagnitudeToClosestPC = _sqrMagnitude;
                    }
                }
            }
            if (null != closestPC)
            {
                targetPneumocyte = closestPC;
                target = closestPC.transform.position;
            }
            #if VERBOSEDEBUG
            Debug.Log(this.gameObject.name + " setTarget end (null == targetPneumocyte)=="+(null == targetPneumocyte));
            #endif
        }
    }

    protected override void onWobbleDone()
    {
        setTarget();
        if (hasReachedTarget)
        {
            if (null != targetPneumocyte)
            {
                targetPneumocyte.infect(this);
                //Destroy(this.gameObject);
                this.setHoldingPosition(true);
            }
        }
    }
}