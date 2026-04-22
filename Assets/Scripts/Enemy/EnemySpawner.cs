using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyPath enemyPath;

    public static EnemySpawner Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // 웨이브 데이터를 받아서 순차적으로 적을 스폰
    public IEnumerator SpawnWave(WaveData waveData)
    {
        foreach (var entry in waveData.spawnEntries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.enemyData);
                float spawnElapsed = 0f;
                while (spawnElapsed < entry.spawnInterval)
                {
                    spawnElapsed += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
        }
    }

    void SpawnEnemy(EnemyData data)
    {
        Vector3 spawnPos = enemyPath.Waypoints[0].position;
        GameObject go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        go.GetComponent<Enemy>().Initialize(data, enemyPath.Waypoints);
        WaveManager.Instance.OnEnemySpawned();
    }
}
