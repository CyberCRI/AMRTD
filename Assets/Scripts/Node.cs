using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    [SerializeField]
    private Color hoverColor = Color.red;

    private Color startColor;
    [SerializeField]
    private Renderer renderor = null;

    private GameObject turret = null;

    BuildManager buildManager;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Awake()
    {
        startColor = renderor.material.color;
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
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            if (turret != null)
            {
                Debug.Log("Can't build there.");
            }
            else
            {
                GameObject turretToBuild = buildManager.getTurretToBuild();
                
                if (null == turretToBuild)
                {
                    Debug.Log("No tower selected.");
                }
                else
                {
                    // Build a turret
                    turret = (GameObject)Instantiate(turretToBuild, this.transform.position, this.transform.rotation);
                }
            }
        }
    }

    /// <summary>
    /// Called when the mouse enters the GUIElement or Collider.
    /// </summary>
    void OnMouseEnter()
    {
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            if (null != buildManager.getTurretToBuild())
            {
                renderor.material.color = hoverColor;
            }
        }
    }

    /// <summary>
    /// Called when the mouse enters the GUIElement or Collider.
    /// </summary>
    void OnMouseExit()
    {
        renderor.material.color = startColor;
    }
}
