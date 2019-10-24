using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager instance = null;

    private TurretBlueprint turretToBuild = null;

    [SerializeField]
    private GameObject buildEffect = null;
    private ParticleSystem buildEffectPS = null;

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
            buildEffectPS = (ParticleSystem)buildEffect.GetComponentsInChildren<ParticleSystem>()[0];
        }
    }

    public bool canBuild { get { return null != turretToBuild; } }
    public bool canBuy   { get { return (null != turretToBuild) && (PlayerStatistics.money >= turretToBuild.cost); } }

    public void buildTurretOn(Node node)
    {
        if (PlayerStatistics.money >= turretToBuild.cost)
        {
            PlayerStatistics.money -= turretToBuild.cost;
            GameObject turret = (GameObject)Instantiate(turretToBuild.prefab, node.transform.position, Quaternion.identity);
            node.setTurret(turret);

            GameObject effect = (GameObject)Instantiate(buildEffect, node.transform.position, Quaternion.identity);
            Destroy(effect, buildEffectPS.main.duration + buildEffectPS.main.startLifetime.constant);
            
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
