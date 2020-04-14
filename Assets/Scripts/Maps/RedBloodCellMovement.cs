//#define DETRIMENTALOPTIMIZATION
//#define VERBOSEDEBUG
//#define ALWAYSUPDATEBLOODCOLOR

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
    private static Color oxygenatedBloodColor = Color.red;
    private static Color deoxygenatedBloodColor = Color.blue; // deoxygenated blood is deep red-purple #8E005F
    private static float topToBottom = 0f;
    public float baseSpeed = 0f;
    private int waypointIndex = 0;
    private static bool isWaypointsBased = false;
    [SerializeField]
    private float speedVariation = 0f;

//    float creationTime = 0f;
//    float timeToComplete = 0f;
    
    // http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/
    private Renderer _renderer = null;
    private MaterialPropertyBlock _propBlock = null;

    // Start is called before the first frame update
    void Start()
    {
        lazyInitializeStatics();
        lazyInitialize();

        if (waypointIndex == 0)
        {
            setTarget();
        }
        setSpeed();

//        creationTime = Time.time;
//        timeToComplete = topToBottom / startSpeed;

        repulsers = new string[3] {WhiteBloodCellMovement.wbcTag, RedBloodCellMovement.rbcTag, Enemy.enemyTag};
    }

    private void lazyInitialize()
    {
        if (null == _propBlock)
        {
            _renderer = GetComponent<Renderer>();
            _propBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_propBlock);
        }
    }

    private void lazyInitializeStatics()
    {
        if (null == bloodOrigin1)
        {
            Transform[] positions = RedBloodCellManager.instance.getBloodPositions();
            bloodOrigin1 = positions[0];
            bloodOrigin2 = positions[1];
            bloodEnd1 = positions[2];
            bloodEnd2 = positions[3];
            Color[] colors = RedBloodCellManager.instance.getBloodColors();
            oxygenatedBloodColor = colors[0];
            deoxygenatedBloodColor = colors[1];
            topToBottom = RedBloodCellManager.instance.topToBottom;

            isWaypointsBased = RedBloodCellManager.instance.isWaypointsBased;
            if (isWaypointsBased)
            {
                bloodWayPoints = RedBloodCellManager.instance.bloodWayPoints;
            }
        }
    }

    protected override void onWobbleDone()
    {
//#if ALWAYSUPDATEBLOODCOLOR
//        float t = Mathf.Clamp((Time.time - creationTime) / timeToComplete, 0f, 1f);
//        _propBlock.SetColor("_Color", Color.Lerp(deoxygenatedBloodColor, oxygenatedBloodColor, t));
//#endif

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
            #if VERBOSEDEBUG
            Debug.Log(string.Format("{0}: setTarget {1}->{2}", this.GetType(), this.gameObject.name, waypointIndex));
            #endif
            target = bloodWayPoints[waypointIndex++].position;
            
            setColor();
        }
        else
        {
            float t = Random.Range(0f, 1f);
            target = t * bloodEnd1.position + (1 - t) * bloodEnd2.position;
        }
    }

    public void setTarget(int _waypointIndex)
    {
        lazyInitializeStatics();
        
        target = bloodWayPoints[_waypointIndex].position;
        waypointIndex = _waypointIndex+1;
            
        setColor();
    }

    private void setColor()
    {
#if !ALWAYSUPDATEBLOODCOLOR
        lazyInitialize();

        //float t = (Time.time - creationTime) / timeToComplete;
        _propBlock.SetColor("_Color", Color.Lerp(deoxygenatedBloodColor, oxygenatedBloodColor, ((float) waypointIndex) / ((float) bloodWayPoints.Length)));

            // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
#endif
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
