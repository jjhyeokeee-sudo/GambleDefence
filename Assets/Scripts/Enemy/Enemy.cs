using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyData data;
    private float currentHp;
    private List<Transform> waypoints;
    private int waypointIndex = 0;
    private bool isDead = false;

    // HP바용 (나중에 UI 연결)
    public float HpRatio => currentHp / data.maxHp;

    [SerializeField] private GameObject hpBarPrefab;

    public void Initialize(EnemyData enemyData, List<Transform> path)
    {
        data = enemyData;
        currentHp = data.maxHp;
        waypoints = path;
        waypointIndex = 0;

        if (data.sprite != null)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = data.sprite;
        }

        if (hpBarPrefab != null)
        {
            var bar = Instantiate(hpBarPrefab);
            bar.GetComponent<EnemyHPBar>()?.Initialize(this);
        }
    }

    void Update()
    {
        if (isDead || waypoints == null || waypointIndex >= waypoints.Count) return;
        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        Transform target = waypoints[waypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * data.moveSpeed * Time.deltaTime;

        // 방향에 따라 스프라이트 뒤집기
        if (dir.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (dir.x > 0) transform.localScale = new Vector3(1, 1, 1);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Count)
                ReachBase();
        }
    }

    void ReachBase()
    {
        // 기지 도달 시 데미지 처리
        GameManager.Instance.TakeDamage(data.damage);
        Die(rewardGold: false);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHp -= amount;
        if (currentHp <= 0) Die(rewardGold: true);
    }

    void Die(bool rewardGold)
    {
        if (isDead) return;
        isDead = true;

        if (rewardGold)
            GameManager.Instance.AddGold(data.goldReward);

        WaveManager.Instance.OnEnemyDead();
        Destroy(gameObject);
    }
}
