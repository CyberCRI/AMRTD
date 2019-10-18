using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager instance = null;

    private GameObject turretToBuild = null;

    [SerializeField]
    private GameObject standardTurretPrefab = null;

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
            turretToBuild = standardTurretPrefab;
        }
    }

    public GameObject getTurretToBuild()
    {
        return turretToBuild;
    }

}
