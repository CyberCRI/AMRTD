//#define VERBOSEDEBUG
#define SELLTURRETS
//#define TURRETLIFETIME
#define STATICTURRETRESISTANCEPOINTSMODE
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
    private GameObject cantPayBuildEffect = null;
    private ParticleSystem cantPayBuildEffectPS = null;
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

        buildEffectPS = (ParticleSystem)buildEffect.GetComponentsInChildren<ParticleSystem>()[0];
        cantPayEffectPS = (ParticleSystem)cantPayEffect.GetComponentsInChildren<ParticleSystem>()[0];
        cantPayBuildEffectPS = (ParticleSystem)cantPayBuildEffect.GetComponentsInChildren<ParticleSystem>()[0];
        damagedEffectPS = (ParticleSystem)damagedEffect.GetComponentsInChildren<ParticleSystem>()[0];
        expiredEffectPS = (ParticleSystem)expiredEffect.GetComponentsInChildren<ParticleSystem>()[0];
        sellEffectPS = (ParticleSystem)sellEffect.GetComponentsInChildren<ParticleSystem>()[0];
        upgradeEffectPS = (ParticleSystem)upgradeEffect.GetComponentsInChildren<ParticleSystem>()[0];
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
#if VERBOSEDEBUG
        Debug.Log("Node OnMouseDown");
#endif
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKTILE, new CustomData(CustomDataTag.ELEMENT, this.gameObject.name));
        if (!HelpButtonUI.instance.isHelpModeOn())
        {
            manageClick();
        }
    }

    public void manageClick()
    {
#if VERBOSEDEBUG
        Debug.Log("Node manageClick");
#endif
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (turretGO != null)
            {
                RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKTOWER, new CustomData(CustomDataTag.ELEMENT, turretGO.name));
                buildManager.selectNode(this);
                unhover();
            }
            else
            {
                if (buildManager.canBuild)
                {
                    TurretBlueprint blueprint = buildManager.getTurretToBuild();
                    if (buildManager.canBuy)
                    {
                        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKTOWERBUILD, 
                            new CustomData(CustomDataTag.ELEMENT, blueprint.prefab.name).add(CustomDataTag.OUTCOME, CustomDataValue.SUCCESS));
                        buildTurret(blueprint);
                    }
                    else
                    {
                        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKTOWERBUILD, 
                            new CustomData(CustomDataTag.ELEMENT, blueprint.prefab.name).add(CustomDataTag.OUTCOME, CustomDataValue.FAILURE));
                        GameObject effect = (GameObject)Instantiate(
                            cantPayBuildEffect,
                            this.transform.position,
                            Quaternion.identity);
                        Destroy(effect, CommonUtilities.getEffectMaxDuration(cantPayBuildEffectPS));
                    }
                }
                else
                {
                    // no tower selected
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
        Destroy(effect, CommonUtilities.getEffectMaxDuration(effectPS));

        Destroy(turretToDestroy);

        if (reason != REMOVETOWER.UPGRADED)
        {
            turretBlueprint = null;
            isUpgraded = false;
        }
    }

    void buildTurret(TurretBlueprint blueprint)
    {
        if (PlayerStatistics.instance.money >= blueprint.cost)
        {
            PlayerStatistics.instance.money -= blueprint.cost;

            turretGO = (GameObject)Instantiate(blueprint.prefab, this.transform.position, Quaternion.identity);
            turretGO.transform.localScale = Vector3.Scale(this.transform.parent.localScale, turretGO.transform.localScale);

            turret = turretGO.GetComponent<Turret>();
            turret.node = this;
            //            turret.range = turret.range * Mathf.Max(
            //                this.transform.parent.localScale.x,
            //                this.transform.parent.localScale.z
            //                );
            turretBlueprint = blueprint;

            GameObject effect = (GameObject)Instantiate(buildEffect, this.transform.position, Quaternion.identity);
            Destroy(effect, CommonUtilities.getEffectMaxDuration(buildEffectPS));
        }
        else
        {
            // not enough money
        }
    }

    public bool canUpgradeTurret()
    {
        return (null != turretBlueprint) && (PlayerStatistics.instance.money >= turretBlueprint.upgradeCost);
    }

    public void upgradeTurret()
    {
        if (canUpgradeTurret())
        {
            PlayerStatistics.instance.money -= turretBlueprint.upgradeCost;

            GameObject newTurretGO = (GameObject)Instantiate(turretBlueprint.upgradePrefab, this.transform.position, Quaternion.identity);
            newTurretGO.transform.localScale = Vector3.Scale(this.transform.parent.localScale, newTurretGO.transform.localScale);

            Quaternion previousRotation = turret.getPartToRotateRotation();
            GameObject oldTurret = turretGO;
            turretGO = newTurretGO;
#if STATICTURRETRESISTANCEPOINTSMODE
            float oldResistanceCost = turret.getResistancePoints();
            turret.setBeingUpgraded(true);
#endif
            turret = turretGO.GetComponent<Turret>();
#if STATICTURRETRESISTANCEPOINTSMODE
            float newResistanceCost = turret.getResistancePoints();
            turret.setUpgraded(true);
            PlayerStatistics.instance.turretResistancePoints += (newResistanceCost - oldResistanceCost);
#endif
            turret.rotatePartToRotate(previousRotation);
            turret.node = this;
            //            turret.range = turret.range * Mathf.Max(
            //                this.transform.parent.localScale.x,
            //                this.transform.parent.localScale.z
            //                );

            removeTurret(REMOVETOWER.UPGRADED, oldTurret);

            isUpgraded = true;
        }
        else
        {
            // not enough money to upgrade
        }
    }

    public void sellTurret()
    {
#if SELLTURRETS
        PlayerStatistics.instance.money += getSellCost();
#endif

        removeTurret(REMOVETOWER.SOLD, turretGO);
    }

    public int getSellCost()
    {
        int result = turretBlueprint.cost;
        if (isUpgraded)
        {
            result += turretBlueprint.upgradeCost;
        }
        return result / 2;
    }

    public void renewTurret()
    {
#if TURRETLIFETIME
#if VERBOSEDEBUG
        Debug.Log("renewTurret");
#endif
        turret.renew();
#endif
    }

    void unhover()
    {
        renderor.material.color = startColor;
    }
}
