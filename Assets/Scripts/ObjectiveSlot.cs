using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSlot : MonoBehaviour
{
    [SerializeField]
    private Collider collider = null;
    public Enemy occupant = null;

    // Update is called once per frame
    void Update()
    {
        if ((null == occupant) && !collider.enabled)
        {
            collider.enabled = true;
        }
        else if ((occupant != null) && collider.enabled)
        {
            collider.enabled = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == Enemy.enemyTag)
        {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            if (null == occupant)
            {
                occupant = enemy;
                enemy.holdPosition();
            }
        }
    }
}
