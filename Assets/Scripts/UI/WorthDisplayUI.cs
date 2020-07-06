//#define VERBOSEDEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorthDisplayUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform = null;
    [SerializeField]
    private Text moneyAmount = null;
    [SerializeField]
    private float lifetime = 1f;
    [SerializeField]
    private Color currentWorthColor = Color.yellow; //D1CDA7
    [SerializeField]
    private Color pastWorthColor = Color.yellow; //ECE05C
    [SerializeField]
    private int currentFontSize = 15;
    [SerializeField]
    private int pastFontSize = 20;

    private Transform toFollow1 = null;
    private Transform toFollow2 = null;

    void Awake()
    {
        StartCoroutine(autoDestructCoroutine());
    }

    void Update()
    {
        if (null != toFollow1)
        {
            Vector3 tfPosition = toFollow1.position;

            if (null != toFollow2)
            {
                tfPosition += toFollow2.position;
                tfPosition = tfPosition / 2f;
            }

            Vector3 screenPoint = Camera.main.WorldToScreenPoint(tfPosition);
            rectTransform.position = new Vector2(screenPoint.x, screenPoint.y);
        }
    }

    public void setup(int amount, Transform parent, Transform follow1, Transform follow2 = null)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setup(" + amount + ", " + parent.name + ", " + follow1.gameObject.name + ")");
        #endif
        
        rectTransform.SetParent(parent);

        moneyAmount.text = amount.ToString("0") + "€";
        moneyAmount.fontSize = (follow2 == null) ? currentFontSize : pastFontSize;
        moneyAmount.color = (follow2 == null) ? currentWorthColor : pastWorthColor;
        
        toFollow1 = follow1;
        toFollow2 = follow2;
        
    }

    private IEnumerator autoDestructCoroutine()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }
}
