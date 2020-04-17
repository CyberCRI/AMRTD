//#define DETRIMENTALOPTIMIZATION
//#define VERBOSEDEBUG
//#define ALWAYSUPDATEBLOODCOLOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBloodCellMovement : WobblyMovement
{
    public const string rbcTag = "RBCTag";
    public float baseSpeed = 0f;
    private int waypointIndex = 0;
    [SerializeField]
    private float speedVariation = 0f;
    private Color _color = Color.red;
    private float _advancement = 0f;
    private float _alveoliHealth = 0f;
    private float _colorLerp = 0f;

//    float creationTime = 0f;
//    float timeToComplete = 0f;
    
    // http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/
    private Renderer _renderer = null;
    private MaterialPropertyBlock _propBlock = null;

    // Start is called before the first frame update
    void Start()
    {
        #if VERBOSEDEBUG
        Debug.Log(string.Format("{0}: {1}: Start ", this.GetType(), this.gameObject.name));
        #endif

        lazyInitialize();

        if (waypointIndex == 0)
        {
            setTarget();
        }
        setSpeed();

//        creationTime = Time.time;
//        timeToComplete = RedBloodCellManager.instance.topToBottom / startSpeed;

        repulsers = new string[4] {Enemy.enemyTag, RedBloodCellMovement.rbcTag, Virus.virusTag, WhiteBloodCellMovement.wbcTag};
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

    protected override void onWobbleDone()
    {
#if ALWAYSUPDATEBLOODCOLOR
        float t = Mathf.Clamp((Time.time - creationTime) / timeToComplete, 0f, 1f);
        _propBlock.SetColor("_Color", Color.Lerp(RedBloodCellManager.instance.deoxygenatedBloodColor, RedBloodCellManager.instance.oxygenatedBloodColor, t));
#endif

        if (hasReachedTarget)
        {
#if DETRIMENTALOPTIMIZATION
            resetPosition();
            setTarget();
            setSpeed();
#else
            if (BloodUtilities.instance.isWaypointsBased && (waypointIndex < BloodUtilities.instance.bloodWayPoints.Length))
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
        if (BloodUtilities.instance.isWaypointsBased)
        {
            #if VERBOSEDEBUG
            Debug.Log(string.Format("{0}: {1}: setTarget() ->{2}", this.GetType(), this.gameObject.name, waypointIndex));
            #endif
            target = BloodUtilities.instance.bloodWayPoints[waypointIndex++].position;
            
            setColor();
        }
        else
        {
            float t = Random.Range(0f, 1f);
            target = t * BloodUtilities.instance.bloodEnd1.position + (1 - t) * BloodUtilities.instance.bloodEnd2.position;
        }
    }

    public void setTarget(int _waypointIndex)
    {
        #if VERBOSEDEBUG
        Debug.Log(string.Format("{0}: {1}: setTarget({2})", this.GetType(), this.gameObject.name, _waypointIndex));
        #endif

        waypointIndex = _waypointIndex;
        setTarget();
    }

    private void setColor()
    {
        #if VERBOSEDEBUG
        Debug.Log(string.Format("{0}: {1}: setColor ", this.GetType(), this.gameObject.name));
        #endif
#if !ALWAYSUPDATEBLOODCOLOR
        lazyInitialize();

        //float t = (Time.time - creationTime) / timeToComplete;

        _advancement = ((float) waypointIndex) / ((float) BloodUtilities.instance.bloodWayPoints.Length);
        _alveoliHealth = PneumocyteManager.derivedInstance.getHealthRatio();
        _colorLerp = Mathf.Max(_colorLerp, _advancement * _alveoliHealth);

        _color = Color.Lerp(
            RedBloodCellManager.instance.deoxygenatedBloodColor,
            RedBloodCellManager.instance.oxygenatedBloodColor,
            _colorLerp
            );
        _propBlock.SetColor("_Color", _color);

            // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
#endif
    }

    private void resetPosition()
    {
        float t = Random.Range(0f, 1f);
        this.transform.position = t * BloodUtilities.instance.bloodOrigin1.position + (1 - t) * BloodUtilities.instance.bloodOrigin2.position;
    }

    private void setSpeed()
    {
        startSpeed = baseSpeed + Random.Range(-speedVariation, speedVariation);
    }
}
