using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    [SerializeField]
    private float speed;
    [SerializeField]
    private ParticleSystem bulletImpactEffect;

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
                transform.Translate(dirToTarget.normalized * distanceThisFrame, Space.World);
            }
        }
    }

    void hitTarget()
    {
        GameObject effect = Instantiate(bulletImpactEffect.gameObject, this.transform.position, this.transform.rotation);
        Destroy(effect, bulletImpactEffect.main.startLifetime.constant);
        Destroy(this.gameObject);
        Destroy(target.gameObject);
    }
}
