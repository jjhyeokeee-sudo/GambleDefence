using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public EnemyData enemyData;
    public int count = 5;
    public float spawnInterval = 1f; // 적 간 스폰 간격 (초)
}

[CreateAssetMenu(fileName = "WaveData", menuName = "CardRoguelike/Wave Data")]
public class WaveData : ScriptableObject
{
    public int waveNumber;
    public List<SpawnEntry> spawnEntries;
    public float delayBeforeWave = 2f; // 웨이브 시작 전 대기 시간
}
