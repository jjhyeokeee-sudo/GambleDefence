using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("기지 HP")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

    [Header("골드")]
    [SerializeField] private TMP_Text goldText;

    [Header("웨이브")]
    [SerializeField] private TMP_Text waveText;

    [Header("웨이브 알림")]
    [SerializeField] private TMP_Text waveAlertText;
    [SerializeField] private float alertDuration = 2f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        GameManager.Instance.onBaseHpChanged.AddListener(UpdateHP);
        GameManager.Instance.onGoldChanged.AddListener(UpdateGold);
        WaveManager.Instance.onWaveChanged.AddListener(UpdateWave);

        if (waveAlertText != null) waveAlertText.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onBaseHpChanged.RemoveListener(UpdateHP);
            GameManager.Instance.onGoldChanged.RemoveListener(UpdateGold);
        }
        if (WaveManager.Instance != null)
            WaveManager.Instance.onWaveChanged.RemoveListener(UpdateWave);
    }

    void UpdateHP(int cur, int max)
    {
        if (hpSlider != null) { hpSlider.maxValue = max; hpSlider.value = cur; }
        if (hpText != null) hpText.text = $"{cur} / {max}";
    }

    void UpdateGold(int gold)
    {
        if (goldText != null) goldText.text = $"Gold: {gold}";
    }

    void UpdateWave(int cur, int total)
    {
        if (waveText != null) waveText.text = $"Wave {cur} / {total}";
        ShowWaveAlert($"Wave {cur} Start!");
    }

    public void ShowWaveAlert(string msg)
    {
        if (waveAlertText == null) return;
        StopAllCoroutines();
        StartCoroutine(ShowAlertRoutine(msg));
    }

    System.Collections.IEnumerator ShowAlertRoutine(string msg)
    {
        waveAlertText.text = msg;
        waveAlertText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(alertDuration);
        waveAlertText.gameObject.SetActive(false);
    }
}
