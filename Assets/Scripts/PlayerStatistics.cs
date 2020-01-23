//#define LIFEPOINTSMODE
#define DEVMODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public static int money;

    public static int lives;

#if LIFEPOINTSMODE
    public static float lifePoints;
    public static float startLifePoints = defaultLifePoints;
    public static float livesToLifePointsFactor;
    public const float defaultLifePoints = 100f;
#endif

    public static float resistancePoints;
    public static float resistanceToLifeFactor;
    public static float offsetRatio = .5f;

    public static int waves;

    public const float defaultMaxResistancePoints = 100f;
    public const float costABPerSec = 1f;

    [SerializeField]
    private float startResistancePoints = 0f;


    [SerializeField]
    private int startMoney = 0;
    [SerializeField]
    private int startLives = 0;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
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

    public static void takeResistance(float amount)
    {
        resistancePoints = Mathf.Min(resistancePoints + amount, defaultMaxResistancePoints);
    }
}
