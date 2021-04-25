//#define VERBOSEDEBUG
//#define DEVMODE
//#define VERBOSEMETRICSLVL2
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhiteBloodCellMovement : WobblyMovement
{
    public const string wbcTag = "WBCTag";
    private int wbcIndex = 0;

    [Header("Movement")]
    private Transform targetTransform = null;
    public float baseSpeed = 0f;
    [SerializeField]
    private float speedVariation = 0f;
    [SerializeField]
    private float gainAltitudeImpulse = 10f;
    public Vector3 idlePosition = Vector3.zero;

    private bool disappearing = false;

    [Header("Health")]
    [SerializeField]
    private float hitsMaxCount = 10f;
    public float _hitsLeft = 0f;
    private float hitsLeft
    {
        get
        {
            return _hitsLeft;
        }
        set
        {
            _hitsLeft = value;
            if (WhiteBloodCellManager.instance.chaseViruses)
            {
                updateHealthIndicators();
            }
        }
    }
    [SerializeField]
    private Renderer _sphericalRenderer = null;
    [SerializeField]
    private Renderer _silhouetteRenderer = null;
    private MaterialPropertyBlock _propBlock = null;
    [SerializeField]
    private Color colorHealthy = Color.white;
    [SerializeField]
    private Color colorWounded = Color.grey;
    [SerializeField]
    private Image healthBar = null;
    private Color _color;
    [SerializeField]
    private AudioEmitter audioEmitter = null;

#if VERBOSEDEBUG
    public WBCACTION action = WBCACTION.NONE;

    public enum WBCACTION
    {
        NONE,
        IDLE,
        CHASING,
        DISAPPEARING,
    }
    
    private GameObject _targetOriginal = null;
    private GameObject _targetComputed = null;
#endif

    // Start is called before the first frame update
    void Start()
    {
#if VERBOSEDEBUG
        Debug.Log(string.Format("{0}: {1}: Start ", this.GetType(), this.gameObject.name));
#endif

#if VERBOSEMETRICSLVL2
        RedMetricsManager.instance.sendEvent(TrackingEvent.WBCSPAWNS, CustomData.getGameObjectContext(this.gameObject));
#endif

        //setTarget();
        setSpeed();

        repulsers = new string[4] { Enemy.enemyTag, RedBloodCellMovement.rbcTag, Virus.virusTag, WhiteBloodCellMovement.wbcTag };
    }

    protected override void OnTriggerEnter(Collider collider)
    {
        if (hitsLeft > 0)
        {
            bool collides = false;

            if (collider.tag == Enemy.enemyTag)
            {
                absorb(collider.gameObject.GetComponent<EnemyMovement>());
            }
            else if (collider.tag == Virus.virusTag)
            {
                absorb(collider.gameObject.GetComponent<Virus>());
            }
        }
        base.OnTriggerEnter(collider);
    }

    protected override void onAwakeDone()
    {
        _propBlock = new MaterialPropertyBlock();
        hitsLeft = hitsMaxCount;
    }

    private void updateHealthIndicators()
    {
        healthBar.fillAmount = hitsLeft / hitsMaxCount;
        _color = Color.Lerp(colorWounded, colorHealthy, healthBar.fillAmount);

        _sphericalRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", _color);
        _sphericalRenderer.SetPropertyBlock(_propBlock);

        _silhouetteRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", _color);
        _silhouetteRenderer.SetPropertyBlock(_propBlock);
    }

    protected override void setDisplacement()
    {
        // set target if needed
        if (null != targetTransform) // chasing
        {
#if VERBOSEDEBUG
            action = WBCACTION.CHASING;
#endif
            hasReachedTarget = false;
            target = targetTransform.position;
            float x, z;
            if (WhiteBloodCellManager.instance.isAreaConstrained)
            {
                x = Mathf.Clamp(target.x, WhiteBloodCellManager.instance.limitLeft, WhiteBloodCellManager.instance.limitRight);
                z = Mathf.Clamp(target.z, WhiteBloodCellManager.instance.limitBottom, WhiteBloodCellManager.instance.limitTop);
            }
            else
            {
                x = target.x;
                z = target.z;
            }
            target = new Vector3(x, this.transform.position.y, z);

#if DEVMODE && VERBOSEDEBUG
            if (null != _targetOriginal)
            {
                Destroy(_targetOriginal);
                Destroy(_targetComputed);
            }
            _targetOriginal = CommonUtilities.createDebugObject(targetTransform.position, this.name + "-target-original", 3f);
            _targetComputed = CommonUtilities.createDebugObject(target, this.name + "-target-computed", 3f);
#endif
        }
        else if ((!disappearing) && (Vector3.zero == target)) // idle
        {
#if VERBOSEDEBUG
            action = WBCACTION.IDLE;
#endif
#if DEVMODE && VERBOSEDEBUG
            if (null != _targetOriginal)
            {
                Destroy(_targetOriginal);
                Destroy(_targetComputed);
            }
#endif
            hasReachedTarget = false;
            target = new Vector3(idlePosition.x, this.transform.position.y, idlePosition.z);
        }

        base.setDisplacement();
    }

    private bool isTargetInConstrainedArea()
    {
        return ((!WhiteBloodCellManager.instance.isAreaConstrained)
                    || ((targetTransform.position.x <= WhiteBloodCellManager.instance.limitRight)
                        && (targetTransform.position.x >= WhiteBloodCellManager.instance.limitLeft)
                        && (targetTransform.position.z <= WhiteBloodCellManager.instance.limitTop)
                        && (targetTransform.position.z >= WhiteBloodCellManager.instance.limitBottom)
                        )
                );
    }

    protected override void onWobbleDone()
    {
        if (hasReachedTarget)
        {
            if ((null != targetTransform) && isTargetInConstrainedArea())
            {
                // chasing
                absorb(targetTransform);
            }
            else if (disappearing)
            {
                // disappearing
#if VERBOSEMETRICSLVL2
                RedMetricsManager.instance.sendEvent(TrackingEvent.WBCLEAVES, CustomData.getGameObjectContext(this.gameObject));
#endif
                Destroy(this.gameObject);
            }
        }
    }

    public void setTarget(Transform _target)
    {
        targetTransform = _target;
        if (this.transform.position.y < _target.position.y)
        {
            _rigidbody.AddForce(Vector3.up * gainAltitudeImpulse, ForceMode.Impulse);
        }
    }

    private void setSpeed()
    {
        startSpeed = baseSpeed + Random.Range(-speedVariation, speedVariation);
    }

    public void initialize(int _index, Vector3 _idlePosition)
    {
        wbcIndex = _index;
        idlePosition = _idlePosition;
    }

    private void prepareAbsorb(bool isBacterium)
    {
        WhiteBloodCellManager.instance.reportDeath(wbcIndex, isBacterium);

#if VERBOSEDEBUG
        action = WBCACTION.DISAPPEARING;
#endif
#if DEVMODE && VERBOSEDEBUG
        if (null != _targetOriginal)
        {
            Destroy(_targetOriginal);
            Destroy(_targetComputed);
        }
#endif

        disappearing = true;
        target = BloodUtilities.instance.bloodEnd2.position;
        GetComponentInChildren<Animator>().SetTrigger("dead");
        targetTransform = null;
        hasReachedTarget = false;

#if !DEVMODE
        if (!GameManager.instance.isObjectiveDefenseMode())
        {
            PlayerStatistics.instance.lives--;
        }
#endif
    }

    System.Collections.Generic.List<GameObject> nearbyGOs = new System.Collections.Generic.List<GameObject>();
    public System.Collections.Generic.List<GameObject> getNearbyGameObjects(string goTag, float detectionRadius)
    {
        // compute neighbours
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, detectionRadius);
        nearbyGOs.Clear();
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            if ((collider.tag == goTag) && (this.gameObject != collider.gameObject))
            {
                nearbyGOs.Add(collider.gameObject);
            }
        }
        return nearbyGOs;
    }

    public void absorb(Virus virus)
    {
        //RedMetricsManager.instance.sendEvent(TrackingEvent.PATHOGENKILLEDBYWBC, CustomData.getVirusContext(virus.gameObject).add(CustomDataTag.WBCHEALTH, (int)hitsLeft));
        audioEmitter.play(AudioEvent.PATHOGENKILLEDBYWBC);
        hitsLeft--;
        if (0 == hitsLeft)
        {
            prepareAbsorb(false);
        }
        virus.getAbsorbed(this.transform);
    }

    public void absorb(EnemyMovement enemy)
    {
        //RedMetricsManager.instance.sendEvent(TrackingEvent.PATHOGENKILLEDBYWBC, CustomData.getBacteriumContext(enemy.gameObject).add(CustomDataTag.WBCHEALTH, (int)hitsLeft));
        audioEmitter.play(AudioEvent.PATHOGENKILLEDBYWBC);
        hitsLeft = 0;
        prepareAbsorb(true);
        enemy.getAbsorbed(this.transform);
    }

    private void absorb(Transform ttransform)
    {
        if (ttransform.tag == Enemy.enemyTag)
        {
            absorb(ttransform.GetComponent<EnemyMovement>());
        }
        else if (ttransform.tag == Virus.virusTag)
        {
            absorb(ttransform.GetComponent<Virus>());
        }
        else
        {
            Debug.LogError("incorrect tag " + ttransform.tag);
        }
    }
}
