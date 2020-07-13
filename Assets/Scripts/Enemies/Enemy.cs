//#define VERBOSEDEBUG
//#define VERBOSEMETRICSLVL2
//#define MUTATIONONDIVISION
#define CHANGEENEMYCOLOR

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Enemy : MonoBehaviour
{
    public const string enemyTag = "EnemyTag";

    [SerializeField]
    private EnemyMovement enemyMovement = null;
    [SerializeField]
    private AudioEmitter audioEmitter = null;

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
    [SerializeField]
    private ParticleSystem resistanceEffect = null;
    [SerializeField]
    private Image resistanceHalo = null;
    private Color resistanceHaloBaseColor = Color.black;
    private float haloBlinkPhase = 0f;
    [SerializeField]
    private float haloBlinkSpeed = 0f;
    private ParticleSystem _resistanceEffectInstance = null;
    public const int maxBurstCount = 15;
    public const int maxEmissionRate = 10;
    // complete immunities
    // Attack.SUBSTANCE-indexed array is faster than Dictionary
    [SerializeField]
    private bool[] immunities = new bool[(int)Attack.SUBSTANCE.COUNT];
    // [0,1] factors applied to antibiotics effects
    // 1f = susceptible
    // 0f = resistant
    // by default, enemies are susceptible, which means the factor applied to the effect is 1f
    public float[] resistances = Enumerable.Repeat(
        1f,
        (int)Attack.SUBSTANCE.COUNT
        ).ToArray();
    // http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/
    [SerializeField]
    private Renderer _renderer = null;
    private MaterialPropertyBlock _propBlock = null;
    private static Color colorSusceptible = Color.white;
    private static Color colorResistant = Color.red;

    #if VERBOSEDEBUG
    public Color currentColor = Color.white;
    #else
    private Color currentColor = Color.white;
    #endif

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
        if (doInitializeHealth)
        {
            health = startHealth;
        }
        isAlive = true;
        divisionCountdown = divisionPeriod;
        _propBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_propBlock);

        showResistance();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (null != RedBloodCellManager.instance)
        {
            EnemyDivisionBlocker blocker = this.gameObject.AddComponent<EnemyDivisionBlocker>();
            blocker.initialize(this);
        }
    }

    private void setUpResistanceEffects()
    {
        if (null == _resistanceEffectInstance)
        {
            _resistanceEffectInstance =
                Instantiate(
                        resistanceEffect.gameObject
                        , this.transform.position
                        , Quaternion.identity
                        , this.transform
                    ).GetComponent<ParticleSystem>();

            setResistanceEffectEmissionRate();
        }
    }

    private float getMaxResistance()
    {
        // the biggest resistance is linked to the smallest resistance factor
        // since 1f => susceptible
        // 0f => resistant
        float maxResistance = 1f;
        for (int i = 0; i < resistances.Length; i++)
        {
            if (immunities[i])
            {
                maxResistance = 0f;
            }
            else if (maxResistance > resistances[i])
            {
                maxResistance = resistances[i];
            }
        }
        return maxResistance;
    }

    private void setResistanceEffectEmissionRate()
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " setResistanceEffectEmissionRate");
        #endif

        if (null != _resistanceEffectInstance)
        {
            float factor = (1f - getMaxResistance());
            //emissionRate = 20;
            //_resistanceEffectInstance.emission.rateOverTime = emissionRate;
            ParticleSystem.EmissionModule em = _resistanceEffectInstance.emission;
            em.rateOverTime = factor * maxEmissionRate;

            #if VERBOSEDEBUG
            Debug.Log("setResistanceEffectEmissionRate " + em.rateOverTime);
            #endif

            resistanceHaloBaseColor = new Color(
                resistanceHalo.color.r,
                resistanceHalo.color.g,
                resistanceHalo.color.b,
                factor //1 opaque, 0 transparent
            );
            resistanceHalo.color = resistanceHaloBaseColor;
            haloBlinkPhase = Random.Range(0f, 2f * Mathf.PI);
            
#if CHANGEENEMYCOLOR
            currentColor = Color.Lerp(colorSusceptible, colorResistant, factor);
            _propBlock.SetColor("_Color", currentColor);
            // Apply the edited values to the renderer.
            _renderer.SetPropertyBlock(_propBlock);

            #if VERBOSEDEBUG
            Debug.Log("setResistanceEffectEmissionRate SetColor currentColor=" + ColorUtility.ToHtmlStringRGBA(currentColor));
            #endif
#endif
        }
    }

    public void doResistanceEffectBurst(int particleCount = maxBurstCount)
    {
        if (null != _resistanceEffectInstance)
        {
            _resistanceEffectInstance.emission.SetBursts(
                new ParticleSystem.Burst[] {
                    new ParticleSystem.Burst(0.0f, particleCount)
                }
            );
            Invoke("cancelBursts", CommonUtilities.getEffectMaxDuration(_resistanceEffectInstance));
            audioEmitter.play(AudioEvent.PATHOGENDEFLECTS);
#if VERBOSEDEBUG
            Debug.Log("doResistanceEffectBurst " + particleCount);
#endif
        }
    }

    private void cancelBursts()
    {
#if VERBOSEDEBUG
        Debug.Log("cancelBursts");
#endif
        if (null != _resistanceEffectInstance)
        {
            _resistanceEffectInstance.emission.SetBursts(
                new ParticleSystem.Burst[] { }
            );
        }
    }

    public void showResistance()
    {
#if VERBOSEDEBUG
        Debug.Log("showResistance");
#endif
        bool instantiateResistanceEffect = false;
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
            /*
            #if VERBOSEDEBUG
                        Debug.Log("Enemy: " + this.gameObject.name
                            + " ABi: " + antibioticIndex
                            + " scale: " + scale
                            );
            #endif
            */
            instantiateResistanceEffect = instantiateResistanceEffect || (0f != scale);

#if VERBOSEDEBUG
            showAntibioticResistanceIndicator(substance, 0f != scale, scale);
#endif
        }

        if (instantiateResistanceEffect && (null == _resistanceEffectInstance))
        {
            setUpResistanceEffects();
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
            divide(enemyMovement.waypointIndex);
            divisionCountdown = divisionPeriod;
        }

        if ((health < startHealth) && isHealingAllowedTotal())
        {
            health = Mathf.Min(startHealth, health + healingRatioSpeed * getHealingFactorTotal() * startHealth * Time.deltaTime);
            updateHealthBar();
        }

        // slow downs are stacked
        enemyMovement.slow(getMovementFactorTotal());

        resistanceHalo.color = new Color(
                                        resistanceHaloBaseColor.r,
                                        resistanceHaloBaseColor.g,
                                        resistanceHaloBaseColor.b,
                                        resistanceHaloBaseColor.a * (2f + Mathf.Cos(haloBlinkPhase + haloBlinkSpeed * Time.timeSinceLevelLoad)) / 3f
                                        );
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
            && (WaveSpawner.instance.isEnemyDivisionAllowed())
            && (canDivideWhileWounded || (health == startHealth))
            && isDivisionAllowedTotal();
    }

    // intialization after procedural instantiation
    public void initialize(
        Wave _wave
        , int _reward
        , float _health
        , float _startHealth
        , int _waypointIndex
        , float[] _resistances
        )
    {
#if VERBOSEDEBUG
        Debug.Log(string.Format("Enemy::initialize({0}, {1}, {2}, {3})",
            //_wave,
            _reward,
            _health,
            _startHealth,
            _waypointIndex
        ));
#endif

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

        if (null != _resistances)
        {
            for (int i = 0; i < (int)Attack.SUBSTANCE.COUNT; i++)
            {
                resistances[i] = Mathf.Clamp(_resistances[i], 0f, 1f);
            }
        }

        updateHealthBar();
        cancelBursts();
        showResistance();
    }

    private void updateHealthBar()
    {
        healthBar.fillAmount = health / startHealth;
    }

    public void takeDamage(float damage, Attack.SOURCE source, Attack.SUBSTANCE substance = Attack.SUBSTANCE.COUNT)
    {
        health -= injuryFactor * damage;
        updateHealthBar();

        if (Attack.SUBSTANCE.COUNT != substance)
        {
            int particleCount = immunities[(int)substance] ? maxBurstCount : (int)Mathf.Floor((1f - resistances[(int)substance]) * maxBurstCount);
            if (0 != particleCount)
            {
                doResistanceEffectBurst(particleCount);
            }
        }

        if (isAlive && health <= 0f)
        {
            RedMetricsManager.instance.sendEvent(
                TrackingEvent.PATHOGENKILLEDBYAB,
                CustomData.getGameObjectContext(this).add(CustomDataTag.SOURCE, source.ToString()));
            AudioManager.instance.play(AudioEvent.PATHOGENKILLEDBYAB);
            die();
        }
    }

    private void die()
    {
        isAlive = false;
        GameObject effect = Instantiate(deathEffect.gameObject, this.transform.position, Quaternion.identity);
        Destroy(effect.gameObject, CommonUtilities.getEffectMaxDuration(deathEffect));
        PlayerStatistics.instance.addMoney(reward, this.gameObject);
        Destroy(this.gameObject);
    }

    public void divide(int waypointIndex)
    {
        if (!isDivisionSafeTotal())
        {
            // insta-death
            takeDamage(health, Attack.SOURCE.FATALDIVISION);
        }
        else if (null != wave)
        {
            #if VERBOSEMETRICSLVL2
            RedMetricsManager.instance.sendEvent(TrackingEvent.PATHOGENDIVIDES, CustomData.getGameObjectContext(this));
            audioEmitter.play(AudioEvent.PATHOGENDIVIDES);
            #endif

            int previousReward = reward;
            reward /= 2;

            // delete resistance effect before division
            if (null != _resistanceEffectInstance)
            {
                _resistanceEffectInstance.transform.parent = null;
            }
            Destroy(_resistanceEffectInstance);
            _resistanceEffectInstance = null;

            Enemy enemy = WaveSpawner.instance.spawnEnemy(
                wave,
                null,
                this.gameObject,
                reward,
                health,
                startHealth,
                waypointIndex
                );

            // put back the resistance effect
            showResistance();

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
#if MUTATIONONDIVISION
                enemy.innerMutate(defaultMutationRange);
#endif
                enemy.enemyMovement.enabled = true;
                enemyMovement.transferWobbleParametersTo(enemy.enemyMovement);

                // worth display coroutine
                StartCoroutine(displayDivisionWorthCoroutine(previousReward, enemy));
            }
        }
        else
        {
            Debug.Log("Tried to divide while wave was unset.");
        }
    }

    private IEnumerator displayDivisionWorthCoroutine(int previousReward, Enemy newCell)
    {
        GameUI.instance.displayWorth(previousReward, this.transform, newCell.transform);
        yield return new WaitForSeconds(.5f);
        displayWorth();
        newCell.displayWorth();
    }

    private void displayWorth()
    {
        GameUI.instance.displayWorth(reward, this.transform);
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
            resistances[i] = Mathf.Clamp(_resistances[i], 0f, 1f);
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
                    bool _canDivideWhileWounded = (Random.Range(0f, 1f) < defaultMutationRange);

                    if (MUTATION_DIRECTION.RANDOM == mutationDirection)
                    {
                        minSeed = -1f;
                        for (int i = 0; i < (int)Attack.SUBSTANCE.COUNT; i++)
                        {
                            _immunities[i] = (Random.Range(0f, 1f) < defaultMutationRange);
                            _resistances[i] = resistances[i] * (1 - defaultMutationRange * Random.Range(minSeed, 1f));
                        }
                    }
                    else // assumes (MUTATION_DIRECTION.MORE_RESISTANCE == directionMode)
                    {
                        minSeed = 0f;
                        for (int i = 0; i < (int)Attack.SUBSTANCE.COUNT; i++)
                        {
                            _immunities[i] = immunities[i] || (Random.Range(0f, 1f) < defaultMutationRange);
                            _resistances[i] = resistances[i] * (1 - defaultMutationRange * Random.Range(minSeed, 1f));
                        }
                        _canDivideWhileWounded = _canDivideWhileWounded || canDivideWhileWounded;
                    }

                    setAllMutable(
                        // if reward is increased, letting pathogens mutate is incentivized
                        startHealth * (1 + defaultMutationRange * Random.Range(minSeed, 1f))
                        , health * (1 + defaultMutationRange * Random.Range(minSeed, 1f))
                        , healingRatioSpeed * (1 + defaultMutationRange * Random.Range(minSeed, 1f))
                        , 1f //, injuryFactor * (1 - defaultMutationRange * Random.Range(minSeed, 1f))
                        , divisionPeriod * (1 - defaultMutationRange * Random.Range(minSeed, 1f))
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
        //bool _canDivideWhileWounded;

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
            divide(waypointIndex);
            divisionCountdown = divisionPeriod;
        }
    }

    public void holdPosition()
    {
        enemyMovement.setHoldingPosition(true);
    }

    public void blockDivision()
    {
        divisionStrategy = DIVISION_STRATEGY.NO_DIVISION;
    }

    public AudioSource play(AudioEvent audioEvent, string parameter = "", bool doPlay = true, AudioClip dontReplay = null)
    {
        return audioEmitter.play(audioEvent, parameter, doPlay, dontReplay);
    }

    void OnDestroy()
    {
        WaveSpawner.instance.enemiesAliveCount--;
    }
}
