using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveToDefend : MonoBehaviour
{
    public bool isCaptured = false;
    [SerializeField]
    private Transform slotsRoot = null;
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
    }

    public ObjectiveSlot getFreeSlot()
    {
        ObjectiveSlot result = null;
        for (int i = 0; i < slots.Length; i++)
        {
            if (null == slots[i].occupant)
            {
                result = slots[i];
                break;
            }
        }

        return result;
    }
}
