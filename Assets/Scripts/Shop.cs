using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private TurretBlueprint standardTurret = null;
    [SerializeField]
    private TurretBlueprint missileLauncher = null;

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
        Debug.Log("Standard Turret selected.");
        buildManager.selectTurretToBuild(standardTurret);
    }
    public void selectMissileLauncher()
    {
        Debug.Log("Missile Launcher selected.");
        buildManager.selectTurretToBuild(missileLauncher);
    }
}
