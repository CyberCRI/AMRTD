using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public const string enemyTag = "EnemyTag";

    private EnemyMovement enemyMovement;

    [Header("Worth")]
    [SerializeField]
    private int reward = 0;

    [Header("Health")]
    [SerializeField]
    private ParticleSystem deathEffect = null;
    private float health = 0f;
    [SerializeField]
    private float startHealth = 0f;
    [SerializeField]
    private Image healthBar = null;
    private bool isAlive = false;

    [Header("Division")]
    private float divisionCooldown = 0f;
    [SerializeField]
    private float divisionPeriod = 1f;
    // the wave during which this enemy was created; hold info on the prefab
    private Wave wave;
    [SerializeField]
    private DIVISION_STRATEGY divisionStrategy;

    public enum DIVISION_STRATEGY
    {
        TIME_BASED,
        WAYPOINT_BASED
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        health = startHealth;
        isAlive = true;
        divisionCooldown = divisionPeriod;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

        divisionCooldown -= Time.deltaTime;

        if ((DIVISION_STRATEGY.TIME_BASED == divisionStrategy) && (divisionCooldown <= 0))
        {
            divide(enemyMovement.waypointIndex - 1);
            divisionCooldown = divisionPeriod;
        }
    }

    // intialization after procedural instantiation
    public void initialize(Wave _wave, int _reward, int _waypointIndex)
    {
        wave = _wave;
        enemyMovement = this.GetComponent<EnemyMovement>();
        enemyMovement.waypointIndex = _waypointIndex;

        if (0 != _reward)
        {
            reward = _reward;
        }
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / startHealth;

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
            Debug.Log("Division!");
            reward /= 2;
            WaveSpawner.instance.spawnEnemy(wave, reward, waypointIndex, this.transform);
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
        if ((DIVISION_STRATEGY.WAYPOINT_BASED == divisionStrategy) && (divisionCooldown <= 0))
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
