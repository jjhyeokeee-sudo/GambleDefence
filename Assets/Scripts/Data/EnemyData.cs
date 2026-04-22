using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "CardRoguelike/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHp = 100f;
    public float moveSpeed = 2f;
    public int goldReward = 10;
    public int damage = 1; // 기지에 도달했을 때 입히는 데미지
    public Sprite sprite;
}
