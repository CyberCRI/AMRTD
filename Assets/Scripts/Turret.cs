using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target = null;
    private Enemy enemy = null;

    [Header("General")]
    [SerializeField]
    private float range = 0f;

    [Header("Bullets mode (default)")]
    [SerializeField]
    private GameObject bulletPrefab = null;
    // rate of shooting, per second
    [SerializeField]
    private float fireRate = 0f;
    [SerializeField]
    private float fireCountdown = 0f;

    [Header("Laser mode")]
    [SerializeField]
    private bool userLaser = false;
    [SerializeField]
    private LineRenderer lineRenderer = null;
    [SerializeField]
    private ParticleSystem laserImpactPS = null;
    [SerializeField]
    private Light laserImpactLight = null;
    [SerializeField]
    private float laserImpactOffsetFactor = 0f;
    [SerializeField]
    private int laserDamageOverTime = 0;
    [SerializeField]
    private float slowRatioFactor = 0f;


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
            enemy = nearestEnemy.GetComponent<Enemy>();
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

            if (userLaser)
            {
                laser();
            }
            else
            {
                if (fireCountdown <= 0)
                {
                    shoot();
                    fireCountdown = 1f / fireRate;
                }

                fireCountdown -= Time.deltaTime;
            }
        }
        else
        {
            if (userLaser && lineRenderer.enabled)
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
        // logics
        enemy.takeDamage(laserDamageOverTime * Time.deltaTime);
        enemy.slow(slowRatioFactor);

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
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
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
