using UnityEngine;

public class PlacedCharacter : MonoBehaviour
{
    public float Atk { get; private set; }
    public float MaxHp { get; private set; }
    public float CurrentHp { get; private set; }
    public float AttackRange { get; private set; }
    public float AttackSpeed { get; private set; }
    public CharacterData Data { get; private set; }
    public bool HasDoubleDamage { get; private set; }

    private float attackTimer;
    private GameObject projPrefab;

    private static readonly Collider2D[] _overlapBuffer = new Collider2D[32];

    public void Initialize(CharacterData data, GameObject projectilePrefab = null)
    {
        Data = data;
        MaxHp = data.baseHp;
        CurrentHp = data.baseHp;
        Atk = data.baseAtk;
        AttackRange = data.attackRange;
        AttackSpeed = data.attackSpeed;
        projPrefab = projectilePrefab;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && data.sprite != null)
            sr.sprite = data.sprite;

        CardManager.Instance?.RegisterCharacter(this);
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;

        var target = GetNearestEnemy();
        if (target == null) return;

        DoAttack(target);
        attackTimer = 1f / AttackSpeed;
    }

    Enemy GetNearestEnemy()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, AttackRange, _overlapBuffer);
        Enemy best = null;
        float bestDist = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            var e = _overlapBuffer[i].GetComponent<Enemy>();
            if (e == null) continue;
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < bestDist) { bestDist = d; best = e; }
        }
        return best;
    }

    void DoAttack(Enemy target)
    {
        if (projPrefab != null)
        {
            var go = Instantiate(projPrefab, transform.position, Quaternion.identity);
            go.GetComponent<Projectile>()?.Initialize(target, Atk);
        }
        else
        {
            target.TakeDamage(Atk);
        }
    }

    public void AddAtk(float v) { Atk += v; }
    public void AddMaxHp(float v) { MaxHp += v; CurrentHp += v; }
    public void AddAttackSpeed(float v) { AttackSpeed += v; }
    public void AddRange(float v) { AttackRange += v; }

    public void ApplyDoubleDamage()
    {
        if (HasDoubleDamage) return; // 중복 적용 방지
        HasDoubleDamage = true;
        Atk *= 2f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
