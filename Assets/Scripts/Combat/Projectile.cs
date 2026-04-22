using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private float damage;
    private float moveSpeed = 8f;
    private bool hasHit;

    public void Initialize(Enemy targetEnemy, float dmg, float spd = 8f)
    {
        target = targetEnemy;
        damage = dmg;
        moveSpeed = spd;
    }

    void Update()
    {
        if (hasHit) return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.25f)
        {
            hasHit = true;
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
