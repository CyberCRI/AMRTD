//#define VERBOSEDEBUG
//#define USESPRITE
#define USECOLOR
using UnityEngine;
using System.Collections;

public class Pneumocyte : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer = null;
    
    [SerializeField]
    private Sprite _healthySprite = null;
    [SerializeField]
    private Sprite _infectedSprite = null;
    [SerializeField]
    private Sprite _recoveringSprite = null;
    [SerializeField]
    private Sprite _deadSprite = null;
    
    [SerializeField]
    private Color _healthyColor = Color.white;
    [SerializeField]
    private Color _infectedColor = Color.white;
    [SerializeField]
    private Color _recoveringColor = Color.white;
    [SerializeField]
    private Color _deadColor = Color.white;

    [Header("Parameters")]
    private Pneumocyte[] neighbours = null;
    private STATUS _status = STATUS.HEALTHY;
    private STATUS status
    {
        get { return _status;}
        set
        {
            if (value != _status)
            {
                _status = value;
                updateAppearance();
            }
        }
    }
    private bool isAlive = true;
    private float maxHealth = 100f;
    private float currentHealth = 0f;
    private float divisionPeriod = 10f;
    private float healingRatioRate = .05f; // regains maxHealth * x per second
    private float healingRate = 0f; // regains maxHealth * x per second

    private int virionsSpawnCountPerLungCell = 0;
    private float lungCellRecoveryProbability = 0f;
    private float damageRatioPerSpawn = 0f;
    private float timeBeforeSpawnStarts = 0f;
    private float timeBeforeRecoveryStarts = 0f;
    private float timeBetweenSpawns = 0f;
    private GameObject virusPrefab = null;

    private bool isDoneSpawning = false;

    // update appearance accordingly
    private enum STATUS
    {
        HEALTHY,
        INFECTED_SPAWNING_VIRUSES,
        RECOVERING,
        DEAD,
    }

    void Start()
    {
        currentHealth = maxHealth;
        healingRate = healingRatioRate * maxHealth; // regains maxHealth * x per second
        // compute neighbours
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            status = (STATUS)(((int)status + 1) % 4);
            Debug.Log("status="+status.ToString());
        }
        /*
        if (STATUS.HEALTHY == status)
        {
            // is neighbour dead? is division countdown over? then divide to replace
        }
        else if (STATUS.RECOVERING == status)
        {
            addLifePoints(healingRate * Time.deltaTime);
        }
        else if (STATUS.RECOVERING == status)
        {
            Debug.LogError("NOT IMPLEMENTED");
        }
        */

    }

    private void resetVariablesAtRecovery()
    {
        virionsSpawnCountPerLungCell = 0;
        lungCellRecoveryProbability = 0f;
        damageRatioPerSpawn = 0f;
        timeBeforeSpawnStarts = 0f;
        timeBeforeRecoveryStarts = 0f;
        timeBetweenSpawns = 0f;
        virusPrefab = null;
        isDoneSpawning = true;
    }

    private void updateAppearance()
    {
        Debug.LogError("NOT IMPLEMENTED");
        switch(status)
        {
            case STATUS.HEALTHY:
#if USESPRITE            
                _renderer.sprite = _healthySprite;
#elif USECOLOR
                _renderer.color = _healthyColor;
#endif
                break;
            case STATUS.INFECTED_SPAWNING_VIRUSES:
#if USESPRITE            
                _renderer.sprite = _infectedSprite;
#elif USECOLOR
                _renderer.color = _infectedColor;
#endif
                break;
            case STATUS.RECOVERING:
#if USESPRITE            
                _renderer.sprite = _recoveringSprite;
#elif USECOLOR
                _renderer.color = _recoveringColor;
#endif
                break;
            case STATUS.DEAD:
#if USESPRITE            
                _renderer.sprite = _deadSprite;
#elif USECOLOR
                _renderer.color = _deadColor;
#endif
                break;
            default:
                Debug.LogError("unknown status " + status);
                break;
        }
    }

    // points can be negative for damage or positive for healing
    private void addLifePoints(float points)
    {
        currentHealth += points;
        if (currentHealth <= 0f)
        {
            die();
        }
        else if (currentHealth >= maxHealth)
        {
            setHealthy();
        }
    }

    private void setHealthy()
    {
        currentHealth = maxHealth;
        status = STATUS.HEALTHY;
    }

    public bool infect(Virus infecter)
    {
        bool result = false;
        if (STATUS.HEALTHY == status)
        {
            status = STATUS.INFECTED_SPAWNING_VIRUSES;

            addLifePoints(-infecter.damageRatioPerInfection * maxHealth);
            virionsSpawnCountPerLungCell = infecter.virionsSpawnCountPerLungCell;
            lungCellRecoveryProbability = infecter.lungCellRecoveryProbability;
            damageRatioPerSpawn = infecter.damageRatioPerSpawn;
            timeBeforeSpawnStarts = infecter.timeBeforeSpawnStarts;
            timeBeforeRecoveryStarts = infecter.timeBeforeRecoveryStarts;
            timeBetweenSpawns = infecter.timeBetweenSpawns;
            virusPrefab = infecter.virusPrefab;

            // launch periodic spawn
            StartCoroutine(spawnVirions());

            result =  true;
        }
        return result;
    }

    private IEnumerator spawnVirions()
    {
        isDoneSpawning = false;
        yield return new WaitForSeconds(timeBeforeSpawnStarts);
        
        for (int i = 0; i < virionsSpawnCountPerLungCell; i++)
        {
            spawnVirion();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        isDoneSpawning = true;

        // cell survives
        if (Random.value <= lungCellRecoveryProbability)
        {
            status = STATUS.RECOVERING;
        }
        //cell dies
        else
        {
            die();
        }
    }

    private IEnumerator recover()
    {
        yield return new WaitForSeconds(timeBeforeRecoveryStarts);
        
        while (STATUS.HEALTHY != status)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void die()
    {
        status = STATUS.DEAD;
        isAlive = false;
        currentHealth = 0f;
    }

    private void spawnVirion()
    {
        Instantiate(virusPrefab, this.transform.position, this.transform.rotation);
        addLifePoints(-damageRatioPerSpawn * maxHealth);
    }

    private void divide()
    {

    }

    // when get infected:
}