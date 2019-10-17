using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager instance;

    private GameObject turretToBuild;

    [SerializeField]
    private GameObject standardTurretPrefab;

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
