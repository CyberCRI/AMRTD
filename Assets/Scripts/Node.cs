//#define DEVMODE
//#define SELLTURRETS
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    [SerializeField]
    private Color hoverColor = Color.red;
    [SerializeField]
    private Color cantBuyColor = Color.red;

    private Color startColor;
    [SerializeField]
    private Renderer renderor = null;

    [Header("Optional")]
    private GameObject turretGO = null;
    public Turret turret = null;
    public TurretBlueprint turretBlueprint = null;
    public bool isUpgraded = false;

    [SerializeField]
    private GameObject buildEffect = null;
    private ParticleSystem buildEffectPS = null;
    [SerializeField]
    private GameObject cantPayEffect = null;
    private ParticleSystem cantPayEffectPS = null;
    [SerializeField]
    private GameObject damagedEffect = null;
    private ParticleSystem damagedEffectPS = null;
    [SerializeField]
    private GameObject expiredEffect = null;
    private ParticleSystem expiredEffectPS = null;
    [SerializeField]
    private GameObject sellEffect = null;
    private ParticleSystem sellEffectPS = null;
    [SerializeField]
    private GameObject upgradeEffect = null;
    private ParticleSystem upgradeEffectPS = null;

    private BuildManager buildManager;

    public enum REMOVETOWER
    {
        CANTPAY,
        DAMAGED,
        EXPIRED,
        SOLD,
        UPGRADED,
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Awake()
    {
        startColor = renderor.material.color;

        buildEffectPS =     (ParticleSystem)buildEffect.GetComponentsInChildren<ParticleSystem>()[0];
        cantPayEffectPS =   (ParticleSystem)cantPayEffect.GetComponentsInChildren<ParticleSystem>()[0];
        damagedEffectPS =   (ParticleSystem)damagedEffect.GetComponentsInChildren<ParticleSystem>()[0];
        expiredEffectPS =   (ParticleSystem)expiredEffect.GetComponentsInChildren<ParticleSystem>()[0];
        sellEffectPS =      (ParticleSystem)sellEffect.GetComponentsInChildren<ParticleSystem>()[0];
        upgradeEffectPS =   (ParticleSystem)upgradeEffect.GetComponentsInChildren<ParticleSystem>()[0];
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        buildManager = BuildManager.instance;
    }

    /// <summary>
    /// OnMouseDown is called when the user has pressed the mouse button while
    /// over the GUIElement or Collider.
    /// </summary>
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (turretGO != null)
            {
                buildManager.selectNode(this);
                unhover();
            }
            else
            {
                if (!buildManager.canBuild)
                {
                    // no tower selected
                }
                else
                {
                    buildTurret(buildManager.getTurretToBuild());
                }
            }
        }
    }

    /// <summary>
    /// Called when the mouse enters the GUIElement or Collider.
    /// </summary>
    void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (buildManager.canBuild)
            {
                if (buildManager.canBuy)
                {
                    renderor.material.color = hoverColor;
                }
                else
                {
                    renderor.material.color = cantBuyColor;
                }
            }
        }
    }

    /// <summary>
    /// Called when the mouse enters the GUIElement or Collider.
    /// </summary>
    void OnMouseExit()
    {
        unhover();
    }

    public void removeTurret(REMOVETOWER reason, GameObject turretToDestroy)
    {
        GameObject effectPrefab;
        ParticleSystem effectPS;

        switch (reason)
        {
            case REMOVETOWER.DAMAGED:
                effectPrefab = damagedEffect;
                effectPS = damagedEffectPS;
                break;
            case REMOVETOWER.CANTPAY:
                effectPrefab = cantPayEffect;
                effectPS = cantPayEffectPS;
                break;
            case REMOVETOWER.EXPIRED:
                effectPrefab = expiredEffect;
                effectPS = expiredEffectPS;
                break;
            case REMOVETOWER.SOLD:
                effectPrefab = sellEffect;
                effectPS = sellEffectPS;
                break;
            case REMOVETOWER.UPGRADED:
                effectPrefab = upgradeEffect;
                effectPS = upgradeEffectPS;
                break;
            default:
                effectPrefab = buildEffect;
                effectPS = buildEffectPS;
                break;
        }

        GameObject effect = (GameObject)Instantiate(
            effectPrefab,
            this.transform.position,
            Quaternion.identity);
        Destroy(effect, effectPS.main.duration + effectPS.main.startLifetime.constant);

        Destroy(turretToDestroy);

        if (reason != REMOVETOWER.UPGRADED)
        {
            turretBlueprint = null;
            isUpgraded = false;
        }
    }

    void buildTurret(TurretBlueprint blueprint)
    {
        if (PlayerStatistics.money >= blueprint.cost)
        {
            PlayerStatistics.money -= blueprint.cost;

            turretGO = (GameObject)Instantiate(blueprint.prefab, this.transform.position, Quaternion.identity);
            turretGO.transform.localScale = Vector3.Scale(this.transform.parent.localScale, turretGO.transform.localScale);

            turret = turretGO.GetComponent<Turret>();
            turret.node = this;
            turret.range = turret.range * Mathf.Max(
                this.transform.parent.localScale.x,
                this.transform.parent.localScale.z
                );
            turretBlueprint = blueprint;

            GameObject effect = (GameObject)Instantiate(buildEffect, this.transform.position, Quaternion.identity);
            Destroy(effect, buildEffectPS.main.duration + buildEffectPS.main.startLifetime.constant);
        }
        else
        {
            // not enough money
        }
    }

    public void upgradeTurret()
    {
        if (PlayerStatistics.money >= turretBlueprint.upgradeCost)
        {
            PlayerStatistics.money -= turretBlueprint.upgradeCost;

            GameObject newTurretGO = (GameObject)Instantiate(turretBlueprint.upgradePrefab, this.transform.position, Quaternion.identity);
            newTurretGO.transform.localScale = Vector3.Scale(this.transform.parent.localScale, newTurretGO.transform.localScale);

            Quaternion previousRotation = turret.getPartToRotateRotation();
            GameObject oldTurret = turretGO;
            turretGO = newTurretGO;
            turret = turretGO.GetComponent<Turret>();
            turret.rotatePartToRotate(previousRotation);
            turret.node = this;
            turret.range = turret.range * Mathf.Max(
                this.transform.parent.localScale.x,
                this.transform.parent.localScale.z
                );

            removeTurret(REMOVETOWER.UPGRADED, oldTurret);

            isUpgraded = true;
        }
        else
        {
            // not enough money to upgrade"
        }
    }

    public void sellTurret()
    {
#if DEVMODE
//        Debug.Log("sellTurret");
#endif
        
#if SELLTURRETS       
//        PlayerStatistics.money += turretBlueprint.getSellCost();
#endif

        removeTurret(REMOVETOWER.SOLD, turretGO);
    }

    public void renewTurret()
    {
#if DEVMODE
        Debug.Log("renewTurret");
#endif

        turret.renew();
    }

    void unhover()
    {
        renderor.material.color = startColor;
    }
}
