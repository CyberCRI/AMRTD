using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public static int money;
    public static int lives;
    public static int waves;
    
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
        waves = 0;
    }
}
