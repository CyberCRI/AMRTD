using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDefenseMode : MonoBehaviour
{
    private bool isLost = false;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Transform objectivesRoot;
    private ObjectiveToDefend[] objectives;

    // Update is called once per frame
    void Update()
    {
        isLost = true;
        for (int i = 0; i < objectives.Length; i++)
        {
            if (!objectives[i].isCaptured)
            {
                isLost = false;
                break;
            }
        }
        if (isLost)
        {
            gameManager.loseLevel();
        }
    }
}
