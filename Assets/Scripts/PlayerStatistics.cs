//#define LIFEPOINTSMODE
//#define DEVMODE
//#define STATICTURRETCOUNTMODE
#define STATICTURRETRESISTANCEPOINTSMODE
//#define DYNAMICTURRETRESISTANCEPOINTSMODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public static PlayerStatistics instance = null;

    public static int waves;

    [Header("Money")]
    [SerializeField]
    private int startMoney = 0;
    public static int money;

    [Header("Lives: #pathogens that may escape")]
    public static int lives;
    [SerializeField]
    private int startLives = 0;

    //#if LIFEPOINTSMODE
    [Header("Life points: integrates resistance")]
    public static float lifePoints = 0f;
    public static float startLifePoints = defaultLifePoints;
    public static float livesToLifePointsFactor;
    public const float defaultLifePoints = 100f;
    //#endif

    [Header("Resistance")]
    public static float resistancePoints = 0f;
    public static float resistanceToLifeFactor = 0f;
    public const float defaultMaxResistancePoints = 100f;
    public const float costABPerSec = 1f;
    [SerializeField]
    private float startResistancePoints = 0f;
    public static float offsetRatio = .5f;

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
    //#if STATICTURRETCOUNTMODE || STATICTURRETRESISTANCEPOINTSMODE
    [SerializeField]
    private float turretCountToResistanceFactor = 15f;
    [SerializeField]
    private float turretResistancePointsToResistanceFactor = 1f;

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
                StartCoroutine(coroutine);
            }
        }
    }
#endif

    // contains the function that computes the resistance points from the number of turrets
    private void updateResistancePoints(float value)
    {
        _turretResistancePoints = value;
        resistancePoints =
            PlayerStatistics.instance.turretResistancePointsToResistanceFactor
            * _turretResistancePoints;
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

#if DYNAMICTURRETRESISTANCEPOINTSMODE
    public static void takeResistance(float amount)
    {
        resistancePoints = Mathf.Min(resistancePoints + amount, defaultMaxResistancePoints);
    }
#endif
}
