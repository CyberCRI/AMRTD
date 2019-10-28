﻿using UnityEngine;
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
    [SerializeField]
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

    public void setTurret(GameObject _turret)
    {
        turret = _turret;
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
                Debug.Log("Selecting node.");
                buildManager.selectNode(this);
                unhover();
            }
            else
            {
                if (!buildManager.canBuild)
                {
                    Debug.Log("No tower selected.");
                }
                else
                {
                    buildManager.buildTurretOn(this);
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

    void unhover()
    {
        renderor.material.color = startColor;
    }
}
