using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public const string enemyTag = "EnemyTag";

    [SerializeField]
    private EnemyMovement enemyMovement;

    [Header("Worth")]
    [SerializeField]
    private int reward = 0;

    [Header("Health")]
    [SerializeField]
    private ParticleSystem deathEffect = null;
    public float health = 0f;
    [SerializeField]
    private float startHealth = 0f;
    // how much of startHealth is regained per second
    [SerializeField]
    private float healingRatioSpeed = 0f;
    // how much of damage is taken
    [SerializeField]
    private float injuryRatio = 0f;
    [SerializeField]
    private Image healthBar = null;
    private bool isAlive = false;
    private bool doInitializeHealth = true;

    [Header("Division")]
    private float divisionCooldown = 0f;
    [SerializeField]
    private float divisionPeriod = 1f;
    // the wave during which this enemy was created; hold info on the prefab
    private Wave wave = null;
    [SerializeField]
    private DIVISION_STRATEGY divisionStrategy = DIVISION_STRATEGY.TIME_BASED;
    [SerializeField]
    private bool canDivideWhileWounded = false;

    [Header("Indicators")]
    [SerializeField]
    private GameObject antibio0 = null;
    [SerializeField]
    private GameObject antibio1 = null;
    [SerializeField]
    private GameObject antibio2 = null;

    [Header("Resistance")]
    [SerializeField]
    private bool[] immunities = new bool[(int)Attack.SUBSTANCE.ANTIBIOTICS_COUNT];

    /*
        [System.Serializable]
        public class AntibioticDictionary: Dictionary<Attack.SUBSTANCE, bool> {}
        [SerializeField]
        private AntibioticDictionary immunities2 = new AntibioticDictionary()
                                                {
                                                    {Attack.SUBSTANCE.ANTIBIOTIC0, false},
                                                    {Attack.SUBSTANCE.ANTIBIOTIC1, false},
                                                    {Attack.SUBSTANCE.ANTIBIOTIC2, false}
                                                };
    */

    public bool isImmuneTo(Attack.SUBSTANCE antibiotic)
    {
        return immunities[(int)antibiotic];
    }

    public void showIndicator(Attack.SUBSTANCE _substance, bool _show)
    {
        switch (_substance)
        {
            case Attack.SUBSTANCE.ANTIBIOTIC0:
                antibio0.SetActive(_show);
                break;
            case Attack.SUBSTANCE.ANTIBIOTIC1:
                antibio1.SetActive(_show);
                break;
            case Attack.SUBSTANCE.ANTIBIOTIC2:
            default:
                antibio2.SetActive(_show);
                break;
        }
    }

    public enum DIVISION_STRATEGY
    {
        TIME_BASED,
        WAYPOINT_BASED,
        NO_DIVISION
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (doInitializeHealth)
        {
            health = startHealth;
        }
        isAlive = true;
        divisionCooldown = divisionPeriod;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

        divisionCooldown -= Time.deltaTime;

        if (canDivide(DIVISION_STRATEGY.TIME_BASED))
        {
            divide(enemyMovement.waypointIndex - 1);
            divisionCooldown = divisionPeriod;
        }

        if (health < startHealth)
        {
            health = Mathf.Min(startHealth, health + healingRatioSpeed * startHealth * Time.deltaTime);
            updateHealthBar();
        }
    }

    private bool canDivide(DIVISION_STRATEGY strategy)
    {
        return (strategy == divisionStrategy)
            && (divisionCooldown <= 0)
            && (WaveSpawner.enemiesAlive < wave.maxEnemyCount)
            && (canDivideWhileWounded || health == startHealth);
    }

    // intialization after procedural instantiation
    public void initialize(
        Wave _wave,
        int _reward,
        float _health,
        float _startHealth,
        int _waypointIndex
        )
    {
        //Debug.Log(string.Format("Enemy::initialize({0}, {1}, {2}, {3})",
        //    //_wave,
        //    _reward,
        //    _health,
        //    _startHealth,
        //    _waypointIndex
        //));

        wave = _wave;
        enemyMovement.waypointIndex = _waypointIndex;

        if (0 != _reward)
        {
            reward = _reward;
        }

        if (0f != _health)
        {
            health = _health;
            doInitializeHealth = false;
        }

        if (0f != _startHealth)
        {
            startHealth = _startHealth;
        }

        updateHealthBar();
    }

    private void updateHealthBar()
    {
        healthBar.fillAmount = health / startHealth;
    }

    public void takeDamage(float damage)
    {
        health -= injuryRatio * damage;
        updateHealthBar();

        if (isAlive && health <= 0f)
        {
            die();
        }
    }

    private void die()
    {
        isAlive = false;
        GameObject effect = (GameObject)Instantiate(deathEffect.gameObject, this.transform.position, Quaternion.identity);
        Destroy(effect.gameObject, deathEffect.main.duration + deathEffect.main.startLifetime.constant);
        PlayerStatistics.money += reward;
        Destroy(this.gameObject);
    }

    public void divide(int waypointIndex)
    {
        if (null != wave)
        {
            reward /= 2;
            WaveSpawner.instance.spawnEnemy(
                wave, reward, health, startHealth, waypointIndex, this.transform);
        }
        else
        {
            Debug.Log("Tried to divide while wave was unset.");
        }
    }

    public void slow(float slowRatioFactor)
    {
        enemyMovement.slow(slowRatioFactor);
    }

    public void onReachedWaypoint(int waypointIndex)
    {
        if (canDivide(DIVISION_STRATEGY.WAYPOINT_BASED))
        {
            // "this" already has the waypointIndex-th waypoint as target whereas the newly instantiated hasn't
            divide(waypointIndex - 1);
            divisionCooldown = divisionPeriod;
        }
    }

    void OnDestroy()
    {
        WaveSpawner.enemiesAlive--;
    }
}
