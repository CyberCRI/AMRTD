//#define VERBOSEDEBUG
#define DEVMODE
//#define USESPRITE
#define USECOLOR
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pneumocyte : MonoBehaviour
{
    public const string pcTag = "PCTag";

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
    [SerializeField]
    private Image healthBar = null;
    [SerializeField]
    private Transform healthBarCanvas = null;

    [Header("Parameters")]
    [SerializeField]
    private Pneumocyte[] neighbours = null;
    [SerializeField]
    private float neighboursRange = 0f;
    [SerializeField]
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
                this.gameObject.name = "Pneumocyte" + m_Index + "_" + _status.ToString();
            }
        }
    }
    private bool isAlive = true;
    private float maxHealth = 100f;
    private float _currentHealth = 0f;
    private float currentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
            updateHealthBar();
        }
    }
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
    private string m_Index = null;
    private static int pneumocyteCount = 0;

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
        m_Index = (pneumocyteCount++).ToString("00");
        currentHealth = maxHealth;
        healingRate = healingRatioRate * maxHealth; // regains maxHealth * x per second
#if DEVMODE
        healthBarCanvas.rotation = Camera.main.transform.rotation;
#else
        healthBarCanvas.gameObject.SetActive(false);
#endif

        // compute neighbours
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, neighboursRange);
        neighbours = new Pneumocyte[colliders.Length];
        int j = 0;
        Debug.Log(this.gameObject.name + " Start neighbours");
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            if (collider.tag == pcTag)
            {
                Debug.Log("PC found!");
                neighbours[j++] = (Pneumocyte)collider.GetComponent<Pneumocyte>();
            }
        }
    }

    void Update()
    {
#if DEVMODE

        // status
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (Random.value > 0.7f)
            {
                status = (STATUS)(((int)status + 1) % 4);
                Debug.Log("status="+status.ToString());   
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            status = (STATUS)(((int)status + 1) % 4);
            Debug.Log("status="+status.ToString());
        }

        // hurt
        if (Input.GetKeyDown(KeyCode.X))
        {
            
            if (Random.value > 0.7f)
            {
                addLifePoints(-50f);
                Debug.Log("addLifePoints(-50f)");
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            addLifePoints(-50f);
            Debug.Log("addLifePoints(-50f)");
        }

        // heal
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Random.value > 0.7f)
            {
                addLifePoints(50f);
                Debug.Log("addLifePoints(50f)");
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            addLifePoints(50f);
            Debug.Log("addLifePoints(50f)");
        }
#endif
        if (STATUS.HEALTHY == status)
        {
            // is neighbour dead? is division countdown over? then divide to replace
        }
        else if (STATUS.RECOVERING == status)
        {
            addLifePoints(healingRate * Time.deltaTime);
            _renderer.color = Color.Lerp(_recoveringColor, _healthyColor, healthBar.fillAmount);
        }

    }

    private void updateHealthBar()
    {
        healthBar.fillAmount = currentHealth/maxHealth;
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
                _renderer.color = Color.Lerp(_recoveringColor, _healthyColor, healthBar.fillAmount);
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
                Debug.LogError(this.gameObject.name + " unknown status " + status);
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
        else if (status != STATUS.INFECTED_SPAWNING_VIRUSES)
        {
            status = STATUS.RECOVERING;
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
            Debug.Log(this.gameObject.name + " spawns a virion!");
            spawnVirion();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        isDoneSpawning = true;

        // cell survives
        if (Random.value <= lungCellRecoveryProbability)
        {
            Debug.Log(this.gameObject.name + " recovers!");
            status = STATUS.RECOVERING;
        }
        //cell dies
        else
        {
            Debug.Log(this.gameObject.name + " doesn't make it :'(");
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
        isAlive = false;
        currentHealth = 0f;
        status = STATUS.DEAD;
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


    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, neighboursRange);
    }
}