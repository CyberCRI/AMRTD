//#define DEVMODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBloodCellMovement : WobblyMovement
{
    private Transform targetTransform = null;
    public float baseSpeed = 0f;
    [SerializeField]
    private float speedVariation = 0f;
    private Vector3 idlePosition = Vector3.zero;
    private static Transform bloodOrigin2 = null;

#if DEVMODE
    public WBCACTION action = WBCACTION.NONE;

    public enum WBCACTION
    {
        NONE,
        GOTOIDLE,
        IDLE,
        GOTOENEMYLIMITED,
        GOTOENEMYFREE,
        REACHEDENEMY
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
        }
    }

    protected override void setDisplacement()
    {
        if (null != targetTransform)
        {
            if ((targetTransform.position - this.transform.position).magnitude > minimumDistance)
            {
                if (targetTransform.position.x > bloodOrigin2.position.x)
                {
                    displacement = (new Vector3(bloodOrigin2.position.x, targetTransform.position.y, targetTransform.position.z) - this.transform.position).normalized * speed * Time.deltaTime;
#if DEVMODE
                    action = WBCACTION.GOTOENEMYLIMITED;
#endif
                }
                else
                {
                    displacement = (targetTransform.position - this.transform.position).normalized * speed * Time.deltaTime;
#if DEVMODE
                    action = WBCACTION.GOTOENEMYFREE;
#endif
                }
            }
            else
            {
                displacement = Vector3.zero;
#if DEVMODE
                action = WBCACTION.REACHEDENEMY;
#endif
            }
        }
        else
        {
            // go to idle position
            if ((idlePosition - this.transform.position).magnitude > minimumDistance)
            {
                displacement = (idlePosition - this.transform.position).normalized * speed * Time.deltaTime;
#if DEVMODE
                action = WBCACTION.GOTOIDLE;
            }
            else
            {
                action = WBCACTION.IDLE;
#endif
            }
        }
    }

    protected override void onWobbleDone()
    {
        if ((null != targetTransform) && (targetTransform.position - this.transform.position).magnitude < minimumDistance)
        {
            absorb();
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

    public void initialize(Vector3 _idlePosition)
    {
        idlePosition = _idlePosition;
    }

    private void absorb()
    {
        Destroy(targetTransform.gameObject);

        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        PlayerStatistics.instance.lives--;
    }
}
