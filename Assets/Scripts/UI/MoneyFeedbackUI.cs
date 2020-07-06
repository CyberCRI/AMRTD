using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyFeedbackUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform = null;
    [SerializeField]
    private Text moneyAmount = null;
    [SerializeField]
    private float lifetime = 1f;
    [SerializeField]
    private Color positiveColor = Color.green;
    [SerializeField]
    private Color zeroColor = Color.yellow;
    [SerializeField]
    private Color negativeColor = Color.red;

    public void setup(int amount, Transform parent, Vector2 screenPosition)
    {
        Debug.Log(this.GetType() + " setup(" + amount + ", " + parent.name + ", " + screenPosition + ")");
        moneyAmount.text = amount.ToString("+0;-#") + "€";
        moneyAmount.color = (amount < 0) ? negativeColor : ((amount == 0) ? zeroColor : positiveColor);
        rectTransform.SetParent(parent);
        //rectTransform.anchoredPosition = screenPosition;
        rectTransform.position = screenPosition;
    }

    void Awake()
    {
        StartCoroutine(autoDestructCoroutine());
    }

    private IEnumerator autoDestructCoroutine()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }
}
