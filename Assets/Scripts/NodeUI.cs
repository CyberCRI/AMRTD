//#define SELLTURRETS
using System.Collections;
using System.Collections.Generic;
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
    private Text upkeepCost = null;

    private Node targetNode;

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
        //sellCost.text = target.turretBlueprint.getSellCost().ToString() + "€";
#endif
        upkeepCost.text = target.turret.upkeepCost.ToString() + "€/s";

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
        targetNode.renewTurret();
    }
}
