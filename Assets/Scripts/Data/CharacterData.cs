using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "CardRoguelike/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public int cost = 50;
    public Sprite sprite;

    [Header("기본 스탯")]
    public float baseHp = 100f;
    public float baseAtk = 20f;
    public float attackRange = 2.5f;
    public float attackSpeed = 1f;

    public enum CharacterType { Melee, Ranged, Support }
    public CharacterType characterType = CharacterType.Ranged;
}
