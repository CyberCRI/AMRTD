#define SELLTURRETS
//#define TURRETUPKEEP
//#define TURRETLIFETIME
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    [SerializeField]
    private GameObject ui = null;
    [SerializeField]
    private Button upgradeButton = null;
    [SerializeField]
    private Text upgradeCost = null;
    [SerializeField]
    private Text sellCost = null;
    [SerializeField]
    private LocalizedText sellText = null;
    [SerializeField]
    private Text upkeepCost = null;
    [SerializeField]
    private GameObject renewButton = null;

    private Node targetNode;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
#if SELLTURRETS
        sellText.setKey("GAME.NODEUI.SELL");
#else
        sellText.setKey("GAME.NODEUI.STOP");
#endif
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        setUpgradeButton();
    }

    private void setUpgradeButton()
    {
        upgradeButton.interactable = (null != targetNode) && (!targetNode.isUpgraded) && (targetNode.canUpgradeTurret());
    }

    public void setTarget(Node target)
    {
        targetNode = target;
        this.transform.position = targetNode.transform.position;

        setUpgradeButton();
        if (targetNode.isUpgraded)
        {
            upgradeCost.text = "";
        }
        else
        {
            upgradeCost.text = target.turretBlueprint.upgradeCost.ToString() + "€";
        }

#if SELLTURRETS
        sellCost.text = target.getSellCost().ToString() + "€";
#endif
#if TURRETUPKEEP
        upkeepCost.text = target.turret.upkeepCost.ToString() + "€/s";
#endif
#if TURRETLIFETIME
        renewButton.SetActive(target.turret.lifetimeStart != Mathf.Infinity);
#else
        renewButton.SetActive(false);
#endif

        ui.SetActive(true);
    }

    public void hide()
    {
        ui.SetActive(false);
    }

    public void upgrade()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKTOWERUPGRADE, 
            CustomData.getGameObjectContext(targetNode.turret).add(CustomDataTag.COST, targetNode.turretBlueprint.upgradeCost));
        targetNode.upgradeTurret();
        BuildManager.instance.deselectNode();
        BuildManager.instance.deselectTurretButton();
    }

    public void sell()
    {
        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKTOWERSELL,
            CustomData.getGameObjectContext(targetNode.turret).add(CustomDataTag.COST, targetNode.getSellCost()));
        targetNode.sellTurret();
        BuildManager.instance.deselectNode();
        BuildManager.instance.deselectTurretButton();
    }

    public void renew()
    {
#if TURRETLIFETIME
        targetNode.renewTurret();
#endif
    }
}
