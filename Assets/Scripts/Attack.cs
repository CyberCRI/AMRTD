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
    // needs a constant channelling or a punctual attack
    [Header("Active")]
    [SerializeField]
    private float damageWhenFirstHit = 0f;
    [SerializeField]
    private float damageWhenHit = 0f;
    [SerializeField]
    private bool blockDivisionActive = false;
    [SerializeField]
    private float damagePerSecondActive = 0f;
    [SerializeField]
    private bool killAtDivisionActive = false;
    [SerializeField]
    private float slowDownFactorActive = 0f;

    // passive
    // happens all the time while the Attack exists
    [Header("Passive")]
    [SerializeField]
    private bool blockDivisionPassive = false;
    [SerializeField]
    private float damagePerSecondPassive = 0f;
    [SerializeField]
    private bool killAtDivisionPassive = false;
    [SerializeField]
    private float slowDownFactorPassive = 0f;

    public void initialize(
         //        EnemyMovement _enemyMovement
         bool _onEnemy
        , Enemy _enemy
        , SUBSTANCE _substance

        , float _attackDuration = 0f
        , float _damageWhenFirstHit = 0f
        , float _damageWhenHit = 0f

        , bool _blockDivisionActive = false
        , float _damagePerSecondActive = 0f
        , bool _killAtDivisionActive = false
        , float _slowDownFactorActive = 0f

        , bool _blockDivisionPassive = false
        , float _damagePerSecondPassive = 0f
        , bool _killAtDivisionPassive = false
        , float _slowDownFactorPassive = 0f
         )
    {
        //        enemyMovement = _enemyMovement;
        onEnemy = _onEnemy;
        enemy = _enemy;
        substance = _substance;

        attackDuration = _attackDuration;
        remainingDurationCountdown = _attackDuration;
        damageWhenFirstHit = _damageWhenFirstHit;
        damageWhenHit = _damageWhenHit;

        blockDivisionActive = _blockDivisionActive;
        damagePerSecondActive = _damagePerSecondActive;
        killAtDivisionActive = _killAtDivisionActive;
        slowDownFactorActive = _slowDownFactorActive;

        blockDivisionPassive = _blockDivisionPassive;
        damagePerSecondPassive = _damagePerSecondPassive;
        killAtDivisionPassive = _killAtDivisionPassive;
        slowDownFactorPassive = _slowDownFactorPassive;
    }

    public void initialize(
         //        EnemyMovement _enemyMovement
         bool _onEnemy
        , Attack _attack
        , Enemy _enemy
        )
    {
        initialize(
         //        _enemyMovement
         _onEnemy
        , _enemy
        , _attack.substance

        , _attack.attackDuration
        , _attack.damageWhenFirstHit
        , _attack.damageWhenHit

        , _attack.blockDivisionActive
        , _attack.damagePerSecondActive
        , _attack.killAtDivisionActive
        , _attack.slowDownFactorActive

        , _attack.blockDivisionPassive
        , _attack.damagePerSecondPassive
        , _attack.killAtDivisionPassive
        , _attack.slowDownFactorPassive
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
            enemy.showIndicator(substance, true);

            if (0f != damageWhenFirstHit)
            {
                enemy.takeDamage(damageWhenFirstHit);
                damageWhenFirstHit = 0f;
            }
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
                Debug.Log("Attack is over:" + substance);
                enemy.showIndicator(substance, false);
                Destroy(this);
            }

            if (0f != damagePerSecondPassive)
            {
                enemy.takeDamage(damagePerSecondPassive * Time.deltaTime);
            }

            if (0f != slowDownFactorPassive)
            {
                enemy.slow(slowDownFactorPassive);
            }
        }
    }

    public void apply()
    {
        if (0f != damagePerSecondActive)
        {
            enemy.takeDamage(damagePerSecondActive * Time.deltaTime);
        }

        if (0f != slowDownFactorActive)
        {
            enemy.slow(slowDownFactorActive);
        }

        if (0f != damageWhenHit)
        {
            enemy.takeDamage(damageWhenHit);
        }
    }

    public void merge(Attack otherAttack)
    {
        initialize(
         //        enemyMovement
         onEnemy
        , enemy
        , substance

        , Mathf.Max(this.remainingDurationCountdown, otherAttack.attackDuration)
        , Mathf.Max(this.damageWhenFirstHit, otherAttack.damageWhenFirstHit)
        , Mathf.Max(this.damageWhenHit, otherAttack.damageWhenHit)

        , this.blockDivisionActive || otherAttack.blockDivisionActive
        , Mathf.Max(this.damagePerSecondActive, otherAttack.damagePerSecondActive)
        , this.killAtDivisionActive || otherAttack.killAtDivisionActive
        , Mathf.Max(this.slowDownFactorActive, otherAttack.slowDownFactorActive)

        , this.blockDivisionPassive || otherAttack.blockDivisionPassive
        , Mathf.Max(this.damagePerSecondPassive, otherAttack.damagePerSecondPassive)
        , this.killAtDivisionPassive || otherAttack.killAtDivisionPassive
        , Mathf.Max(this.slowDownFactorPassive, otherAttack.slowDownFactorPassive)
        );
    }
}
