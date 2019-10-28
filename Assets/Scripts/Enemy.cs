using UnityEngine;

public class Enemy : MonoBehaviour
{
    public const string enemyTag = "EnemyTag";
    public float startSpeed = 0f;
    [HideInInspector]
    public float speed = 0f;
    public float minimumDistance = 0f;

    [SerializeField]
    private ParticleSystem deathEffect = null;
    [SerializeField]
    private float health = 100f;
    [SerializeField]
    private int reward = 50;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        speed = startSpeed;
    }

    public void takeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            die();
        }
    }

    private void die()
    {
        GameObject effect = (GameObject)Instantiate(deathEffect.gameObject, this.transform.position, Quaternion.identity);
        Destroy(effect.gameObject, deathEffect.main.duration + deathEffect.main.startLifetime.constant);
        PlayerStatistics.money += reward;
        Destroy(this.gameObject);
    }

    public void slow(float slowRatioFactor)
    {
        speed = startSpeed * (1f - slowRatioFactor);
    }
}
