//#define DETRIMENTALOPTIMIZATION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBloodCellMovement : WobblyMovement
{
    private static Transform bloodOrigin1 = null;
    private static Transform bloodOrigin2 = null;
    private static Transform bloodEnd1 = null;
    private static Transform bloodEnd2 = null;
    public float baseSpeed = 0f;
    [SerializeField]
    private float speedVariation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (null == bloodOrigin1)
        {
            Transform[] positions = RedBloodCellManager.instance.getBloodPositions();
            bloodOrigin1 = positions[0];
            bloodOrigin2 = positions[1];
            bloodEnd1 = positions[2];
            bloodEnd2 = positions[3];
        }

        setTarget();
        setSpeed();
    }

    protected override void onWobbleDone()
    {
        if ((target - this.transform.position).magnitude <= minimumDistance)
        {
#if DETRIMENTALOPTIMIZATION
            resetPosition();
            setTarget();
            setSpeed();
#else
            Destroy(this.gameObject);
#endif
        }
    }

    private void setTarget()
    {
        float t = Random.Range(0f, 1f);
        target = t * bloodEnd1.position + (1 - t) * bloodEnd2.position;
    }

    private void resetPosition()
    {
        float t = Random.Range(0f, 1f);
        this.transform.position = t * bloodOrigin1.position + (1 - t) * bloodOrigin2.position;
    }

    private void setSpeed()
    {
        startSpeed = baseSpeed + Random.Range(-speedVariation, speedVariation);
    }
}
