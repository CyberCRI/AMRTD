using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBloodCellMovement : MonoBehaviour
{

    [SerializeField]
    private float minimumDistance = 0f;
    public float baseSpeed = 0f;
    [SerializeField]
    private float speedVariation = 0f;
    private float speed = 0f;

    private Transform target = null;
    private float displacement = 0f;

    // Start is called before the first frame update
    void Start()
    {
        setTarget();
        setSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if ((null == target) && (0 < WaveSpawner.enemiesAliveCount))
        {
            setTarget();
        }

        if (null != target)
        {
            if ((target.position - this.transform.position).magnitude > minimumDistance)
            {
                displacement = ((target.position - this.transform.position).normalized).z * speed * Time.deltaTime;
                this.transform.Translate(new Vector3(0f, 0f, displacement), Space.World);
            }
            else
            {
                Destroy(this.gameObject);
                Destroy(target.gameObject);
            }
        }
    }

    private void setTarget()
    {
        for (int i = 0; i < WaveSpawner.enemiesAlive.Length; i++)
        {
            if (null != WaveSpawner.enemiesAlive[i])
            {
                target = WaveSpawner.enemiesAlive[i].transform;
                break;
            }
        }
    }

    private void setSpeed()
    {
        speed = baseSpeed + Random.Range(-speedVariation, speedVariation);
    }
}
