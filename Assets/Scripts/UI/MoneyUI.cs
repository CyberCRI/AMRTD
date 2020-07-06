using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    public static MoneyUI instance = null;

    [SerializeField]
    private Text moneyText = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        udpateMoneyText(PlayerStatistics.instance.money);
    }

    public void udpateMoneyText(int amount)
    {
        moneyText.text = amount.ToString() + "€";
    }
}
