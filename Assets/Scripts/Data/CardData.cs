using UnityEngine;

public enum CardEffectType { AtkUp, HpUp, AttackSpeedUp, RangeUp, DoubleDamage }
public enum CardRarity { Common, Rare, Legendary }

[CreateAssetMenu(fileName = "CardData", menuName = "CardRoguelike/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;
    public Sprite cardSprite;
    public CardRarity rarity = CardRarity.Common;
    public CardEffectType effectType;
    public float value;
}
