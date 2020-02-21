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


    public void setTarget(Node target)
    {
        targetNode = target;
        this.transform.position = targetNode.transform.position;

        if (target.isUpgraded)
        {
            upgradeButton.interactable = false;
            upgradeCost.text = "";
        }
        else
        {
            upgradeButton.interactable = true;
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
        targetNode.upgradeTurret();
        BuildManager.instance.deselectNode();
    }

    public void sell()
    {
        targetNode.sellTurret();
        BuildManager.instance.deselectNode();
    }

    public void renew()
    {
#if TURRETLIFETIME
        targetNode.renewTurret();
#endif
    }
}
