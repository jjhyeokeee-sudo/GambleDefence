using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] private List<WaveData> waves;
    [SerializeField] private EnemySpawner spawner;

    private int currentWaveIndex = 0;
    private int aliveEnemies = 0;
    private bool isWaveRunning = false;

    public UnityEvent<int, int> onWaveChanged;   // (현재 웨이브, 전체 웨이브)
    public UnityEvent onAllWavesCleared;
    public UnityEvent onWaveCleared; // 웨이브 클리어 → 카드 선택 트리거용

    public int CurrentWave => currentWaveIndex + 1;
    public int TotalWaves => waves.Count;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Application.runInBackground = true;
    }

    [SerializeField] private float preparationTime = 10f;

    void Start()
    {
        onWaveChanged?.Invoke(CurrentWave, TotalWaves);
        StartCoroutine(PreparationPhase());
    }

    IEnumerator PreparationPhase()
    {
        yield return null; // 첫 프레임 대기 (HUDManager 초기화 보장)
        int seconds = Mathf.CeilToInt(preparationTime);
        while (seconds > 0)
        {
            HUDManager.Instance?.ShowWaveAlert($"캐릭터를 배치하세요! ({seconds}초)");
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        StartCoroutine(RunNextWave());
    }

    IEnumerator RunNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            GameManager.Instance.GameClear();
            onAllWavesCleared?.Invoke();
            yield break;
        }

        WaveData wave = waves[currentWaveIndex];
        isWaveRunning = true;
        aliveEnemies = 0;

        float elapsed = 0f;
        while (elapsed < wave.delayBeforeWave)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.Log($"[WaveManager] 웨이브 {CurrentWave} 시작!");
        onWaveChanged?.Invoke(CurrentWave, TotalWaves);

        yield return StartCoroutine(spawner.SpawnWave(wave));

        // 마지막 적이 죽을 때까지 대기
        yield return new WaitUntil(() => aliveEnemies <= 0);

        isWaveRunning = false;
        Debug.Log($"[WaveManager] 웨이브 {CurrentWave} 클리어!");

        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            GameManager.Instance.GameClear();
            onAllWavesCleared?.Invoke();
            yield break;
        }

        // 카드 선택 이벤트 발동 → CardManager가 일시정지 후 카드 뽑기
        onWaveCleared?.Invoke();
    }

    public void OnEnemySpawned() => aliveEnemies++;
    public void OnEnemyDead() => aliveEnemies = Mathf.Max(0, aliveEnemies - 1);

    // 카드 선택 완료 후 외부에서 호출
    public void StartNextWaveManually() => StartCoroutine(RunNextWave());
}
