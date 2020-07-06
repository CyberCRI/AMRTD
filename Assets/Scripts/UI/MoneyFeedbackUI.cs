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

    public void setup(int amount, Transform parent, Vector2 screenPosition)
    {
        Debug.Log(this.GetType() + " setup(" + amount + ", " + parent.name + ", " + screenPosition + ")");
        moneyAmount.text = amount.ToString("+#;-#") + "€";
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
