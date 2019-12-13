﻿using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance = null;

    private TurretBlueprint turretToBuild = null;
    private Node selectedNode = null;
    private NodeUI nodeUI = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (null != instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public bool canBuild { get { return (null != turretToBuild); } }
    public bool canBuy { get { return canBuild && (PlayerStatistics.money >= turretToBuild.cost); } }

    public void linkUI(NodeUI _nodeUI)
    {
        nodeUI = _nodeUI;
    }

    public void selectNode(Node node)
    {
        turretToBuild = null;

        if (selectedNode == node)
        {
            deselectNode();
        }
        else
        {
            selectedNode = node;
            nodeUI.setTarget(node);
        }
    }

    public void deselectNode()
    {
        selectedNode = null;
        nodeUI.hide();
    }

    public void selectTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;
        deselectNode();
    }

    public TurretBlueprint getTurretToBuild()
    {
        return turretToBuild;
    }
}
