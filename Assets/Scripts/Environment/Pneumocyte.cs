//#define VERBOSEDEBUG
//#define DEVMODE
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
    private Sprite[] _sprites = new Sprite[(int)STATUS.COUNT];
    [SerializeField]
    private Color[] _colors = new Color[(int)STATUS.COUNT];
    [SerializeField]
    private Image healthBar = null;
    [SerializeField]
    private Transform healthBarCanvas = null;

    [Header("Parameters")]
    [SerializeField]
    private Transform spawnPosition = null;
    [SerializeField]
    private Pneumocyte[] neighbours = null;
    [SerializeField]
    private BoxCollider m_boxCollider = null;
    System.Collections.Generic.List<Pneumocyte> tempNeighbours = new System.Collections.Generic.List<Pneumocyte>();
    [SerializeField]
    private float neighboursRange = 0f;
    [SerializeField]
    private STATUS _status = STATUS.HEALTHY;
    public STATUS status
    {
        get { return _status;}
        private set
        {
            if (value != _status)
            {
                _status = value;
                updateAppearance();
                this.gameObject.name = "Pneumocyte" + m_Index + "_" + _status.ToString();
            }
        }
    }
    private bool _isAlive = true;
    public bool isAlive
    {
        get
        {
            return _isAlive;
        }
    }
    public const float maxHealth = 100f;
    private float _currentHealth = 0f;
    public float currentHealth
    {
        get
        {
            return _currentHealth;
        }
        private set
        {
            _currentHealth = value;
            updateHealthBar();
        }
    }
    [SerializeField]
    private float divisionPeriod = 2f;
    private float divisionCountdown = 0f;
    [SerializeField]
    private float healingRatioRate = .05f; // regains maxHealth * x per second
    private float healingRate = 0f; // regains maxHealth * x per second

    // variables set during infection
    private int virionsSpawnCountPerLungCell = 0;
    private float lungCellRecoveryProbability = 0f;
    private float damageRatioPerSpawn = 0f;
    private float timeBeforeSpawnStarts = 0f;
    private float timeBeforeRecoveryStarts = 0f;
    private float timeBetweenSpawns = 0f;
    private GameObject virusPrefab = null;
    private string m_Index = null;
    int neighboursIndex = 0;

    private bool isDoneSpawning = false;

    // update appearance accordingly
    public enum STATUS
    {
        HEALTHY,
        INFECTED_SPAWNING_VIRUSES,
        RECOVERING,
        DEAD,

        COUNT,
    }

    // pneumocyte animation parameters
    [Header("Wobble scale")]
    [Tooltip("Scale over time is Si * (1 + Rf * cos(ωt)")]
    [SerializeField]
    private Transform wobbledTransform;
    [SerializeField]
    private float[] ratioFactors = new float[(int)STATUS.COUNT];
    private float ratioFactor = 0f;
    [SerializeField]
    private float[] wobbleScaleSpeeds = new float[(int)STATUS.COUNT];
    private float wobbleScaleSpeed = 0f;
    private Vector3 initialScale = Vector3.zero;
    private Vector3 temporaryLocalScaleVector3 = Vector3.zero;
    [SerializeField]
    private float piDivisor = 8f;
    public Transform infectionTransform = null;
    
    void Awake()
    {
        #if VERBOSEDEBUG
        Debug.Log(string.Format("{0}: {1}: Awake neighbours ", this.GetType(), this.gameObject.name));
        #endif

        // compute neighbours
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, neighboursRange);
        tempNeighbours.Clear();
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            if ((collider.tag == pcTag) && (this.gameObject != collider.gameObject))
            {
                tempNeighbours.Add(collider.GetComponent<Pneumocyte>());
            }
        }
        neighbours = tempNeighbours.ToArray();
        tempNeighbours.Clear();

        if (Vector3.zero == initialScale)
        {
            initialScale = wobbledTransform.localScale;
        }
        updateAppearance();
    }

    void Start()
    {
        PneumocyteManager.instance.register(this);

        Destroy(m_boxCollider);
        m_Index = (PneumocyteManager.instance.entityIndex++).ToString("00");
        currentHealth = maxHealth;
        healingRate = healingRatioRate * maxHealth; // regains maxHealth * x per second
#if DEVMODE
        healthBarCanvas.rotation = Camera.main.transform.rotation;
#else
        healthBarCanvas.gameObject.SetActive(false);
#endif

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
                #if VERBOSEDEBUG
                Debug.Log("status="+status.ToString());   
                #endif
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            status = (STATUS)(((int)status + 1) % 4);
            #if VERBOSEDEBUG
            Debug.Log("status="+status.ToString());
            #endif
        }

        // hurt
        if (Input.GetKeyDown(KeyCode.X))
        {
            
            if (Random.value > 0.7f)
            {
                addLifePoints(-50f);
                #if VERBOSEDEBUG
                Debug.Log("addLifePoints(-50f)");
                #endif
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            addLifePoints(-50f);
            #if VERBOSEDEBUG
            Debug.Log("addLifePoints(-50f)");
            #endif
        }

        // heal
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Random.value > 0.7f)
            {
                addLifePoints(50f);
                #if VERBOSEDEBUG
                Debug.Log("addLifePoints(50f)");
                #endif
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            addLifePoints(50f);
            #if VERBOSEDEBUG
            Debug.Log("addLifePoints(50f)");
            #endif
        }
#endif
        if (STATUS.HEALTHY == status)
        {
            // is neighbour dead? is division countdown over? then divide to replace
            divisionCountdown += Time.deltaTime;
            if (divisionCountdown >= divisionPeriod)
            {
                for (int i = neighboursIndex; i < neighboursIndex + neighbours.Length; i++)
                {
                    if (neighbours[neighboursIndex % neighbours.Length].status == STATUS.DEAD)
                    {
                        #if VERBOSEDEBUG
                        Debug.Log("FOUND DEAD PC!");
                        #endif
                        divisionCountdown = 0;
                        neighbours[neighboursIndex % neighbours.Length].setHealthy();
                        break;
                    }
                }
                neighboursIndex = (neighboursIndex + 1) % neighbours.Length;
            }
        }
        else if (STATUS.RECOVERING == status)
        {
            addLifePoints(healingRate * Time.deltaTime);
            _renderer.color = Color.Lerp(_colors[(int)STATUS.DEAD], _colors[(int)STATUS.RECOVERING], healthBar.fillAmount);
        }

        wobbledTransform.localScale = new Vector3(
                initialScale.x * (1 + ratioFactor * Mathf.Cos(wobbleScaleSpeed * Time.timeSinceLevelLoad)),
                initialScale.y,
                initialScale.z * (1 + ratioFactor * Mathf.Cos(Mathf.PI/piDivisor + wobbleScaleSpeed * Time.timeSinceLevelLoad))
            );
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
        if (status != STATUS.COUNT)
        {
            int index = (int)status;
            ratioFactor = ratioFactors[index];
            wobbleScaleSpeed = wobbleScaleSpeeds[index];
#if USESPRITE            
            _renderer.sprite = _sprites[index];
#elif USECOLOR
            _renderer.color = _colors[index];
#endif
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

    public void setHealthy()
    {
        _isAlive = true;
        currentHealth = maxHealth;
        status = STATUS.HEALTHY;
    }

    public bool getInfected(Virus infecter)
    {
        bool result = false;
        if (STATUS.HEALTHY == status)
        {
            status = STATUS.INFECTED_SPAWNING_VIRUSES;

            virionsSpawnCountPerLungCell = infecter.virionsSpawnCountPerLungCell;
            lungCellRecoveryProbability = infecter.lungCellRecoveryProbability;
            addLifePoints(-infecter.damageRatioPerInfection * maxHealth);
            damageRatioPerSpawn = infecter.damageRatioPerSpawn;
            timeBeforeSpawnStarts = infecter.timeBeforeSpawnStarts;
            timeBeforeRecoveryStarts = infecter.timeBeforeRecoveryStarts;
            timeBetweenSpawns = infecter.timeBetweenSpawns;
            virusPrefab = infecter.getPrefab();

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
            #if VERBOSEDEBUG
            Debug.Log(this.gameObject.name + " spawns a virion!");
            #endif
            spawnVirion();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isDoneSpawning = true;

        // cell survives
        if (Random.value <= lungCellRecoveryProbability)
        {
            #if VERBOSEDEBUG
            Debug.Log(this.gameObject.name + " recovers!");
            #endif
            status = STATUS.RECOVERING;
        }
        //cell dies
        else
        {
            #if VERBOSEDEBUG
            Debug.Log(this.gameObject.name + " doesn't make it :'(");
            #endif
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
        _isAlive = false;
        currentHealth = 0f;
        status = STATUS.DEAD;
    }

    private void spawnVirion()
    {
        GameObject virion = Instantiate(virusPrefab, spawnPosition.position, virusPrefab.transform.rotation);
        Virus virus = virion.GetComponent<Virus>();
        //virus.setTarget(this);
        
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