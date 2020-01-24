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

    #region turret points
//#if STATICTURRETCOUNTMODE || STATICTURRETRESISTANCEPOINTSMODE
    [SerializeField]
    private float turretCountToResistanceFactor = 15f;
    [SerializeField]
    private float turretResistancePointsToResistanceFactor = 1f;

    public static int _turretCount = 0;
    public static float _turretResistancePoints = 0f;

#if STATICTURRETCOUNTMODE
    public static int turretCount
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
    public static float turretResistancePoints
    {
        get { return _turretResistancePoints; }
        set
        {
            _turretResistancePoints = value;
            resistancePoints = PlayerStatistics.instance.turretResistancePointsToResistanceFactor * _turretResistancePoints;
        }
    }
#endif

//#endif
    #endregion

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
