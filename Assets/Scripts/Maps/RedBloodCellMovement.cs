//#define DETRIMENTALOPTIMIZATION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBloodCellMovement : MonoBehaviour
{
    private Transform bloodOrigin1 = null;
    private Transform bloodOrigin2 = null;
    private Transform bloodEnd1 = null;
    private Transform bloodEnd2 = null;

    [SerializeField]
    private float minimumDistance = 0f;
    public float baseSpeed = 0f;
    [SerializeField]
    private float speedVariation = 0f;
    private float speed = 0f;

    private Vector3 target = Vector3.zero;
    private Vector3 displacement = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] positions = RedBloodCellManager.instance.getBloodPositions();
        bloodOrigin1 = positions[0];
        bloodOrigin2 = positions[1];
        bloodEnd1 = positions[2];
        bloodEnd2 = positions[3];
        
        setTarget();
        setSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if ((target - this.transform.position).magnitude > minimumDistance)
        {
            displacement = (target - this.transform.position).normalized * speed * Time.deltaTime;
            this.transform.Translate(displacement, Space.World);
        }
        else
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
        speed = baseSpeed + Random.Range(-speedVariation, speedVariation);
    }
}
