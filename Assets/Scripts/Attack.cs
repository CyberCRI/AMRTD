using UnityEngine;

// temporary class, will be turned into a ScriptableObject
public class Attack : MonoBehaviour
{
    public enum SUBSTANCE
    {
        ANTIBIOTIC1,
        ANTIBIOTIC2,
        ANTIBIOTIC3
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
    public ABILITY[] abilities;

    private bool onEnemy = false;
    private Enemy enemy;
    public float remainingDuration;

    public float startDuration = 0f;
    private bool blockDivision = false;
    private bool killAtDivision = false;
    private float damagePerSecond = 0f;
    private float damageWhenFirstHit = 0f;
    private float damageWhenHit = 0f;
    private float slowDownFactor = 0f;

    public void initialize(
        EnemyMovement _enemy
        , bool _onEnemy

        , float _startDuration
        , bool _blockDivision
        , bool _killAtDivision
        , float _damagePerSecond
        , float _damageWhenFirstHit
        , float _damageWhenHit
        , float _slowDownFactor
         )
    {
        onEnemy = _onEnemy;

        startDuration = _startDuration;
        remainingDuration = _startDuration;
        blockDivision = _blockDivision;
        killAtDivision = _killAtDivision;
        damagePerSecond = _damagePerSecond;
        damageWhenFirstHit = _damageWhenFirstHit;
        damageWhenHit = _damageWhenHit;
        slowDownFactor = _slowDownFactor;
    }

    public void initialize(EnemyMovement _enemy, bool _onEnemy, Attack _attack)
    {
        initialize(
        _enemy
        , _onEnemy

        , _attack.startDuration
        , _attack.blockDivision
        , _attack.killAtDivision
        , _attack.damagePerSecond
        , _attack.damageWhenFirstHit
        , _attack.damageWhenHit
        , _attack.slowDownFactor);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (0f != damageWhenFirstHit)
        {
            enemy.takeDamage(damageWhenFirstHit);
            damageWhenFirstHit = 0f;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (onEnemy)
        {
            remainingDuration -= Time.deltaTime;
            if (remainingDuration <= 0)
            {
                Debug.Log("ATTACK IS OVER:" + substance);
                Destroy(this);
            }

            if (0f != damagePerSecond)
            {
                enemy.takeDamage(damagePerSecond * Time.deltaTime);
            }

            if (0f != slowDownFactor)
            {
                enemy.slow(slowDownFactor);
            }
        }
    }

    public void hit()
    {
        // deal damage
        enemy.takeDamage(damageWhenHit);
    }
}
