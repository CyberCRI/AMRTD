#define VERBOSEDEBUG
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance = null;

    [SerializeField]
    private Texture2D buildCursorTexture = null;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    private TurretBlueprint turretToBuild = null;
    private Node selectedNode = null;
    private NodeUI nodeUI = null;
    private Toggle selectedTurretButton = null;

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
    public bool canBuy { get { return canBuild && (PlayerStatistics.instance.money >= turretToBuild.cost); } }

    public void linkUI(NodeUI _nodeUI)
    {
        nodeUI = _nodeUI;
    }

    public void selectNode(Node node)
    {
        #if VERBOSEDEBUG
        Debug.Log(this.GetType() + " selectNode");
        #endif

        turretToBuild = null;
        setBuildCursor(null != turretToBuild);

        if (selectedNode == node)
        {
            deselectNode();
        }
        else
        {
            selectedNode = node;
            nodeUI.setTarget(node);
            deselectTurretButton();
        }
    }

    public void deselectNode()
    {
        selectedNode = null;
        nodeUI.hide();
    }

    // assumes button != null and button.isOn
    public void selectTurretButton(Toggle button)
    {
        if (null == button || !button.isOn)
        {
            Debug.Log("selectTurretButton: incorrect parameter button=" + button);
        }

        if (null != selectedTurretButton)
        {
            if (selectedTurretButton.isOn)
            {
                selectedTurretButton.isOn = false;
            }
        }
        selectedTurretButton = button;
    }

    public void deselectTurretButton(Toggle button)
    {
        if (null != selectedTurretButton && (selectedTurretButton == button))
        {
            if (selectedTurretButton.isOn)
            {
                selectedTurretButton.isOn = false;
            }
            selectedTurretButton = null;
        }
    }
    
    public void deselectTurretButton()
    {
        if (null != selectedTurretButton)
        {
            selectedTurretButton.isOn = false;
            selectedTurretButton = null;
        }
    }

    // null == button means old turret button is used - deprecated
    // null != button means new turret toggle button is used, in which case:
    //    button.isOn: select new turret button
    //    !button.isOn: deselect current turret button
    public void selectTurretToBuild(TurretBlueprint turret, Toggle button = null)
    {
        CustomDataValue outcome;
        if (null == button || !button.isOn)
        {
            deselectTurretButton(button);
            outcome = CustomDataValue.OFF;
        }
        else
        {
            selectTurretButton(button);
            outcome = CustomDataValue.ON;
        }

        RedMetricsManager.instance.sendEvent(TrackingEvent.CLICKTOWERBUTTON,
            new CustomData(CustomDataTag.ELEMENT, turret.prefab).add(CustomDataTag.OUTCOME, outcome).add(CustomDataTag.COST, turret.cost));

        turretToBuild = (null == button) || button.isOn ? turret : null;
        setBuildCursor(null != turretToBuild);
        deselectNode();
    }

    public TurretBlueprint getTurretToBuild()
    {
        return turretToBuild;
    }

    private void setBuildCursor(bool buildOn)
    {
        if (buildOn)
        {
            Cursor.SetCursor(buildCursorTexture, hotSpot, cursorMode);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }
    }
}
