using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private Button upgradeButton;
    [SerializeField]
    private Text upgradeCost;
    [SerializeField]
    private Button sellButton;
    [SerializeField]
    private Text sellCost;

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
}
