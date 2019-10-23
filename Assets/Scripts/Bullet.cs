using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target = null;
    [SerializeField]
    private float speed = 0f;
    [SerializeField]
    private float explosionRadius = 0f;
    [SerializeField]
    private ParticleSystem bulletImpactEffect = null;

    public void seek(Transform _target)
    {
        target = _target;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Vector3 dirToTarget = target.position - this.transform.position;
            float distanceThisFrame = speed * Time.deltaTime;

            if (dirToTarget.magnitude <= distanceThisFrame)
            {
                hitTarget();
            }
            else
            {
                this.transform.Translate(dirToTarget.normalized * distanceThisFrame, Space.World);

                this.transform.LookAt(target);
            }
        }
    }

    void hitTarget()
    {
        GameObject effect = Instantiate(bulletImpactEffect.gameObject, this.transform.position, this.transform.rotation);
        Destroy(effect, bulletImpactEffect.main.startLifetime.constant);

        if (explosionRadius > 0f)
        {
            explode();
        }
        else
        {
            damage(target);
        }

        Destroy(this.gameObject);
    }

    void explode()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, explosionRadius);
        foreach(Collider collider in colliders)
        {
            if (collider.tag == Enemy.enemyTag)
            {
                damage(collider.transform);
            }
        }
    }

    void damage (Transform enemy)
    {
        Destroy(enemy.gameObject);
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, explosionRadius);
    }
}
