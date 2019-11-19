using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private TurretBlueprint standardTurret = null;
    [SerializeField]
    private TurretBlueprint missileLauncher = null;
    [SerializeField]
    private TurretBlueprint laserBeamer = null;


    [Header("Active")]
    [SerializeField]
    private TurretBlueprint laserBeamer_damageWhenFirstHit = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_damageWhenHit = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_damagePerSecondActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_slowDownDivisionActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_slowDownHealingActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_slowDownMovementActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_blockDivisionActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_blockHealingActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_blockMovementActive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_killAtDivisionActive = null;

    // passive
    // happens all the time while the Attack exists
    [Header("Passive")]
    [SerializeField]
    private TurretBlueprint laserBeamer_damagePerSecondPassive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_slowDownDivisionPassive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_slowDownHealingPassive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_slowDownMovementPassive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_blockDivisionPassive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_blockHealingPassive = null;
    [SerializeField]
    private TurretBlueprint laserBeamer_blockMovementPassive = null;
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

    // active
    public void selectLaserBeamer_damageWhenFirstHit()
    {
        buildManager.selectTurretToBuild(laserBeamer_damageWhenFirstHit);
    }
    public void selectLaserBeamer_damageWhenHit()
    {
        buildManager.selectTurretToBuild(laserBeamer_damageWhenHit);
    }
    public void selectLaserBeamer_damagePerSecondActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_damagePerSecondActive);
    }
    public void selectLaserBeamer_slowDownDivisionActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_slowDownDivisionActive);
    }
    public void selectLaserBeamer_slowDownHealingActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_slowDownHealingActive);
    }
    public void selectLaserBeamer_slowDownMovementActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_slowDownMovementActive);
    }
    public void selectLaserBeamer_blockDivisionActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockDivisionActive);
    }
    public void selectLaserBeamer_blockHealingActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockHealingActive);
    }
    public void selectLaserBeamer_blockMovementActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockMovementActive);
    }
    public void selectLaserBeamer_killAtDivisionActive()
    {
        buildManager.selectTurretToBuild(laserBeamer_killAtDivisionActive);
    }

    // passive
    public void selectLaserBeamer_damagePerSecondPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_damagePerSecondPassive);
    }
    public void selectLaserBeamer_slowDownDivisionPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_slowDownDivisionPassive);
    }
    public void selectLaserBeamer_slowDownHealingPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_slowDownHealingPassive);
    }
    public void selectLaserBeamer_slowDownMovementPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_slowDownMovementPassive);
    }
    public void selectLaserBeamer_blockDivisionPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockDivisionPassive);
    }
    public void selectLaserBeamer_blockHealingPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockHealingPassive);
    }
    public void selectLaserBeamer_blockMovementPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_blockMovementPassive);
    }
    public void selectLaserBeamer_killAtDivisionPassive()
    {
        buildManager.selectTurretToBuild(laserBeamer_killAtDivisionPassive);
    }
}