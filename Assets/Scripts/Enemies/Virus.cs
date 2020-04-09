//#define VERBOSEDEBUG
using UnityEngine;

public class Virus : MonoBehaviour
{
    [Header("Parameters")]
    public GameObject virusPrefab = null;
    private STATUS status = STATUS.SEARCHING_CELL;
    public int virionsSpawnCountPerLungCell = 2;

    // lungCellRecoveryProbability * 100 % chances of not dying when spawn ends
    public float lungCellRecoveryProbability = .6f;

    // damage ratios inflected; ratio of max health
    public float damageRatioPerInfection = .4f;
    public float damageRatioPerSpawn = .1f;
    public float timeBeforeSpawnStarts = 1f;
    public float timeBeforeRecoveryStarts = .5f;
    public float timeBetweenSpawns = 1f;

    private enum STATUS
    {
        SEARCHING_CELL,
        GOING_TO_CELL,
    }

    void Update()
    {

    }
}