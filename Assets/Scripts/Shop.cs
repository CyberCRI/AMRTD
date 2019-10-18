using UnityEngine;

public class Shop : MonoBehaviour
{
    private BuildManager buildManager;
    
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        buildManager = BuildManager.instance;
    }

    public void purchaseStandardTurret()
    {
        Debug.Log("Standard Turret selected.");
        buildManager.setTurretToBuild(buildManager.standardTurretPrefab);
    }
    public void purchaseAnotherTurret()
    {
        Debug.Log("Another Turret selected.");
        buildManager.setTurretToBuild(buildManager.anotherTurretPrefab);
    }
}
