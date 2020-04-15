//#define VERBOSEDEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBloodCellMovement : WobblyMovement
{
    public const string wbcTag = "WBCTag";
    private int index = 0;
    private Transform targetTransform = null;
    public float baseSpeed = 0f;
    [SerializeField]
    private float speedVariation = 0f;
    public Vector3 idlePosition = Vector3.zero;
    private static Transform bloodOrigin2 = null;
    private static Transform bloodUnder = null;
    private static Transform bloodEnd2 = null;
    private bool disappearing = false;

#if VERBOSEDEBUG
    public WBCACTION action = WBCACTION.NONE;

    public enum WBCACTION
    {
        NONE,
        IDLE,
        CHASING,
        DISAPPEARING,
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        //setTarget();
        setSpeed();

        if (null == bloodOrigin2)
        {
            bloodOrigin2 = RedBloodCellManager.instance.bloodOrigin2;
            bloodUnder = RedBloodCellManager.instance.bloodUnder;
            bloodEnd2 = RedBloodCellManager.instance.bloodEnd2;
        }

        repulsers = new string[4] {Enemy.enemyTag, RedBloodCellMovement.rbcTag, Virus.virusTag, WhiteBloodCellMovement.wbcTag};
    }

    protected override void setDisplacement()
    {
        // set target if needed
        if (null != targetTransform) // chasing
        {
#if VERBOSEDEBUG
            action = WBCACTION.CHASING;
#endif
            hasReachedTarget = false;
            target = targetTransform.position;
            target = new Vector3(Mathf.Min(target.x, bloodOrigin2.position.x), target.y, target.z);
        }
        else if ((!disappearing) && (Vector3.zero == target)) // idle
        {
#if VERBOSEDEBUG
            action = WBCACTION.IDLE;
#endif
            hasReachedTarget = false;
            target = idlePosition;
        }

        base.setDisplacement();
    }

    protected override void onWobbleDone()
    {
        if (hasReachedTarget)
        {
            if ((null != targetTransform) && (target.x < bloodOrigin2.position.x)) // chasing
            {
                // chasing
                absorb();
            }
            else if (disappearing)
            {
                // disappearing
                Destroy(this.gameObject);
            }
        }
    }

    public void setTarget(Transform _target)
    {
        targetTransform = _target;
    }

    private void setSpeed()
    {
        startSpeed = baseSpeed + Random.Range(-speedVariation, speedVariation);
    }

    public void initialize(int _index, Vector3 _idlePosition)
    {
        index = _index;
        idlePosition = _idlePosition;
    }

    public void absorb(Virus virus = null)
    {

        WhiteBloodCellManager.instance.reportDeath(index);

#if VERBOSEDEBUG
            action = WBCACTION.DISAPPEARING;
#endif

        disappearing = true;
        target = bloodEnd2.position;
        hasReachedTarget = false;

        if (null != virus)
        {
            RedMetricsManager.instance.sendEvent(TrackingEvent.PATHOGENKILLEDBYWBC, CustomData.getGameObjectContext(virus.gameObject));
            virus.getAbsorbed(this.transform);
        }
        else
        {
            RedMetricsManager.instance.sendEvent(TrackingEvent.PATHOGENKILLEDBYWBC, CustomData.getGameObjectContext(targetTransform.gameObject));
            Destroy(targetTransform.gameObject);
        }
        
    }

    void OnDestroy()
    {
        PlayerStatistics.instance.lives--;
    }
}
