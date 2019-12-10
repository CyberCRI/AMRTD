using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDefenseMode : MonoBehaviour
{
    public static ObjectiveDefenseMode instance;
    private int capturedObjectivesCount = 0;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Transform objectivesRoot;
    private ObjectiveToDefend[] objectives;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CommonUtilities.fillArrayFromRoot<ObjectiveToDefend>(objectivesRoot, ref objectives);
    }

    // Update is called once per frame
    void Update()
    {
        capturedObjectivesCount = 0;
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i].isCaptured)
            {
                capturedObjectivesCount++;
            }
        }
        if (capturedObjectivesCount == objectives.Length)
        {
            gameManager.loseLevel();
        }
    }

    public int getFreeObjectiveToDefend()
    {
        int result = -1;
        for (int i = 0; i < objectives.Length; i++)
        {
            if (!objectives[i].isCaptured)
            {
                result = i;
                break;
            }
        }

        return result;
    }

    public Vector3 getFreeSlotPosition()
    {
        Vector3 position = Vector3.positiveInfinity;
        for (int i = 0; i < objectives.Length; i++)
        {
            if (!objectives[i].isCaptured)
            {
                position = objectives[i].getFreeSlot().transform.position;
                break;
            }
        }

        return position;
    }

    public string getCapturedObjectivesStats()
    {
        return (objectives.Length - capturedObjectivesCount).ToString("00") + "/" + objectives.Length.ToString("00");
    }
}
