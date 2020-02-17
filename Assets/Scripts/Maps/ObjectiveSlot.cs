using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSlot : MonoBehaviour
{
    [SerializeField]
    private Collider slotCollider = null;
    private Enemy occupant = null;
    public bool available {
        get {
            return null == occupant;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((null == occupant) && !slotCollider.enabled)
        {
            slotCollider.enabled = true;
        }
        else if ((occupant != null) && slotCollider.enabled)
        {
            slotCollider.enabled = false;
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
