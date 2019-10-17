﻿using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [Header("Attributes")]
    [SerializeField]
    private float range;
    // rate of shooting, per second
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float fireCountdown;


    [Header("Unity Step Fields")]
    [SerializeField]
    private string enemyTag;

    [Header("Turret Rotation")]
    [SerializeField]
    private float timeStartTurret;
    [SerializeField]
    private float updatePeriod;
    [SerializeField]
    private Transform partToRotate;
    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    public Transform firePoint;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        InvokeRepeating("updateTarget", timeStartTurret, updatePeriod);
    }

    void updateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        // find closest enemy
        float shortestDistanceToEnemy = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistanceToEnemy)
            {
                shortestDistanceToEnemy = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistanceToEnemy <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (target != null)
        {
            // Target lock on
            Vector3 dir = target.position - this.partToRotate.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
            partToRotate.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);

            if(fireCountdown <= 0)
            {
                shoot();
                fireCountdown = 1f / fireRate;
            }

            fireCountdown -= Time.deltaTime;
        }
    }

    void shoot()
    {
        Debug.Log("Shoot");
        GameObject bulletGO = (GameObject) Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null && target != null)
        {
            bullet.seek(target);
        }
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, range);
    }
}
