//#define DETRIMENTALOPTIMIZATION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBloodCellMovement : WobblyMovement
{
    public const string rbcTag = "RBCTag";
    private static Transform bloodOrigin1 = null;
    private static Transform bloodOrigin2 = null;
    private static Transform bloodEnd1 = null;
    private static Transform bloodEnd2 = null;
    private static Transform[] bloodWayPoints = null;
    public float baseSpeed = 0f;
    private int waypointIndex = 0;
    private static bool isWaypointsBased = false;
    [SerializeField]
    private float speedVariation = 0f;
    
    // http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/
    private Renderer _renderer = null;
    private MaterialPropertyBlock _propBlock = null;

    // Start is called before the first frame update
    void Start()
    {
        if (null == bloodOrigin1)
        {
            Transform[] positions = RedBloodCellManager.instance.getBloodPositions();
            bloodOrigin1 = positions[0];
            bloodOrigin2 = positions[1];
            bloodEnd1 = positions[2];
            bloodEnd2 = positions[3];

            isWaypointsBased = RedBloodCellManager.instance.isWaypointsBased;
            if (isWaypointsBased)
            {
                bloodWayPoints = RedBloodCellManager.instance.bloodWayPoints;
            }
        }

        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_propBlock);

        setTarget();
        setSpeed();

        repulsers = new string[3] {WhiteBloodCellMovement.wbcTag, RedBloodCellMovement.rbcTag, Enemy.enemyTag};
    }

    protected override void onWobbleDone()
    {
        if (hasReachedTarget)
        {
#if DETRIMENTALOPTIMIZATION
            resetPosition();
            setTarget();
            setSpeed();
#else
            if (isWaypointsBased && (waypointIndex < bloodWayPoints.Length))
            {
                setTarget();
            }
            else
            {
                Destroy(this.gameObject);
            }
#endif
        }
    }

    private void setTarget()
    {
        if (isWaypointsBased)
        {
            target = bloodWayPoints[waypointIndex++].position;
            
            _propBlock.SetColor("_Color", Color.Lerp(Color.blue, Color.red, ((float) waypointIndex) / ((float) bloodWayPoints.Length)));
            // Apply the edited values to the renderer.
            _renderer.SetPropertyBlock(_propBlock);
        }
        else
        {
            float t = Random.Range(0f, 1f);
            target = t * bloodEnd1.position + (1 - t) * bloodEnd2.position;
        }
    }

    private void resetPosition()
    {
        float t = Random.Range(0f, 1f);
        this.transform.position = t * bloodOrigin1.position + (1 - t) * bloodOrigin2.position;
    }

    private void setSpeed()
    {
        startSpeed = baseSpeed + Random.Range(-speedVariation, speedVariation);
    }
}
