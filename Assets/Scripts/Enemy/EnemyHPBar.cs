using UnityEngine;

// 적 위에 따라다니는 월드스페이스 HP바 (SpriteRenderer 기반)
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer fill;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.6f, 0f);

    private Transform followTarget;
    private Enemy enemy;

    static readonly Color ColHigh   = new Color(0.20f, 0.85f, 0.20f);
    static readonly Color ColMid    = new Color(1.00f, 0.75f, 0.00f);
    static readonly Color ColLow    = new Color(0.90f, 0.15f, 0.15f);

    public void Initialize(Enemy target)
    {
        enemy = target;
        followTarget = target.transform;
    }

    void LateUpdate()
    {
        if (followTarget == null) { Destroy(gameObject); return; }

        transform.position = followTarget.position + offset;

        float ratio = enemy != null ? enemy.HpRatio : 0f;

        if (fill != null)
        {
            // fill의 로컬 스케일 X를 비율로 조절 (원점 왼쪽 정렬)
            var s = fill.transform.localScale;
            fill.transform.localScale = new Vector3(ratio, s.y, s.z);

            // 가운데 정렬 보정 (배경 너비의 절반만큼 왼쪽 이동)
            var pos = fill.transform.localPosition;
            float halfBg = background != null ? background.transform.localScale.x * 0.5f : 0.5f;
            fill.transform.localPosition = new Vector3(-halfBg + halfBg * ratio, pos.y, pos.z);

            fill.color = ratio > 0.5f ? ColHigh : ratio > 0.25f ? ColMid : ColLow;
        }
    }
}
