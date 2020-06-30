//#define VERBOSEDEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{

    // (target != null) && (enemyAttack == null) while the enemy is not striken yet
    // (bullet still flying)
    protected Transform target = null;
    protected Enemy enemy = null;

    // useful when repeatingly attacking the same enemy
    protected Attack enemyAttack = null;

    // will the next attack be the first received by the target?
    // does enemyAttack need to be set?
    protected bool firstAttack = false;

    [SerializeField]
    protected Attack modelAttack = null;

    protected void doAttack(Transform _target, Enemy _enemy = null, AudioEvent _event = 0)
    {
        _enemy = (_enemy == null) ? _target.GetComponent<Enemy>() : _enemy;

        if (null != _enemy)
        {
            if (!_enemy.isImmuneTo(modelAttack.substance))
            {
                Attack _enemyAttack = strikeAttack(_target, _enemy);
                if (null != _enemyAttack)
                {
                    _enemyAttack.apply();
                    if (0 != _event)
                    {
                        AudioManager.instance.play(_event);
                    }
                }
            }
            else
            {
                _enemy.doResistanceEffectBurst();
            }
        }
    }

    protected Attack strikeAttack(Transform _target, Enemy _enemy)
    {
        bool doesKnowTarget = false;
        Attack _enemyAttack = null;

        doesKnowTarget = (_target == target);

        if (_target == null)
        {
#if VERBOSEDEBUG
            Debug.Log("Target was apparently shot down recently");
#endif
            firstAttack = true;
        }
        else
        {
            if (firstAttack || !doesKnowTarget || (null == enemyAttack))
            {
#if VERBOSEDEBUG
//                Debug.Log("strikeAttack first attack");
#endif
                Attack[] enemyAttacks = _target.GetComponents<Attack>();
                foreach (Attack eAttack in enemyAttacks)
                {
                    if (eAttack.substance == modelAttack.substance)
                    {
                        _enemyAttack = eAttack;
#if VERBOSEDEBUG
//                        Debug.Log("Found matching attack " + eAttack.substance);
#endif
                        break;
                    }
                }

                float resistanceFactor = _enemy.resistances[(int)modelAttack.substance];

                if (null == _enemyAttack)
                {
                    // case 1: enemy has no similar undergoing attack
                    _enemyAttack = (Attack)_target.gameObject.AddComponent<Attack>();
                    _enemyAttack.initialize(true, modelAttack, _enemy, resistanceFactor);
                    if (doesKnowTarget)
                    {
                        enemyAttack = _enemyAttack;
                    }
                }
                else
                {
                    // case 2: enemy has a similar undergoing attack
                    // merge the effects
                    // if enemyAttack != null, then enemyAttack == _enemyAttack,
                    // because found through list of unique attacks affecting the enemy
                    _enemyAttack.merge(modelAttack, resistanceFactor);
                }

                if (doesKnowTarget)
                {
                    firstAttack = false;
                }
            }
            else
            {
                _enemyAttack = enemyAttack;
            }
        }

        return _enemyAttack;
    }
}
