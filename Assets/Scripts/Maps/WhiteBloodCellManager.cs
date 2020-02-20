using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WhiteBloodCellManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] whiteBloodCells = null;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // manage count of white blood cells
        if (
            (whiteBloodCells.Length > 0)
            && (whiteBloodCells.Length + 1 > PlayerStatistics.instance.lives)
        )
        {
            Destroy(whiteBloodCells[whiteBloodCells.Length - 1]);
            Array.Resize(ref whiteBloodCells, whiteBloodCells.Length - 1);
        }
    }
}
