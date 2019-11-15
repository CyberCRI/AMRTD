//#define DEVMODE
using UnityEngine;

public class Turret : Attacker
{
    public enum ROUTE_OF_ADMINISTRATION
    {
        TOPICAL_SKIN,
        ENTERAL, // gastrointestinal
        INJECTION,
        INHALED
    }

    [Header("General")]
    [SerializeField]
    private float range = 0f;
    public ROUTE_OF_ADMINISTRATION route;

    [Header("Bullets mode (default)")]
    [SerializeField]
    private GameObject bulletPrefab = null;
    // Cooldown time, in seconds (time between two attacks)
    [SerializeField]
    private float fireCooldown = 0f;
    private float fireCountdown = 0f;

    [Header("Laser mode")]
    [SerializeField]
    private bool useLaser = false;
    [SerializeField]
    private LineRenderer lineRenderer = null;
    [SerializeField]
    private ParticleSystem laserImpactPS = null;
    [SerializeField]
    private Light laserImpactLight = null;
    [SerializeField]
    private float laserImpactOffsetFactor = 0f;


    [Header("Unity Step Fields")]

    [Header("Turret Rotation")]
    [SerializeField]
    private float timeStartTurret = 0f;
    [SerializeField]
    private float updatePeriod = 0f;
    [SerializeField]
    private Transform partToRotate = null;
    [SerializeField]
    private float rotationSpeed = 0f;

    [SerializeField]
    public Transform firePoint = null;

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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Enemy.enemyTag);

        // find closest enemy
        float shortestDistanceToEnemy = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemyGO in enemies)
        {
            float distanceToEnemy = Vector3.Distance(this.transform.position, enemyGO.transform.position);
            if (distanceToEnemy < shortestDistanceToEnemy)
            {
                shortestDistanceToEnemy = distanceToEnemy;
                nearestEnemy = enemyGO;
            }
        }

        if (nearestEnemy != null && shortestDistanceToEnemy <= range)
        {
            if (nearestEnemy.transform != target)
            {
#if DEVMODE
                Debug.Log("firstAttack");
#endif
                firstAttack = true;
                target = nearestEnemy.transform;
                enemy = nearestEnemy.GetComponent<Enemy>();
            }
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
            lockOnTarget();

            if (useLaser)
            {
                doAttack(target, enemy);
                laser();
            }
            else
            {
                if (fireCountdown <= 0)
                {
                    shoot();
                    fireCountdown = fireCooldown;
                }

                fireCountdown -= Time.deltaTime;
            }
        }
        else
        {
            if (useLaser && lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                laserImpactPS.Stop();
                laserImpactLight.enabled = false;
            }
        }
    }

    public Quaternion getPartToRotateRotation()
    {
        return partToRotate.rotation;
    }

    public void rotatePartToRotate(Quaternion rotation)
    {
        partToRotate.rotation = rotation;
    }

    void lockOnTarget()
    {
        // Target lock on
        Vector3 dir = target.position - this.partToRotate.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
    }

    void laser()
    {
        // graphics
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            laserImpactPS.Play();
            laserImpactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;
        laserImpactPS.transform.position = target.position + dir.normalized * target.transform.localScale.x * laserImpactOffsetFactor;
        laserImpactPS.transform.rotation = Quaternion.LookRotation(dir);
    }

    void shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation);

        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null && target != null)
        {
            bullet.initialize(modelAttack);
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
