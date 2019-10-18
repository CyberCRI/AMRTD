using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager instance = null;

    private GameObject turretToBuild = null;

    [SerializeField]
    public GameObject standardTurretPrefab = null;
    [SerializeField]
    public GameObject anotherTurretPrefab = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one BuildManager");
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public GameObject getTurretToBuild()
    {
        return turretToBuild;
    }

    public void setTurretToBuild(GameObject turret)
    {
        turretToBuild = turret;
    }
}
