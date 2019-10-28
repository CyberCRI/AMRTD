using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeUI : MonoBehaviour
{
    [SerializeField]
    private GameObject ui;

    private Node targetNode;

    public void setTarget(Node target)
    {
        targetNode = target;
        this.transform.position = targetNode.transform.position;
        ui.SetActive(true);
    }

    public void hide()
    {
        ui.SetActive(false);
    }
}
