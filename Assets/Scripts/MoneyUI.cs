using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    [SerializeField]
    private Text moneyText = null;

    // Update is called once per frame
    void Update()
    {
        moneyText.text = PlayerStatistics.money.ToString() + "€";
    }
}
