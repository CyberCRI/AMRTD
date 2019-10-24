using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public static int money;
    [SerializeField]
    private int startMoney = 0;

    // Start is called before the first frame update
    void Start()
    {
        money = startMoney;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
