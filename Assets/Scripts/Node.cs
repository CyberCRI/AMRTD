using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField]
    private Color hoverColor;
    
    private Color startColor;
    [SerializeField]
    private Renderer renderor;

    private GameObject turret;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Awake()
    {
        startColor = renderor.material.color;
    }

    /// <summary>
    /// OnMouseDown is called when the user has pressed the mouse button while
    /// over the GUIElement or Collider.
    /// </summary>
    void OnMouseDown()
    {
        if(turret != null)
        {
            Debug.Log("Can't build there.");
        }
        else
        {
            // Build a turret
            GameObject turretToBuild = BuildManager.instance.getTurretToBuild();
            turret = (GameObject)Instantiate(turretToBuild, this.transform.position, this.transform.rotation);
        }
    }

    /// <summary>
    /// Called when the mouse enters the GUIElement or Collider.
    /// </summary>
    void OnMouseEnter()
    {
        renderor.material.color = hoverColor;
    }

    /// <summary>
    /// Called when the mouse enters the GUIElement or Collider.
    /// </summary>
    void OnMouseExit()
    {
        renderor.material.color = startColor;
    }
}
