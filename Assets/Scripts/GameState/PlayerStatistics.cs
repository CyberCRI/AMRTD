//#define LIFEPOINTSMODE
#define DEVMODE
//#define STATICTURRETCOUNTMODE
#define STATICTURRETRESISTANCEPOINTSMODE
//#define DYNAMICTURRETRESISTANCEPOINTSMODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public static PlayerStatistics instance = null;

    public int waves = 0;

    [Header("Money")]
    [SerializeField]
    private int startMoney = 0;
    public int money = 0;

    [Header("Lives: #pathogens that may escape")]
    public int lives = 0;
    public int startLives = 5;

    //#if LIFEPOINTSMODE
    [Header("Life points: integrates resistance")]
    public float lifePoints = 0f;
    public float startLifePoints = defaultLifePoints;
    public float livesToLifePointsFactor;
    public const float defaultLifePoints = 100f;
    //#endif

    [Header("Resistance")]
    private float _resistancePoints = 0f;
    public float resistancePoints
    {
        get
        {
            return _resistancePoints;
        }
        set
        {
            _resistancePoints = value;
            // update Resistance Ratio
            _resistancePointsRatio = resistancePoints / defaultMaxResistancePoints;
        }
    }
    private float _resistancePointsRatio = 0f; // in [0f, 1f]
    public float resistancePointsRatio
    {
        get
        {
            return _resistancePointsRatio;
        }
    }
    public float resistanceToLifeFactor = 0f;
    public const float defaultMaxResistancePoints = 100f;
    public const float costABPerSec = 1f;
    [SerializeField]
    private float startResistancePoints = 0f;
    public float offsetRatio = .5f;

    [Header("Resistance decay")]
    private float animationDuration = .1f;
    private bool isLerpInProgress = false;
    private IEnumerator coroutine = null;
    private float negativePointsPool = 0f;
    private float negativePointsStep = 1f;
    private float step = 0f;
    private float lerp = 0f;
    private float startValue = 0f;
    private float endValue = 0f;
    private float timeParameter = 0f;

    #region turret points
    [SerializeField]
    private float turretCountToResistanceFactor = 15f;
    [SerializeField]
    private float turretResistancePointsToResistanceFactor = 1f;
    [SerializeField]
    private float tRPThreshold1 = 0f;
    [SerializeField]
    private float tRPMarkerRatio1 = 0f;
    [SerializeField]
    private float tRPThreshold2 = 0f;
    [SerializeField]
    private float tRPMarkerRatio2 = 0f;
    [SerializeField]
    private float computedRP = 0f;

    private float tRPSmallSlope = 0f;
    private float tRPBigSlope = 0f;
    private int _turretCount = 0;
    private float _turretResistancePoints = 0f;

#if STATICTURRETCOUNTMODE
    public int turretCount
    {
        get { return _turretCount; }
        set
        {
            _turretCount = value;
            resistancePoints = PlayerStatistics.instance.turretCountToResistanceFactor * _turretCount;
        }
    }
#endif
#if STATICTURRETRESISTANCEPOINTSMODE
    public float turretResistancePoints
    {
        get { return _turretResistancePoints; }
        set
        {
            if (value < _turretResistancePoints)
            {
                negativePointsPool += (_turretResistancePoints - value);
            }
            else
            {
                if (null != coroutine)
                {
                    StopCoroutine(coroutine);
                    isLerpInProgress = false;
                    updateResistancePoints(value - lerp + endValue);
                }
                else
                {
                    updateResistancePoints(value);
                }
            }

            if ((!isLerpInProgress) && (negativePointsPool > 0))
            {
                coroutine = smoothReduce();
                if (null != coroutine)
                {
                    StartCoroutine(coroutine);
                }
                else
                {
                    Debug.LogError("unexpected null coroutine");
                }
            }
        }
    }
#endif

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        money = startMoney;
        lives = startLives;
        resistancePoints = startResistancePoints;
        waves = 0;

#if LIFEPOINTSMODE
        lifePoints = startLifePoints;
        resistanceToLifeFactor = (1f - offsetRatio) * startLifePoints / defaultMaxResistancePoints;
        livesToLifePointsFactor = startLifePoints / lives;
#endif

#if STATICTURRETRESISTANCEPOINTSMODE 
        tRPSmallSlope = tRPMarkerRatio1 / tRPThreshold1;
        tRPBigSlope = (tRPMarkerRatio2 - tRPMarkerRatio1) / (tRPThreshold2 - tRPThreshold1);
#endif
    }

#if DEVMODE || LIFEPOINTSMODE
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

#if DEVMODE
        if (Input.GetKeyDown(KeyCode.Return))
        {
            money += startMoney;
        }
#endif

#if LIFEPOINTSMODE
        lifePoints = startLifePoints;
        lifePoints = lives * livesToLifePointsFactor - resistancePoints * resistanceToLifeFactor;
        lifePoints = Mathf.Clamp(lifePoints, 0f, startLifePoints);
#endif
    }
#endif

    // updates the turret resistance points using the given value
    //   also updates the resistance points displayed in the resistance bar by using a custom function
    //   (the function that computes the resistance points from the turret resistance points)
    private void updateResistancePoints(float value)
    {
        _turretResistancePoints = value;
        resistancePoints =
            getResistancePointsFromTurretResistancePoints(_turretResistancePoints);
        //PlayerStatistics.instance.turretResistancePointsToResistanceFactor
        //* _turretResistancePoints;
    }

    // tRP: turret Resistance Points
    // piecewise linear function - linearized sigmoid
    //
    //                    t2 _________
    //                  /
    //    _________ t1 /
    //
    // the slopes before t1 and after t2 are the same (arbitrary)
    // the slope is computed from the slope before t1: tRPMarkerValue1/tRPThreshold1
    private float getResistancePointsFromTurretResistancePoints(float tRP)
    {
        if (tRP <= tRPThreshold1)
        {
            computedRP = tRPSmallSlope * tRP;
        }
        else if (tRP >= tRPThreshold2)
        {
            computedRP = tRPMarkerRatio2 + tRPSmallSlope * (tRP - tRPThreshold2);
        }
        else
        {
            computedRP = tRPMarkerRatio1 + tRPBigSlope * (tRP - tRPThreshold1);
        }
        return computedRP * defaultMaxResistancePoints;
    }

    //#endif
    #endregion

    // allows for a smooth transition from resistance(turrets_count(t+1))
    // to resistance(turrets_count(t+1))
    // through the use of the variable negativePointsPool
    private IEnumerator smoothReduce()
    {
        isLerpInProgress = true;
        while (negativePointsPool > 0f)
        {
            timeParameter = 0f;
            step = Mathf.Min(negativePointsStep, negativePointsPool);
            negativePointsPool -= step;
            startValue = _turretResistancePoints;
            endValue = _turretResistancePoints - step;
            while (timeParameter <= 1)
            {
                timeParameter += Time.deltaTime / animationDuration;
                lerp = Mathf.Lerp(startValue, endValue, timeParameter);
                updateResistancePoints(lerp);
                yield return null;
            }
        }
        isLerpInProgress = false;
    }

#if DYNAMICTURRETRESISTANCEPOINTSMODE
    public void takeResistance(float amount)
    {
        resistancePoints = Mathf.Min(resistancePoints + amount, defaultMaxResistancePoints);
    }
#endif
}
