using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveToDefend : MonoBehaviour
{
    public bool isCaptured = false;
    [SerializeField]
    private Transform slotsRoot = null;
    [SerializeField]
    private Collider collider = null;
    private ObjectiveSlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        CommonUtilities.fillArrayFromRoot<ObjectiveSlot>(slotsRoot, ref slots);
    }

    // Update is called once per frame
    void Update()
    {
        isCaptured = (null == getFreeSlot());

        if (!isCaptured && !collider.enabled)
        {
            collider.enabled = true;
        }
        else if (isCaptured && collider.enabled)
        {
            collider.enabled = false;
        }
    }

    private ObjectiveSlot getFreeSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (null == slots[i].occupant)
            {
                return slots[i];
            }
        }

        return null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == Enemy.enemyTag)
        {
            Enemy enemy = collision.transform.GetComponent<Enemy>();
            ObjectiveSlot freeSlot = getFreeSlot();
            if (null != freeSlot)
            {
                freeSlot.occupant = enemy;
                enemy.holdPosition();
            }
        }
    }
}
