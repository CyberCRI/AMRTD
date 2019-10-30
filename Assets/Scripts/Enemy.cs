using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public const string enemyTag = "EnemyTag";

    [Header("Movement")]
    public float startSpeed = 0f;
    [HideInInspector]
    public float speed = 0f;
    public float minimumDistance = 0f;

    [Header("Worth")]
    [SerializeField]
    private int reward = 0;
    
    [Header("Health")]
    [SerializeField]
    private ParticleSystem deathEffect = null;
    private float health = 0f;
    [SerializeField]
    private float startHealth = 0f;
    [SerializeField]
    private Image healthBar = null;
    private bool isAlive = false;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        speed = startSpeed;
        health = startHealth;
        isAlive = true;
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health/startHealth;

        if (isAlive && health <= 0f)
        {
            die();
        }
    }

    private void die()
    {
        isAlive = false;
        GameObject effect = (GameObject)Instantiate(deathEffect.gameObject, this.transform.position, Quaternion.identity);
        Destroy(effect.gameObject, deathEffect.main.duration + deathEffect.main.startLifetime.constant);
        PlayerStatistics.money += reward;
        Destroy(this.gameObject);
    }

    public void slow(float slowRatioFactor)
    {
        speed = startSpeed * (1f - slowRatioFactor);
    }

    void OnDestroy()
    {
        WaveSpawner.enemiesAlive--;
    }
}
