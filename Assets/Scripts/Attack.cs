//#define DEVMODE
using UnityEngine;

// temporary class, will be turned into a ScriptableObject
public class Attack : MonoBehaviour
{
    public enum SUBSTANCE
    {
        ANTIBIOTIC0,
        ANTIBIOTIC1,
        ANTIBIOTIC2,

        // add new antibiotics in-between

        COUNT
    }
    ///TODO link SUBSTANCEs to their effects: table, dict, ...
    public SUBSTANCE substance;

    private Enemy enemy;
    //    private EnemyMovement enemyMovement;
    private bool onEnemy = false;

    [SerializeField]
    private float remainingDurationCountdown;
    [SerializeField]
    private float attackDuration = 0f;

    // active
    // needs a constant channelling or a punctual attack,
    // for instance a constant attack like a laser one
    [Header("Active")]
    private bool wasApplied = false;
    [Tooltip("Is applied only once")]
    [SerializeField]
    private float damageWhenFirstHit = 0f;
    [SerializeField]
    private float damageWhenHit = 0f;
    [SerializeField]
    private float damagePerSecondActive = 0f;
    [Space(20)]
    [SerializeField]
    private float slowDownDivisionFactorActive = 1f;
    [SerializeField]
    private float slowDownHealingFactorActive = 1f;
    [SerializeField]
    private float slowDownMovementFactorActive = 1f;
    [SerializeField]
    private bool blockDivisionActive = false;
    [SerializeField]
    private bool blockHealingActive = false;
    [SerializeField]
    private bool blockMovementActive = false;
    [SerializeField]
    private bool killAtDivisionActive = false;

    // passive
    // happens all the time while the Attack exists
    [Header("Passive")]
    [SerializeField]
    private float damagePerSecondPassive = 0f;
    [SerializeField]
    private float slowDownDivisionFactorPassive = 1f;
    [SerializeField]
    private float slowDownHealingFactorPassive = 1f;
    [SerializeField]
    private float slowDownMovementFactorPassive = 1f;
    [SerializeField]
    private bool blockDivisionPassive = false;
    [SerializeField]
    private bool blockHealingPassive = false;
    [SerializeField]
    private bool blockMovementPassive = false;
    [SerializeField]
    private bool killAtDivisionPassive = false;

    // TODO
    // condition for booleans similar to (Random.Range(0, .5) < resistanceFactor)?

    private void initialize(
         //        EnemyMovement _enemyMovement
         bool _onEnemy
        , Enemy _enemy
        , SUBSTANCE _substance

        , float _attackDuration = 0f

        // active
        , float _damageWhenFirstHit = 0f
        , float _damageWhenHit = 0f
        , float _damagePerSecondActive = 0f
        , float _slowDownDivisionFactorActive = 1f
        , float _slowDownHealingFactorActive = 1f
        , float _slowDownMovementFactorActive = 1f
        , bool _blockDivisionActive = false
        , bool _blockHealingActive = false
        , bool _blockMovementActive = false
        , bool _killAtDivisionActive = false

        // passive
        , float _damagePerSecondPassive = 0f
        , float _slowDownDivisionFactorPassive = 1f
        , float _slowDownHealingFactorPassive = 1f
        , float _slowDownMovementFactorPassive = 1f
        , bool _blockDivisionPassive = false
        , bool _blockHealingPassive = false
        , bool _blockMovementPassive = false
        , bool _killAtDivisionPassive = false

        , float resistanceFactor = 1f
         )
    {
        //        enemyMovement = _enemyMovement;
        onEnemy = _onEnemy;
        enemy = _enemy;
        substance = _substance;

        attackDuration = _attackDuration * resistanceFactor;
        remainingDurationCountdown = _attackDuration * resistanceFactor;


        // active
        damageWhenFirstHit =            getOverallEffectValue(_damageWhenFirstHit, resistanceFactor);
        damageWhenHit =                 getOverallEffectValue(_damageWhenHit, resistanceFactor);
        damagePerSecondActive =         getOverallEffectValue(_damagePerSecondActive, resistanceFactor);
        slowDownDivisionFactorActive =  getOverallEffectFactor(_slowDownDivisionFactorActive, resistanceFactor);
        slowDownHealingFactorActive =   getOverallEffectFactor(_slowDownHealingFactorActive, resistanceFactor);
        slowDownMovementFactorActive =  getOverallEffectFactor(_slowDownMovementFactorActive, resistanceFactor);
        blockDivisionActive = _blockDivisionActive;
        blockHealingActive = _blockHealingActive;
        blockMovementActive = _blockMovementActive;
        killAtDivisionActive = _killAtDivisionActive;

        // passive
        damagePerSecondPassive = getOverallEffectValue(_damagePerSecondPassive, resistanceFactor);
        slowDownDivisionFactorPassive = getOverallEffectFactor(_slowDownDivisionFactorPassive, resistanceFactor);
        slowDownHealingFactorPassive =  getOverallEffectFactor(_slowDownHealingFactorPassive, resistanceFactor);
        slowDownMovementFactorPassive = getOverallEffectFactor(_slowDownMovementFactorPassive, resistanceFactor);
        blockDivisionPassive = _blockDivisionPassive;
        blockHealingPassive = _blockHealingPassive;
        blockMovementPassive = _blockMovementPassive;
        killAtDivisionPassive = _killAtDivisionPassive;
    }

    public void initialize(
         //        EnemyMovement _enemyMovement
         bool _onEnemy
        , Attack _attack
        , Enemy _enemy
        , float resistanceFactor = 1f
        )
    {
        initialize(
         //        _enemyMovement
         _onEnemy
        , _enemy
        , _attack.substance

        , _attack.attackDuration

        // active
        , _attack.damageWhenFirstHit
        , _attack.damageWhenHit
        , _attack.damagePerSecondActive
        , _attack.slowDownDivisionFactorActive
        , _attack.slowDownHealingFactorActive
        , _attack.slowDownMovementFactorActive
        , _attack.blockDivisionActive
        , _attack.blockHealingActive
        , _attack.blockMovementActive
        , _attack.killAtDivisionActive

        // passive
        , _attack.damagePerSecondPassive
        , _attack.slowDownDivisionFactorPassive
        , _attack.slowDownHealingFactorPassive
        , _attack.slowDownMovementFactorPassive
        , _attack.blockDivisionPassive
        , _attack.blockHealingPassive
        , _attack.blockMovementPassive
        , _attack.killAtDivisionPassive

        , resistanceFactor
        );
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (onEnemy)
        {
#if DEVMODE
            enemy.showAntibioticAttackIndicator(substance, true);
#endif

            if (0f != damageWhenFirstHit)
            {
                // resistance is taken into account at creation of Attack
                enemy.takeDamage(damageWhenFirstHit, substance);
                damageWhenFirstHit = 0f;
            }

            // set all passive abilities
            enemy.divisionFactor[(int)substance] = slowDownDivisionFactorPassive;
            enemy.healingFactor[(int)substance] = slowDownHealingFactorPassive;
            enemy.movementFactor[(int)substance] = slowDownMovementFactorPassive;
            enemy.isDivisionAllowed[(int)substance] = !blockDivisionPassive;
            enemy.isHealingAllowed[(int)substance] = !blockHealingPassive;
            enemy.isMovementAllowed[(int)substance] = !blockMovementPassive;
            enemy.isDivisionSafe[(int)substance] = !killAtDivisionPassive;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (onEnemy)
        {
            remainingDurationCountdown -= Time.deltaTime;
            if (remainingDurationCountdown <= 0)
            {
#if DEVMODE
//                Debug.Log("Attack is over:" + substance);
                enemy.showAntibioticAttackIndicator(substance, false);
#endif

                // set off active and passive abilities
                enemy.divisionFactor[(int)substance] = 1f;
                enemy.healingFactor[(int)substance] = 1f;
                enemy.movementFactor[(int)substance] = 1f;
                enemy.isDivisionAllowed[(int)substance] = true;
                enemy.isHealingAllowed[(int)substance] = true;
                enemy.isMovementAllowed[(int)substance] = true;
                enemy.isDivisionSafe[(int)substance] = true;

                Destroy(this);
            }

            if (0f != damagePerSecondPassive)
            {
                enemy.takeDamage(damagePerSecondPassive * Time.deltaTime, substance);
            }

            // TODO make sure this does not happen too soon for Enemy to take it into account
            // Change script execution order?
            if (wasApplied)
            {
                wasApplied = false;
            }
            else
            {
                // revert active abilities
                enemy.divisionFactor[(int)substance] = slowDownDivisionFactorPassive;
                enemy.healingFactor[(int)substance] = slowDownHealingFactorPassive;
                enemy.movementFactor[(int)substance] = slowDownMovementFactorPassive;
                enemy.isDivisionAllowed[(int)substance] = !blockDivisionPassive;
                enemy.isHealingAllowed[(int)substance] = !blockHealingPassive;
                enemy.isMovementAllowed[(int)substance] = !blockMovementPassive;
                enemy.isDivisionSafe[(int)substance] = !killAtDivisionPassive;
            }
        }
    }

    // computes the overall antibiotic effect [0, 1] factor applied
    // works only for [0, 1] mechanism factors
    // eg movement speed is
    //   speed                      ie overallEffectFactor = 1f with no antibiotic (antibioticEffect = 1f)
    //   speed * antibioticEffect   ie overallEffectFactor = antibioticEffect with antibiotic and no resistance ie antibioticResistanceFactor = 1f
    //   speed                      ie overallEffectFactor = 1f with antibiotic and full resistance ie antibioticResistanceFactor = 0
    private float getOverallEffectFactor(float antibioticEffect, float antibioticResistanceFactor)
    {
        return antibioticEffect + (1f - antibioticEffect) * (1f - antibioticResistanceFactor);
    }

    // computes the overall antibiotic effect value
    // works only for effect values that are not [0, 1] mechanism factors
    // eg damage being dealt
    //   0 if no antibiotic
    //   damage if antibiotic and no resistance ie antibioticResistanceFactor = 1f
    //   0 if full resistance ie antibioticResistanceFactor = 0
    private float getOverallEffectValue(float antibioticEffect, float antibioticResistanceFactor)
    {
        return antibioticEffect * antibioticResistanceFactor;
    }

    public void apply()
    {
        if (0f != damagePerSecondActive)
        {
            enemy.takeDamage(damagePerSecondActive * Time.deltaTime, substance);
        }

        if (0f != damageWhenHit)
        {
            enemy.takeDamage(damageWhenHit, substance);
        }

        // apply active abilities
        enemy.divisionFactor[(int)substance] = slowDownDivisionFactorPassive * slowDownDivisionFactorActive;
        enemy.healingFactor[(int)substance] = slowDownHealingFactorPassive * slowDownHealingFactorActive;
        enemy.movementFactor[(int)substance] = slowDownMovementFactorPassive * slowDownMovementFactorActive;
        enemy.isDivisionAllowed[(int)substance] = !blockDivisionPassive && !blockDivisionActive;
        enemy.isHealingAllowed[(int)substance] = !blockHealingPassive && !blockHealingActive;
        enemy.isMovementAllowed[(int)substance] = !blockMovementPassive && !blockHealingActive;
        enemy.isDivisionSafe[(int)substance] = killAtDivisionPassive && !blockHealingActive;

        wasApplied = true;
    }

    public void merge(Attack otherAttack, float otherAttackResistanceFactor = 1f)
    {
        initialize(
         //        enemyMovement
         onEnemy
        , enemy
        , substance

        , Mathf.Max(this.remainingDurationCountdown,    getOverallEffectValue(otherAttack.attackDuration, otherAttackResistanceFactor))

        // active
        , Mathf.Max(this.damageWhenFirstHit,            getOverallEffectValue(otherAttack.damageWhenFirstHit, otherAttackResistanceFactor))
        , Mathf.Max(this.damageWhenHit,                 getOverallEffectValue(otherAttack.damageWhenHit, otherAttackResistanceFactor))
        , Mathf.Max(this.damagePerSecondActive,         getOverallEffectValue(otherAttack.damagePerSecondActive, otherAttackResistanceFactor))
        , Mathf.Max(this.slowDownDivisionFactorActive,  getOverallEffectFactor(otherAttack.slowDownDivisionFactorActive, otherAttackResistanceFactor))
        , Mathf.Max(this.slowDownHealingFactorActive,   getOverallEffectFactor(otherAttack.slowDownHealingFactorActive, otherAttackResistanceFactor))
        , Mathf.Max(this.slowDownMovementFactorActive,  getOverallEffectFactor(otherAttack.slowDownMovementFactorActive, otherAttackResistanceFactor))
        , this.blockDivisionActive || otherAttack.blockDivisionActive
        , this.blockHealingActive || otherAttack.blockHealingActive
        , this.blockMovementActive || otherAttack.blockMovementActive
        , this.killAtDivisionActive || otherAttack.killAtDivisionActive

        // passive
        , Mathf.Max(this.damagePerSecondPassive,        getOverallEffectValue(otherAttack.damagePerSecondPassive, otherAttackResistanceFactor))
        , Mathf.Max(this.slowDownDivisionFactorPassive, getOverallEffectFactor(otherAttack.slowDownDivisionFactorPassive, otherAttackResistanceFactor))
        , Mathf.Max(this.slowDownHealingFactorPassive,  getOverallEffectFactor(otherAttack.slowDownHealingFactorPassive, otherAttackResistanceFactor))
        , Mathf.Max(this.slowDownMovementFactorPassive, getOverallEffectFactor(otherAttack.slowDownMovementFactorPassive, otherAttackResistanceFactor))
        , this.blockDivisionPassive || otherAttack.blockDivisionPassive
        , this.blockHealingPassive || otherAttack.blockHealingPassive
        , this.blockMovementPassive || otherAttack.blockMovementPassive
        , this.killAtDivisionPassive || otherAttack.killAtDivisionPassive
        );
    }
}
