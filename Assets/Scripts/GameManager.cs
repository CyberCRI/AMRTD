using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isGameOver = false;

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            if (PlayerStatistics.lives <= 0)
            {
                endGame();
            }
        }
    }

    private void endGame()
    {
        isGameOver = true;
        Debug.Log("Game over!");
    }
}
