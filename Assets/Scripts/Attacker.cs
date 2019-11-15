//#define DEVMODE
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

    protected void doAttack(Transform _target, Enemy _enemy = null)
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
                }
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
#if DEVMODE            
            DEVMODE.Log("Target was apparently shot down recently");
#endif
            firstAttack = true;
        }
        else
        {
            if (firstAttack || !doesKnowTarget || (null == enemyAttack))
            {
#if DEVMODE                
                DEVMODE.Log("strikeAttack first attack");
#endif
                Attack[] enemyAttacks = _target.GetComponents<Attack>();
                foreach (Attack eAttack in enemyAttacks)
                {
                    if (eAttack.substance == modelAttack.substance)
                    {
                        _enemyAttack = eAttack;
#if DEVMODE                        
                        DEVMODE.Log("Found matching attack " + eAttack.substance);
#endif
                        break;
                    }
                }

                if (null == _enemyAttack)
                {
                    // case 1: enemy has no similar undergoing attack
                    _enemyAttack = (Attack)_target.gameObject.AddComponent<Attack>();
                    _enemyAttack.initialize(true, modelAttack, _enemy);
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
                    _enemyAttack.merge(modelAttack);
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
