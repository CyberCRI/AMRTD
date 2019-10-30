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
    private GameObject turret = null;
    public TurretBlueprint turretBlueprint = null;
    public bool isUpgraded = false;

    [SerializeField]
    private GameObject buildEffect = null;
    private ParticleSystem buildEffectPS = null;
    [SerializeField]
    private GameObject sellEffect = null;
    private ParticleSystem sellEffectPS = null;

    private BuildManager buildManager;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Awake()
    {
        startColor = renderor.material.color;
        buildEffectPS = (ParticleSystem)buildEffect.GetComponentsInChildren<ParticleSystem>()[0];
        sellEffectPS = (ParticleSystem)sellEffect.GetComponentsInChildren<ParticleSystem>()[0];
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
            if (turret != null)
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

    void buildTurret(TurretBlueprint blueprint)
    {
        if (PlayerStatistics.money >= blueprint.cost)
        {
            PlayerStatistics.money -= blueprint.cost;
            turret = (GameObject)Instantiate(blueprint.prefab, this.transform.position, Quaternion.identity);
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
            
            GameObject newTurret = (GameObject)Instantiate(turretBlueprint.upgradePrefab, this.transform.position, Quaternion.identity);
            Quaternion previousRotation = turret.GetComponent<Turret>().getPartToRotateRotation();
            Destroy(turret);
            turret = newTurret;
            turret.GetComponent<Turret>().rotatePartToRotate(previousRotation);

            GameObject effect = (GameObject)Instantiate(buildEffect, this.transform.position, Quaternion.identity);
            Destroy(effect, buildEffectPS.main.duration + buildEffectPS.main.startLifetime.constant);

            isUpgraded = true;
        }
        else
        {
            // not enough money to upgrade"
        }
    }

    public void sellTurret()
    {
        PlayerStatistics.money += turretBlueprint.getSellCost();

        Destroy(turret);
        turretBlueprint = null;
        isUpgraded = false;

        GameObject effect = (GameObject)Instantiate(sellEffect, this.transform.position, Quaternion.identity);
        Destroy(effect, sellEffectPS.main.duration + sellEffectPS.main.startLifetime.constant);
    }

    void unhover()
    {
        renderor.material.color = startColor;
    }
}
