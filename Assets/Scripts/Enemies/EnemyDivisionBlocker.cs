using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyDivisionBlocker : MonoBehaviour
{
    private static float xLimit = 0f;
    private Enemy enemy = null;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (this.transform.position.x < xLimit)
        {
            if (null == enemy)
            {
                enemy = this.GetComponent<Enemy>();
            }
            enemy.blockDivision();
            this.enabled = false;
            Destroy(this);
        }
    }

    public void initialize(Enemy _enemy)
    {
        enemy = _enemy;
        if (0f == xLimit)
        {
            xLimit = RedBloodCellManager.instance.bloodOrigin2.position.x;
        }
    }
}
