using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager instance = null;

    private TurretBlueprint turretToBuild = null;

    [SerializeField]
    public GameObject standardTurretPrefab = null;
    [SerializeField]
    public GameObject missileLauncherPrefab = null;

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

    public bool canBuild { get { return null != turretToBuild; } }

    public void buildTurretOn(Node node)
    {
        if (PlayerStatistics.money >= turretToBuild.cost)
        {
            PlayerStatistics.money -= turretToBuild.cost;
            GameObject turret = (GameObject)Instantiate(turretToBuild.prefab, node.transform.position, Quaternion.identity);
            node.setTurret(turret);
            
            Debug.Log("Turret built, money left: " + PlayerStatistics.money);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    public void selectTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;
    }
}
