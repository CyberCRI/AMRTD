#define VERBOSEDEBUG
//#define DEVMODE
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
        #if VERBOSEDEBUG
        Debug.Log(string.Format("{0}: {1}: Start ", this.GetType(), this.gameObject.name));
        #endif

        //setTarget();
        setSpeed();

        repulsers = new string[4] {Enemy.enemyTag, RedBloodCellMovement.rbcTag, Virus.virusTag, WhiteBloodCellMovement.wbcTag};
    }

    protected override void OnTriggerEnter(Collider collider)
    {
        if (collider.transform == targetTransform)
        {
            absorb(targetTransform);
        }
        base.OnTriggerEnter(collider);
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
            float x,z;
            if (WhiteBloodCellManager.instance.isAreaConstrained)
            {
                x = Mathf.Clamp(target.x, WhiteBloodCellManager.instance.limitLeft, WhiteBloodCellManager.instance.limitRight);
                z = Mathf.Clamp(target.z, WhiteBloodCellManager.instance.limitBottom, WhiteBloodCellManager.instance.limitTop);
            }
            else
            {
                x = target.x;
                z = target.z;
            }
            target = new Vector3(x, target.y, z);
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

    private bool isTargetInConstrainedArea()
    {
        return (       (!WhiteBloodCellManager.instance.isAreaConstrained)
                    || (   (targetTransform.position.x <= WhiteBloodCellManager.instance.limitRight)
                        && (targetTransform.position.x >= WhiteBloodCellManager.instance.limitLeft)
                        && (targetTransform.position.z <= WhiteBloodCellManager.instance.limitTop)
                        && (targetTransform.position.z >= WhiteBloodCellManager.instance.limitBottom)
                        )
                );
    }

    protected override void onWobbleDone()
    {
        if (hasReachedTarget)
        {
            if ((null != targetTransform) && isTargetInConstrainedArea())
            {
                // chasing
                absorb(targetTransform);
            }
            else if (disappearing)
            {
                // disappearing
                RedMetricsManager.instance.sendEvent(TrackingEvent.WBCLEAVES, CustomData.getGameObjectContext(this.gameObject));
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

    private void prepareAbsorb()
    {
        WhiteBloodCellManager.instance.reportDeath(index);

#if VERBOSEDEBUG
        action = WBCACTION.DISAPPEARING;
#endif

        disappearing = true;
        target = BloodUtilities.instance.bloodEnd2.position;
        targetTransform = null;
        hasReachedTarget = false;
    }

    public void absorb(Virus virus)
    {
        prepareAbsorb();
        RedMetricsManager.instance.sendEvent(TrackingEvent.PATHOGENKILLEDBYWBC, CustomData.getGameObjectContext(virus.gameObject));
        virus.getAbsorbed(this.transform);
    }

    private void absorb(EnemyMovement enemy)
    {
        prepareAbsorb();
        RedMetricsManager.instance.sendEvent(TrackingEvent.PATHOGENKILLEDBYWBC, CustomData.getGameObjectContext(enemy.gameObject));
        enemy.getAbsorbed(this.transform);
    }

    private void absorb(Transform ttransform)
    {
        if (ttransform.tag == Enemy.enemyTag)
        {
            absorb(ttransform.GetComponent<EnemyMovement>());
        }
        else if (ttransform.tag == Virus.virusTag)
        {
            absorb(ttransform.GetComponent<Virus>());
        }
        else
        {
            Debug.LogError("incorrect tag " + ttransform.tag);
        }
    }

#if !DEVMODE
    void OnDestroy()
    {
        if (!GameManager.instance.isObjectiveDefenseMode())
        {
            PlayerStatistics.instance.lives--;
        }
    }
#endif
}
