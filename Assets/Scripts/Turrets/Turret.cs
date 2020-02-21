//#define DEVMODE
//#define STATICTURRETCOUNTMODE
#define STATICTURRETRESISTANCEPOINTSMODE
//#define DYNAMICTURRETRESISTANCEPOINTSMODE
//#define TURRETUPKEEP
//#define TURRETLIFETIME
//#define SHOOTCLOSEST
using UnityEngine;
using UnityEngine.UI;

public class Turret : Attacker
{
    public const string turretTag = "TurretTag";

    public enum ROUTE_OF_ADMINISTRATION
    {
        TOPICAL_SKIN,
        ENTERAL, // gastrointestinal
        INJECTION,
        INHALED
    }

    [Header("General")]
    public float range = 0f;
    public ROUTE_OF_ADMINISTRATION route = ROUTE_OF_ADMINISTRATION.TOPICAL_SKIN;
    private bool isBeingUpgraded = false;
    private bool isUpgraded = false;

    //#if STATICTURRETRESISTANCEPOINTSMODE
    [Header("Resistance")]
    [Tooltip("Points added to a pool taken into account for the computation of the resistance gauge.")]
    [SerializeField]
    private float _resistancePoints = 15f;
    //#endif

    //#if TURRETUPKEEP
    [Header("Upkeep")]
    // amount of money paid every upkeepPeriod seconds
    [SerializeField]
    public int upkeepCost = 0;
    [SerializeField]
    private float upkeepPeriod = 0f;
    private float upkeepCountdown = 0f;
    //#endif

    //#if TURRETLIFETIME
    [Header("Lifetime")]
    public float lifetimeStart = 0;
    [SerializeField]
    private float lifetimeRemaining = 0;
    public Node node;
    [SerializeField]
    private Image lifetimeBar = null;
    //#endif

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
#if TURRETLIFETIME
        lifetimeRemaining = lifetimeStart;
#else
        lifetimeBar.fillAmount = 0f;
#endif
#if TURRETUPKEEP
        upkeepCountdown = upkeepPeriod;
#endif
#if STATICTURRETCOUNTMODE
        if (!isUpgraded)
        {
            PlayerStatistics.instance.turretCount += 1;
        }
#endif
#if STATICTURRETRESISTANCEPOINTSMODE
        if (!isUpgraded)
        {
            PlayerStatistics.instance.turretResistancePoints += _resistancePoints;
        }
#endif
        InvokeRepeating("updateTarget", timeStartTurret, updatePeriod);
    }

    /// <summary>
    /// OnMouseDown is called when the user has pressed the mouse button while
    /// over the GUIElement or Collider.
    /// </summary>
    void OnMouseDown()
    {
        node.manageClick();
    }

    public void renew(float duration = 10f)
    {
#if TURRETLIFETIME
        lifetimeRemaining += duration;
        lifetimeStart = Mathf.Max(lifetimeStart, lifetimeRemaining);
#endif
    }

#if DYNAMICTURRETRESISTANCEPOINTSMODE
    private void applyResistanceCost(float deltaTime)
    {
        PlayerStatistics.instance.takeResistance(deltaTime * PlayerStatistics.instance.costABPerSec);
    }
#endif

#if TURRETLIFETIME
    private void updateLifetimeBar()
    {
        lifetimeBar.fillAmount = lifetimeRemaining / lifetimeStart;
    }
#endif

    void updateTarget()
    {
#if !SHOOTCLOSEST
        if (null == target)
        {
#endif
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
                    firstAttack = true;
                    target = nearestEnemy.transform;
                    enemy = nearestEnemy.GetComponent<Enemy>();
                }
            }
            else
            {
                target = null;
            }
#if !SHOOTCLOSEST
        }
#endif
    }

#if DEVMODE
    public void selfDestruct(Node.REMOVETOWER reason)
#else
    private void selfDestruct(Node.REMOVETOWER reason)
#endif
    {
        node.removeTurret(reason, this.gameObject);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
#if DEVMODE && TURRETLIFETIME
        if (Input.GetKeyDown(KeyCode.Space))
        {
            renew(10f);
        }
#endif

#if TURRETLIFETIME
        if (lifetimeRemaining <= 0)
        {
            selfDestruct(Node.REMOVETOWER.EXPIRED);
        }
        updateLifetimeBar();
        lifetimeRemaining -= Time.deltaTime;
#endif

#if DYNAMICTURRETRESISTANCEPOINTSMODE
        applyResistanceCost(Time.deltaTime);
#endif

#if TURRETUPKEEP
            if (upkeepCountdown <= 0)
            {
                if (PlayerStatistics.instance.money < upkeepCost)
                {
                    selfDestruct(Node.REMOVETOWER.CANTPAY);
                    //TODO fix this
                    return;
                }
                else
                {
                    PlayerStatistics.instance.money -= upkeepCost;
                    upkeepCountdown = upkeepPeriod;
                }
            }
            else
            {
                upkeepCountdown -= Time.deltaTime;
            }
#endif

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
        dir = new Vector3(dir.x, 0f, dir.z);
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
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

    public void setBeingUpgraded(bool value)
    {
        isBeingUpgraded = value;
    }
    public void setUpgraded(bool value)
    {
        isUpgraded = value;
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, range);
    }

    public float getResistancePoints()
    {
        return _resistancePoints;
    }

#if STATICTURRETCOUNTMODE || STATICTURRETRESISTANCEPOINTSMODE
    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
#if STATICTURRETCOUNTMODE
        if (!isBeingUpgraded)
        {
            PlayerStatistics.instance.turretCount -= 1;
        }
#endif
#if STATICTURRETRESISTANCEPOINTSMODE
        if (!isBeingUpgraded)
        {
            PlayerStatistics.instance.turretResistancePoints -= _resistancePoints;
        }
#endif
    }
#endif
}
