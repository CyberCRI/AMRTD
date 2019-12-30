#define DEVMODE
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Enemy : MonoBehaviour
{
    public const string enemyTag = "EnemyTag";

    [SerializeField]
    private EnemyMovement enemyMovement = null;

    // TODO fix issue #36 on wrong enemy collider scale
    /*
    [SerializeField]
    private Transform innerTransform = null;
    [SerializeField]
    private SphereCollider sphereCollider = null;
    */

    [Header("Mutation")]
    [SerializeField]
    private float defaultMutationRange = 0.2f;
    //[SerializeField]
    private MUTATION_VARIABLES mutationVariables = MUTATION_VARIABLES.DIFFERENT_RANDS;
    //[SerializeField]
    private MUTATION_DIRECTION mutationDirection = MUTATION_DIRECTION.RANDOM;

    [Header("Worth")]
    [SerializeField]
    private int reward = 0;

    [Header("Health")]
    public float health = 0f;
    private bool isAlive = false;
    private bool doInitializeHealth = true;
    [SerializeField]
    private ParticleSystem deathEffect = null;
    [SerializeField]
    private float startHealth = 0f;
    // how much of startHealth is regained per second
    [SerializeField]
    private float healingRatioSpeed = 0f;
    // how much of damage is taken
    //[SerializeField]
    private float injuryFactor = 1f;
    [SerializeField]
    private Image healthBar = null;

    [Header("Division")]
    // the wave during which this enemy was created; hold info on the prefab
    private Wave wave = null;
    private float divisionCountdown = 0f;
    [SerializeField]
    private float divisionPeriod = 1f;
    [SerializeField]
    private DIVISION_STRATEGY divisionStrategy = DIVISION_STRATEGY.TIME_BASED;
    [SerializeField]
    private bool canDivideWhileWounded = false;

    [Header("Indicators")]
    [SerializeField]
    private GameObject[] antibioticAttackIndicators = new GameObject[(int)Attack.SUBSTANCE.COUNT];
    [SerializeField]
    private Image[] antibioticResistanceIndicators = new Image[(int)Attack.SUBSTANCE.COUNT];
    [SerializeField]
    private GameObject[] antibioticResistanceIndicatorBackgrounds = new GameObject[(int)Attack.SUBSTANCE.COUNT];

    [Header("Resistance")]
    // complete immunities
    // Attack.SUBSTANCE-indexed array is faster than Dictionary
    [SerializeField]
    private bool[] immunities = new bool[(int)Attack.SUBSTANCE.COUNT];
    // [0,1] factors applied to antibiotics effects
    // by default, enemies are susceptible, which means the factor applied to the effect is 1f
    public float[] resistances = Enumerable.Repeat(
        1f,
        (int)Attack.SUBSTANCE.COUNT
        ).ToArray();

    public enum DIVISION_STRATEGY
    {
        TIME_BASED,
        WAYPOINT_BASED,
        NO_DIVISION
    }

    // current factors applied by antibiotics to the division:
    // is the division currently allowed by the absence of this antibiotic, or presence of a harmless one?
    [HideInInspector]
    public bool[] isDivisionAllowed = Enumerable.Repeat(
        true,
        (int)Attack.SUBSTANCE.COUNT
        ).ToArray();

    // current factors applied by antibiotics to the cell repair:
    // is the healing/cell repair currently allowed by the absence of this antibiotic, or presence of a harmless one?
    [HideInInspector]
    public bool[] isHealingAllowed = Enumerable.Repeat(
        true,
        (int)Attack.SUBSTANCE.COUNT
        ).ToArray();

    // current factors applied by antibiotics to the movement:
    // is the movement currently allowed by the absence of this antibiotic, or presence of a harmless one?
    [HideInInspector]
    public bool[] isMovementAllowed = Enumerable.Repeat(
        true,
        (int)Attack.SUBSTANCE.COUNT
        ).ToArray();

    // current factors applied by antibiotics to the division safety:
    // is the division currently safe or insta-killing due to the presence of any antibiotic?
    [HideInInspector]
    public bool[] isDivisionSafe = Enumerable.Repeat(
        true,
        (int)Attack.SUBSTANCE.COUNT
        ).ToArray();


    // current factors applied by antibiotics to the division speed (duration):
    // the smaller the factor, the smaller the division speed
    [HideInInspector]
    public float[] divisionFactor = Enumerable.Repeat(
        1f,
        (int)Attack.SUBSTANCE.COUNT
        ).ToArray();

    // current factors applied by antibiotics to the healing speed (health points per second):
    // the smaller the factor, the smaller the healing speed
    [HideInInspector]
    public float[] healingFactor = Enumerable.Repeat(
        1f,
        (int)Attack.SUBSTANCE.COUNT
        ).ToArray();

    // current factors applied by antibiotics to the movement speed:
    // the smaller the factor, the smaller the healing speed
    [HideInInspector]
    public float[] movementFactor = Enumerable.Repeat(
        1f,
        (int)Attack.SUBSTANCE.COUNT
        ).ToArray();


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // TODO fix issue #36 on wrong enemy collider scale
        /*
        if (null != innerTransform && null != sphereCollider)
        {
            sphereCollider.radius = innerTransform.localScale.x;
        }
        */

        if (doInitializeHealth)
        {
            health = startHealth;
        }
        isAlive = true;
        divisionCountdown = divisionPeriod;

        Attack.SUBSTANCE substance;
        float scale;
        for (int antibioticIndex = 0; antibioticIndex < (int)Attack.SUBSTANCE.COUNT; antibioticIndex++)
        {
            substance = (Attack.SUBSTANCE)antibioticIndex;
            if (immunities[antibioticIndex])
            {
                scale = 1f;
            }
            else
            {
                scale = 1f - resistances[antibioticIndex];
            }
#if DEVMODE
            showAntibioticResistanceIndicator(substance, 0f != scale, scale);
#endif
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

        divisionCountdown -= Time.deltaTime * getDivisionFactorTotal();

        if (canDivide(DIVISION_STRATEGY.TIME_BASED))
        {
            divide(enemyMovement.waypointIndex - 1);
            divisionCountdown = divisionPeriod;
        }

        if ((health < startHealth) && isHealingAllowedTotal())
        {
            health = Mathf.Min(startHealth, health + healingRatioSpeed * getHealingFactorTotal() * startHealth * Time.deltaTime);
            updateHealthBar();
        }

        // slow downs are stacked
        enemyMovement.slow(getMovementFactorTotal());
    }

    public bool isImmuneTo(Attack.SUBSTANCE antibiotic)
    {
        return immunities[(int)antibiotic] || (0 == resistances[(int)antibiotic]);
    }

    public void showAntibioticAttackIndicator(Attack.SUBSTANCE _substance, bool _show)
    {
        antibioticAttackIndicators[(int)_substance].SetActive(_show);
    }

    public void showAntibioticResistanceIndicator(Attack.SUBSTANCE _substance, bool _show, float scale)
    {
        antibioticResistanceIndicators[(int)_substance].fillAmount = scale;
        antibioticResistanceIndicatorBackgrounds[(int)_substance].SetActive(_show);
    }

    // TODO Optimization
    private bool isMechanismSafeTotal(bool[] _array)
    {
        for (int i = 0; i < (int)Attack.SUBSTANCE.COUNT; i++)
        {
            if (!_array[i])
            {
                return false;
            }
        }
        return true;
    }

    private bool isDivisionAllowedTotal()
    {
        return isMechanismSafeTotal(isDivisionAllowed);
    }

    private bool isHealingAllowedTotal()
    {
        return isMechanismSafeTotal(isHealingAllowed);
    }

    private bool isMovementAllowedTotal()
    {
        return isMechanismSafeTotal(isMovementAllowed);
    }

    private bool isDivisionSafeTotal()
    {
        return isMechanismSafeTotal(isDivisionSafe);
    }

    // TODO Optimization
    private float getMechanismFactorTotal(float[] _array)
    {
        float result = 1f;
        for (int i = 0; i < (int)Attack.SUBSTANCE.COUNT; i++)
        {
            result *= _array[i];
        }
        return result;
    }

    private float getDivisionFactorTotal()
    {
        return getMechanismFactorTotal(divisionFactor);
    }

    private float getHealingFactorTotal()
    {
        return getMechanismFactorTotal(healingFactor);
    }

    private float getMovementFactorTotal()
    {
        return getMechanismFactorTotal(movementFactor);
    }

    private bool canDivide(DIVISION_STRATEGY strategy)
    {
        return (strategy == divisionStrategy)
            && (divisionCountdown <= 0)
            && (WaveSpawner.enemiesAlive < wave.maxEnemyCount)
            && (canDivideWhileWounded || health == startHealth)
            && isDivisionAllowedTotal();
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
        health -= injuryFactor * damage;
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
        if (!isDivisionSafeTotal())
        {
            // insta-death
            takeDamage(health);
        }
        else if (null != wave)
        {
            reward /= 2;
            Enemy enemy = WaveSpawner.instance.spawnEnemy(
                wave,
                this.gameObject,
                reward,
                health,
                startHealth,
                waypointIndex
                );
            if (null != enemy)
            {

                // initialize Attacks
                Attack[] originalAttacks = this.gameObject.GetComponents<Attack>();
                Attack[] instantiatedAttacks = enemy.gameObject.GetComponents<Attack>();

                foreach (Attack iAttack in instantiatedAttacks)
                {
                    foreach (Attack oAttack in originalAttacks)
                    {
                        if (iAttack.substance == oAttack.substance)
                        {
                            iAttack.initialize(true, oAttack, enemy);
                            continue;
                        }
                    }
                }

                enemy.innerMutate(defaultMutationRange);
                enemy.enemyMovement.enabled = true;
                enemyMovement.transferWobblingParametersTo(enemy.enemyMovement);
            }
        }
        else
        {
            Debug.Log("Tried to divide while wave was unset.");
        }
    }

    public enum MUTATION_VARIABLES
    {
        NONE,
        DIFFERENT_RANDS,
        ONE_RAND,
        CONSTANT,
    }

    public enum MUTATION_DIRECTION
    {
        NONE,
        RANDOM,
        MORE_RESISTANCE
    }

    private void setAllMutable(
        float _startHealth
        , float _health
        , float _healingRatioSpeed
        , float _injuryRatio
        , float _divisionPeriod
        , bool _canDivideWhileWounded
        , bool[] _immunities
        , float[] _resistances
    )
    {
        // if reward is increased, letting pathogens mutate is incentivized
        startHealth = _startHealth;
        health = _health;
        health = Mathf.Min(startHealth, health);
        healingRatioSpeed = _healingRatioSpeed;
        injuryFactor = Mathf.Min(1.0f, _injuryRatio);
        divisionPeriod = _divisionPeriod;
        canDivideWhileWounded = _canDivideWhileWounded;
        for (int i = 0; i < (int)Attack.SUBSTANCE.COUNT; i++)
        {
            immunities[i] = _immunities[i];
            resistances[i] = Mathf.Min(1.0f, _resistances[i]);
        }
    }

    public void mutate()
    {
        if (MUTATION_VARIABLES.NONE != mutationVariables
        || MUTATION_DIRECTION.NONE != mutationDirection)
        {
            switch (mutationVariables)
            {
                case MUTATION_VARIABLES.DIFFERENT_RANDS:

                    float minSeed = 0f;
                    bool[] _immunities = new bool[(int)Attack.SUBSTANCE.COUNT];
                    float[] _resistances = new float[(int)Attack.SUBSTANCE.COUNT];
                    bool _canDivideWhileWounded = (Random.Range(0, 1) < defaultMutationRange);

                    if (MUTATION_DIRECTION.RANDOM == mutationDirection)
                    {
                        minSeed = -1f;
                        for (int i = 0; i < (int)Attack.SUBSTANCE.COUNT; i++)
                        {
                            _immunities[i] = (Random.Range(0, 1) < defaultMutationRange);
                            _resistances[i] = resistances[i] * (1 - defaultMutationRange * Random.Range(minSeed, 1));
                        }
                    }
                    else // assumes (MUTATION_DIRECTION.MORE_RESISTANCE == directionMode)
                    {
                        minSeed = 0f;
                        for (int i = 0; i < (int)Attack.SUBSTANCE.COUNT; i++)
                        {
                            _immunities[i] = immunities[i] || (Random.Range(0, 1) < defaultMutationRange);
                            _resistances[i] = resistances[i] * (1 - defaultMutationRange * Random.Range(minSeed, 1));
                        }
                        _canDivideWhileWounded = _canDivideWhileWounded || canDivideWhileWounded;
                    }

                    setAllMutable(
                        // if reward is increased, letting pathogens mutate is incentivized
                        startHealth * (1 + defaultMutationRange * Random.Range(minSeed, 1))
                        , health * (1 + defaultMutationRange * Random.Range(minSeed, 1))
                        , healingRatioSpeed * (1 + defaultMutationRange * Random.Range(minSeed, 1))
                        , 1f //, injuryFactor * (1 - defaultMutationRange * Random.Range(minSeed, 1))
                        , divisionPeriod * (1 - defaultMutationRange * Random.Range(minSeed, 1))
                        , _canDivideWhileWounded
                        , _immunities
                        , _resistances
                    );
                    break;

                case MUTATION_VARIABLES.ONE_RAND:
                    innerMutate(Random.Range(0f, defaultMutationRange));
                    break;
                case MUTATION_VARIABLES.CONSTANT:
                    innerMutate(defaultMutationRange);
                    break;
                default:
                    innerMutate(defaultMutationRange);
                    break;
            }
        }
    }

    private void innerMutate(float mutationFactor)
    {
        bool[] _immunities = new bool[(int)Attack.SUBSTANCE.COUNT];
        float[] _resistances = new float[(int)Attack.SUBSTANCE.COUNT];
        bool _canDivideWhileWounded;

        if (MUTATION_DIRECTION.RANDOM == mutationDirection)
        {
            mutationFactor *= Mathf.Sign(Random.Range(-1f, 1f));
        }
        // else assumes (MUTATION_DIRECTION.MORE_RESISTANCE == mutationDirection)

        float mutationDecreaseFactor = (1 - mutationFactor);
        float mutationIncreaseFactor = (1 + mutationFactor);
        bool mutationBool = (Mathf.Abs(mutationFactor) <= 0.5);

        _immunities = new bool[(int)Attack.SUBSTANCE.COUNT];
        _resistances = new float[(int)Attack.SUBSTANCE.COUNT];
        for (int i = 0; i < (int)Attack.SUBSTANCE.COUNT; i++)
        {
            _resistances[i] = resistances[i] * mutationDecreaseFactor;
            _immunities[i] = immunities[i] || (_resistances[i] < .5f);
        }

        setAllMutable(
            // if reward is increased, letting pathogens mutate is incentivized
            startHealth * mutationIncreaseFactor
            , health * mutationIncreaseFactor
            , healingRatioSpeed * mutationIncreaseFactor
            , 1f //, injuryFactor * mutationDecreaseFactor
            , divisionPeriod * mutationDecreaseFactor
            , mutationBool
            , _immunities
            , _resistances
        );
    }

    public void onReachedWaypoint(int waypointIndex)
    {
        if (canDivide(DIVISION_STRATEGY.WAYPOINT_BASED))
        {
            // "this" already has the waypointIndex-th waypoint as target whereas the newly instantiated hasn't
            divide(waypointIndex - 1);
            divisionCountdown = divisionPeriod;
        }
    }

    public void holdPosition()
    {
        enemyMovement.setHoldingPosition(true);
    }

    void OnDestroy()
    {
        WaveSpawner.enemiesAlive--;
    }
}
