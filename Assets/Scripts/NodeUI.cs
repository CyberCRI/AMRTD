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
        
        sellCost.text = target.turretBlueprint.getSellCost().ToString() + "€";

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
}
