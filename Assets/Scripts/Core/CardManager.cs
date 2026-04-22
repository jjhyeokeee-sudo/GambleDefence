using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [SerializeField] private List<CardData> cardPool = new();

    public UnityEvent<List<CardData>> onCardsDraw;  // UI에 3장 전달

    private readonly List<PlacedCharacter> characters = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RegisterCharacter(PlacedCharacter c) { characters.Add(c); }
    public void UnregisterCharacter(PlacedCharacter c) { characters.Remove(c); }
    public IReadOnlyList<PlacedCharacter> AllCharacters => characters;

    // WaveManager.onWaveCleared 에 연결
    public void OnWaveCleared()
    {
        if (cardPool.Count == 0) { Debug.LogWarning("[CardManager] 카드 풀 비어있음"); return; }

        var pool = new List<CardData>(cardPool);
        var drawn = new List<CardData>();
        int count = Mathf.Min(3, pool.Count);
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, pool.Count);
            drawn.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        Time.timeScale = 0f;
        onCardsDraw?.Invoke(drawn);
    }

    public void ApplyCard(CardData card, PlacedCharacter target)
    {
        if (card == null || target == null) return;
        switch (card.effectType)
        {
            case CardEffectType.AtkUp:          target.AddAtk(card.value);          break;
            case CardEffectType.HpUp:           target.AddMaxHp(card.value);        break;
            case CardEffectType.AttackSpeedUp:  target.AddAttackSpeed(card.value);  break;
            case CardEffectType.RangeUp:        target.AddRange(card.value);        break;
            case CardEffectType.DoubleDamage:   target.ApplyDoubleDamage();         break;
        }
        Debug.Log($"[CardManager] {card.cardName} → {target.Data?.characterName}");
        Time.timeScale = 1f;
        WaveManager.Instance.StartNextWaveManually();
    }
}
