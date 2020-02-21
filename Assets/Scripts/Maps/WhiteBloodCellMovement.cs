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
    private Vector3 displacement = Vector3.zero;
    private float maxDisplacement = 0f;
    private float zDisplacement = 0f;
    private float xDisplacement = 0f;
    private static Transform bloodOrigin2 = null;

    private Vector3 idlePosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        //setTarget();
        setSpeed();

        if (null == bloodOrigin2)
        {
            bloodOrigin2 = RedBloodCellManager.instance.bloodOrigin2;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (null != target)
        {
            if ((target.position - this.transform.position).magnitude > minimumDistance)
            {
                if (target.position.x > bloodOrigin2.position.x)
                {
                    displacement = (new Vector3(bloodOrigin2.position.x, target.position.y, target.position.z) - this.transform.position).normalized * speed * Time.deltaTime;
                }
                else
                {
                    displacement = (target.position - this.transform.position).normalized * speed * Time.deltaTime;
                }
                this.transform.Translate(displacement, Space.World);
            }
            else
            {
                Destroy(this.gameObject);
                Destroy(target.gameObject);
            }
        }
        else
        {
            // go to idle position
            if ((idlePosition - this.transform.position).magnitude > minimumDistance)
            {
                displacement = (idlePosition - this.transform.position).normalized * speed * Time.deltaTime;
                this.transform.Translate(displacement, Space.World);
            }
        }
    }

    public void setTarget(Transform _target)
    {
        target = _target;
    }

    private void setSpeed()
    {
        speed = baseSpeed + Random.Range(-speedVariation, speedVariation);
    }

    public void initialize(Vector3 _idlePosition)
    {
        idlePosition = _idlePosition;
    }

    void OnDestroy()
    {
        PlayerStatistics.instance.lives--;
    }
}
