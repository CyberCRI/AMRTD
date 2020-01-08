using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Player resistance bar
// Fills up when too many towers are used
public class ResistanceBarUI : MonoBehaviour
{
    [SerializeField]
    private Image resistanceBar = null;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // update GUI element
        resistanceBar.fillAmount = PlayerStatistics.resistancePoints / PlayerStatistics.defaultMaxResistancePoints;
    }
}
