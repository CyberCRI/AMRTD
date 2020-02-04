using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    [SerializeField]
    private Text moneyText = null;

    // Update is called once per frame
    void Update()
    {
        moneyText.text = PlayerStatistics.instance.money.ToString() + "€";
    }
}
