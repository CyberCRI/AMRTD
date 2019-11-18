using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private TurretBlueprint standardTurret = null;
    [SerializeField]
    private TurretBlueprint missileLauncher = null;
    [SerializeField]
    private TurretBlueprint laserBeamer = null;
    
    [SerializeField]
    private TurretBlueprint laserBeamer_blockDivisionActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_blockDivisionPassive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_blockHealingActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_blockHealingPassive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_killAtDivisionActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_killAtDivisionPassive = null;

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

    
    public void selectLaserBeamer_blockDivisionActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockDivisionActive);
    }
    public void selectLaserBeamer_blockDivisionPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockDivisionPassive);
    }
    public void selectLaserBeamer_blockHealingActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockHealingActive);
    }
    public void selectLaserBeamer_blockHealingPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockHealingPassive);
    }
    public void selectLaserBeamer_killAtDivisionActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_killAtDivisionActive);
    }
    public void selectLaserBeamer_killAtDivisionPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_killAtDivisionPassive);
    }
}
