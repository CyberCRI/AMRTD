using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private TurretBlueprint standardTurret = null;
    [SerializeField]
    private TurretBlueprint missileLauncher = null;
    [SerializeField]
    private TurretBlueprint laserBeamer = null;

    private BuildManager buildManager;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        buildManager = BuildManager.instance;
    }

    public void selectStandardTurret()
    {
        buildManager.selectTurretToBuild(standardTurret);
    }
    public void selectMissileLauncher()
    {
        buildManager.selectTurretToBuild(missileLauncher);
    }
    public void selectLaserBeamer()
    {
        buildManager.selectTurretToBuild(laserBeamer);
    }
}
