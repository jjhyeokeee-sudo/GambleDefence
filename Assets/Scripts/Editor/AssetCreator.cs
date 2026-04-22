#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

// Tools > CardRoguelike > Create Sample Assets 으로 샘플 에셋 자동 생성
public class AssetCreator
{
    [MenuItem("Tools/CardRoguelike/Create Sample Assets")]
    public static void CreateAll()
    {
        EnsureDir("Assets/ScriptableObjects/Characters");
        EnsureDir("Assets/ScriptableObjects/Enemies");
        EnsureDir("Assets/ScriptableObjects/Cards");
        EnsureDir("Assets/ScriptableObjects/Waves");

        // ── 캐릭터 ─────────────────────────────────────────────
        CreateCharacter("Archer",  cost: 60,  hp: 80f,  atk: 25f, range: 3.5f, speed: 1.2f);
        CreateCharacter("Warrior", cost: 80,  hp: 150f, atk: 20f, range: 1.5f, speed: 0.8f);
        CreateCharacter("Mage",    cost: 100, hp: 60f,  atk: 45f, range: 4.0f, speed: 0.7f);

        // ── 적 ─────────────────────────────────────────────────
        CreateEnemy("Goblin",    hp: 60f,  speed: 2.5f, gold: 8,  dmg: 1);
        CreateEnemy("Orc",       hp: 150f, speed: 1.5f, gold: 15, dmg: 2);
        CreateEnemy("Troll",     hp: 300f, speed: 0.8f, gold: 25, dmg: 3);
        CreateEnemy("BossGoblin",hp: 500f, speed: 1.0f, gold: 50, dmg: 5);

        // ── 카드 ─────────────────────────────────────────────────
        CreateCard("공격 강화",    "공격력 +15",          CardEffectType.AtkUp,          CardRarity.Common,    15f);
        CreateCard("체력 강화",    "최대 체력 +50",       CardEffectType.HpUp,           CardRarity.Common,    50f);
        CreateCard("속사",        "공격 속도 +0.5",       CardEffectType.AttackSpeedUp,  CardRarity.Rare,      0.5f);
        CreateCard("사거리 확장", "사거리 +1.0",          CardEffectType.RangeUp,        CardRarity.Rare,      1.0f);
        CreateCard("이중 타격",   "공격력 2배",           CardEffectType.DoubleDamage,   CardRarity.Legendary, 0f);
        CreateCard("대공격",      "공격력 +40",           CardEffectType.AtkUp,          CardRarity.Rare,      40f);
        CreateCard("불사신",      "최대 체력 +150",       CardEffectType.HpUp,           CardRarity.Legendary, 150f);
        CreateCard("기관총",      "공격 속도 +1.5",       CardEffectType.AttackSpeedUp,  CardRarity.Legendary, 1.5f);

        // ── 웨이브 ─────────────────────────────────────────────
        CreateWave(1, new[] { ("Goblin", 5, 1.2f) });
        CreateWave(2, new[] { ("Goblin", 6, 1.0f), ("Orc", 2, 2.0f) });
        CreateWave(3, new[] { ("Orc", 4, 1.5f), ("Goblin", 8, 0.8f) });
        CreateWave(4, new[] { ("Troll", 3, 2.5f), ("Orc", 4, 1.2f) });
        CreateWave(5, new[] { ("BossGoblin", 1, 0f), ("Troll", 4, 1.5f), ("Orc", 6, 1.0f) });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[AssetCreator] 샘플 에셋 생성 완료!");
    }

    static void CreateCharacter(string name, int cost, float hp, float atk, float range, float speed)
    {
        string path = $"Assets/ScriptableObjects/Characters/{name}.asset";
        if (AssetExists(path)) return;
        var data = ScriptableObject.CreateInstance<CharacterData>();
        data.characterName = name;
        data.cost = cost;
        data.baseHp = hp;
        data.baseAtk = atk;
        data.attackRange = range;
        data.attackSpeed = speed;
        AssetDatabase.CreateAsset(data, path);
    }

    static void CreateEnemy(string name, float hp, float speed, int gold, int dmg)
    {
        string path = $"Assets/ScriptableObjects/Enemies/{name}.asset";
        if (AssetExists(path)) return;
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.enemyName = name;
        data.maxHp = hp;
        data.moveSpeed = speed;
        data.goldReward = gold;
        data.damage = dmg;
        AssetDatabase.CreateAsset(data, path);
    }

    static void CreateCard(string name, string desc, CardEffectType effect, CardRarity rarity, float value)
    {
        string safeName = name.Replace(" ", "_");
        string path = $"Assets/ScriptableObjects/Cards/{safeName}.asset";
        if (AssetExists(path)) return;
        var data = ScriptableObject.CreateInstance<CardData>();
        data.cardName = name;
        data.description = desc;
        data.effectType = effect;
        data.rarity = rarity;
        data.value = value;
        AssetDatabase.CreateAsset(data, path);
    }

    static void CreateWave(int number, (string enemy, int count, float interval)[] entries)
    {
        string path = $"Assets/ScriptableObjects/Waves/Wave{number}.asset";
        if (AssetExists(path)) return;
        var data = ScriptableObject.CreateInstance<WaveData>();
        data.waveNumber = number;
        data.delayBeforeWave = 2f;
        data.spawnEntries = new System.Collections.Generic.List<SpawnEntry>();
        foreach (var (eName, cnt, interval) in entries)
        {
            var enemyPath = $"Assets/ScriptableObjects/Enemies/{eName}.asset";
            var enemyData = AssetDatabase.LoadAssetAtPath<EnemyData>(enemyPath);
            if (enemyData == null) { Debug.LogWarning($"[AssetCreator] Enemy 에셋 없음: {eName}"); continue; }
            data.spawnEntries.Add(new SpawnEntry { enemyData = enemyData, count = cnt, spawnInterval = interval });
        }
        AssetDatabase.CreateAsset(data, path);
    }

    static void EnsureDir(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }

    static bool AssetExists(string path) => File.Exists(path);
}
#endif
