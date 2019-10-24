using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [SerializeField]
    private Text livesText = null;

    // Update is called once per frame
    void Update()
    {
        livesText.text = PlayerStatistics.lives.ToString() + " LIVES";
    }
}
