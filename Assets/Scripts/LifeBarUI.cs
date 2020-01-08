using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Overall player lifebar
// Takes into account starting lives that are decreased when an enemy slips through
// but also decreases when too many towers are used
public class LifeBarUI : MonoBehaviour
{
    [SerializeField]
    private Image lifeBar = null;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // update GUI element
        lifeBar.fillAmount = PlayerStatistics.lifePoints / PlayerStatistics.startLifePoints;
    }
}
