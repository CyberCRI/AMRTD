//#define VERBOSEDEBUG

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

    void Awake()
    {
        StartCoroutine(autoDestructCoroutine());
    }

    public void setup(int amount, Transform parent, Vector2 screenPosition)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setup(" + amount + ", " + parent.name + ", " + screenPosition + ")");
        #endif

        moneyAmount.text = amount.ToString("+0;-#") + "€";
        moneyAmount.color = (amount < 0) ? negativeColor : ((amount == 0) ? zeroColor : positiveColor);
        rectTransform.SetParent(parent);
        rectTransform.SetAsFirstSibling();
        
        rectTransform.position = screenPosition;
    }

    private IEnumerator autoDestructCoroutine()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }
}
