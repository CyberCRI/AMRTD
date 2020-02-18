﻿#define SELLTURRETS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurretBlueprint
{
    public GameObject prefab;
    public int cost;

    public GameObject upgradePrefab;
    public int upgradeCost;

#if SELLTURRETS
    public int getSellCost()
    {
        return cost / 2;
    }
#endif
}