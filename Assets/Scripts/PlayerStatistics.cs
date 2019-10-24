﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public static int money;
    public static int lives;
    
    [SerializeField]
    private int startMoney = 0;
    [SerializeField]
    private int startLives = 0;

    // Start is called before the first frame update
    void Start()
    {
        money = startMoney;
        lives = startLives;
    }
}
