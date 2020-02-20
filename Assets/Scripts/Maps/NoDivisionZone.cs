using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDivisionZone : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == Enemy.enemyTag)
        {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            if (null != enemy)
            {
                enemy.blockDivision();
            }
        }
    }
}
