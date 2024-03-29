//#define VERBOSEDEBUG
//#define DEVMODE

using UnityEngine;
using System.Collections;

public class PneumocyteManager : GenericEntityGroupManager<Pneumocyte>
{
    public static PneumocyteManager derivedInstance = null;
    
    protected override void Awake()
    {
        base.Awake();
        if (null == derivedInstance)
        {
            derivedInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public float currScore = 0f, maxScore = 0f;

    public float getHealthRatio()
    {
        currScore = 0f;
        for (int i = 0; i < entities.Length; i++)
        {
            currScore += entities[i].currentHealth;
        }

        maxScore = Pneumocyte.maxHealth * entities.Length;

        return currScore / maxScore;
    }
}