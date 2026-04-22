using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("기지 설정")]
    [SerializeField] private int maxBaseHp = 20;
    private int currentBaseHp;

    [Header("골드")]
    [SerializeField] private int startingGold = 100;
    private int currentGold;

    // UI 업데이트용 이벤트
    public UnityEvent<int, int> onBaseHpChanged;  // (현재, 최대)
    public UnityEvent<int> onGoldChanged;
    public UnityEvent onGameOver;
    public UnityEvent onGameClear;

    public int Gold => currentGold;
    public int BaseHp => currentBaseHp;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        currentBaseHp = maxBaseHp;
        currentGold = startingGold;

        onBaseHpChanged?.Invoke(currentBaseHp, maxBaseHp);
        onGoldChanged?.Invoke(currentGold);
    }

    public void TakeDamage(int amount)
    {
        currentBaseHp = Mathf.Max(0, currentBaseHp - amount);
        onBaseHpChanged?.Invoke(currentBaseHp, maxBaseHp);

        if (currentBaseHp <= 0)
            GameOver();
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        onGoldChanged?.Invoke(currentGold);
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount) return false;
        currentGold -= amount;
        onGoldChanged?.Invoke(currentGold);
        return true;
    }

    void GameOver()
    {
        Debug.Log("[GameManager] 게임 오버!");
        Time.timeScale = 0f;
        onGameOver?.Invoke();
    }

    public void GameClear()
    {
        Debug.Log("[GameManager] 게임 클리어!");
        Time.timeScale = 0f;
        onGameClear?.Invoke();
    }
}
