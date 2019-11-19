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

        ANTIBIOTICS_COUNT
    }

    public enum ABILITY
    {
        NONE,
        OVER_TIME,
        BLOCK_DIVISION,
        KILL_DIVISION,
        DIRECT_DAMAGE,
        SLOW_DOWN
    }

    ///TODO link SUBSTANCEs to ABILITYs: table, dict, ...

    public SUBSTANCE substance;
    //    public ABILITY[] abilities;

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
    [SerializeField]
    private float damageWhenFirstHit = 0f;
    [SerializeField]
    private float damageWhenHit = 0f;
    [SerializeField]
    private float damagePerSecondActive = 0f;
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
        damageWhenFirstHit = _damageWhenFirstHit * resistanceFactor;
        damageWhenHit = _damageWhenHit * resistanceFactor;
        damagePerSecondActive = _damagePerSecondActive * resistanceFactor;
        slowDownDivisionFactorActive = _slowDownDivisionFactorActive * resistanceFactor;
        slowDownHealingFactorActive = _slowDownHealingFactorActive * resistanceFactor;
        slowDownMovementFactorActive = _slowDownMovementFactorActive * resistanceFactor;
        blockDivisionActive = _blockDivisionActive;
        blockHealingActive = _blockHealingActive;
        blockMovementActive = _blockMovementActive;
        killAtDivisionActive = _killAtDivisionActive;

        // passive
        damagePerSecondPassive = _damagePerSecondPassive * resistanceFactor;
        slowDownDivisionFactorPassive = _slowDownDivisionFactorPassive * resistanceFactor;
        slowDownHealingFactorPassive = _slowDownHealingFactorPassive * resistanceFactor;
        slowDownMovementFactorPassive = _slowDownMovementFactorPassive * resistanceFactor;
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
            enemy.showAntibioticAttackIndicator(substance, true);

            if (0f != damageWhenFirstHit)
            {
                enemy.takeDamage(damageWhenFirstHit);
                damageWhenFirstHit = 0f;
            }

            // set all passive abilities
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
                Debug.Log("Attack is over:" + substance);
#endif
                enemy.showAntibioticAttackIndicator(substance, false);

                // set off active and passive abilities
                enemy.isDivisionAllowed[(int)substance] = true;
                enemy.isHealingAllowed[(int)substance] = true;
                enemy.isMovementAllowed[(int)substance] = true;
                enemy.isDivisionSafe[(int)substance] = true;
                
                Destroy(this);
            }

            if (0f != damagePerSecondPassive)
            {
                enemy.takeDamage(damagePerSecondPassive * Time.deltaTime);
            }

            if (0f != slowDownMovementFactorPassive)
            {
                enemy.slow(slowDownMovementFactorPassive);
            }

            // TODO make sure this does not happen too soon for Enemy to take it into account
            // Change script execution order?
            if (wasApplied)
            {
                // revert active abilities
                enemy.isDivisionAllowed[(int)substance] = !blockDivisionPassive;
                enemy.isHealingAllowed[(int)substance] = !blockHealingPassive;
                enemy.isMovementAllowed[(int)substance] = !blockMovementPassive;
                enemy.isDivisionSafe[(int)substance] = !killAtDivisionPassive;
                wasApplied = false;
            }
        }
    }

    public void apply()
    {
        if (0f != damagePerSecondActive)
        {
            enemy.takeDamage(damagePerSecondActive * Time.deltaTime);
        }

        if (0f != slowDownMovementFactorActive)
        {
            enemy.slow(slowDownMovementFactorActive);
        }

        if (0f != damageWhenHit)
        {
            enemy.takeDamage(damageWhenHit);
        }

        // apply active abilities
        enemy.isDivisionAllowed[(int)substance] = blockDivisionPassive || !blockDivisionActive;
        enemy.isHealingAllowed[(int)substance] = blockHealingPassive || !blockHealingActive;

        wasApplied = true;
    }

    public void merge(Attack otherAttack, float otherAttackResistanceFactor = 1f)
    {
        initialize(
         //        enemyMovement
         onEnemy
        , enemy
        , substance

        , Mathf.Max(this.remainingDurationCountdown, otherAttack.attackDuration * otherAttackResistanceFactor)

        // active
        , Mathf.Max(this.damageWhenFirstHit, otherAttack.damageWhenFirstHit * otherAttackResistanceFactor)
        , Mathf.Max(this.damageWhenHit, otherAttack.damageWhenHit * otherAttackResistanceFactor)
        , Mathf.Max(this.damagePerSecondActive, otherAttack.damagePerSecondActive * otherAttackResistanceFactor)
        , Mathf.Max(this.slowDownDivisionFactorActive, otherAttack.slowDownDivisionFactorActive * otherAttackResistanceFactor)
        , Mathf.Max(this.slowDownHealingFactorActive, otherAttack.slowDownHealingFactorActive * otherAttackResistanceFactor)
        , Mathf.Max(this.slowDownMovementFactorActive, otherAttack.slowDownMovementFactorActive * otherAttackResistanceFactor)
        , this.blockDivisionActive || otherAttack.blockDivisionActive
        , this.blockHealingActive || otherAttack.blockHealingActive
        , this.blockMovementActive || otherAttack.blockMovementActive
        , this.killAtDivisionActive || otherAttack.killAtDivisionActive

        // passive
        , Mathf.Max(this.damagePerSecondPassive, otherAttack.damagePerSecondPassive * otherAttackResistanceFactor)
        , Mathf.Max(this.slowDownDivisionFactorPassive, otherAttack.slowDownDivisionFactorPassive * otherAttackResistanceFactor)
        , Mathf.Max(this.slowDownHealingFactorPassive, otherAttack.slowDownHealingFactorPassive * otherAttackResistanceFactor)
        , Mathf.Max(this.slowDownMovementFactorPassive, otherAttack.slowDownMovementFactorPassive * otherAttackResistanceFactor)
        , this.blockDivisionPassive || otherAttack.blockDivisionPassive
        , this.blockHealingPassive || otherAttack.blockHealingPassive
        , this.blockMovementPassive || otherAttack.blockMovementPassive
        , this.killAtDivisionPassive || otherAttack.killAtDivisionPassive
        );
    }
}
