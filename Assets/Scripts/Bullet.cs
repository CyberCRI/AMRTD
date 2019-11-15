using UnityEngine;

public class Bullet : Attacker
{
    [SerializeField]
    private float speed = 0f;
    [SerializeField]
    private float explosionRadius = 0f;
    [SerializeField]
    private ParticleSystem bulletImpactEffect = null;

    public void initialize(Attack _attack)
    {
        modelAttack = _attack;
    }

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
            blowUp();
        }
        else
        {
            Vector3 dirToTarget = target.position - this.transform.position;
            float distanceThisFrame = speed * Time.deltaTime;

            if (dirToTarget.magnitude <= distanceThisFrame)
            {
                blowUp();
            }
            else
            {
                this.transform.Translate(dirToTarget.normalized * distanceThisFrame, Space.World);

                this.transform.LookAt(target);
            }
        }
    }

    void blowUp()
    {
        GameObject effect = Instantiate(bulletImpactEffect.gameObject, this.transform.position, this.transform.rotation);
        Destroy(effect, bulletImpactEffect.main.duration + bulletImpactEffect.main.startLifetime.constant);

        if (explosionRadius > 0f)
        {
            shrapnel();
        }
        else if (null != target)
        {
            doAttack(target, enemy);
        }

        Destroy(this.gameObject);
    }

    void shrapnel()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == Enemy.enemyTag)
            {
                Enemy _enemy = collider.transform.GetComponent<Enemy>();
                if (null != _enemy)
                {
                    doAttack(collider.transform, _enemy);
                }

            }
        }
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
