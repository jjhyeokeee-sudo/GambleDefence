using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    void Awake()
    {
        panel?.SetActive(false);
        restartButton?.onClick.AddListener(Restart);
        mainMenuButton?.onClick.AddListener(GoToMainMenu);
    }

    void Start()
    {
        GameManager.Instance.onGameOver.AddListener(ShowGameOver);
        GameManager.Instance.onGameClear.AddListener(ShowGameClear);
    }

    void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.onGameOver.RemoveListener(ShowGameOver);
        GameManager.Instance.onGameClear.RemoveListener(ShowGameClear);
    }

    void ShowGameOver()
    {
        panel?.SetActive(true);
        if (titleText != null)    titleText.text    = "GAME OVER";
        if (subtitleText != null) subtitleText.text = "기지가 함락되었습니다...";
    }

    void ShowGameClear()
    {
        panel?.SetActive(true);
        if (titleText != null)    titleText.text    = "CLEAR!";
        if (subtitleText != null) subtitleText.text = "모든 웨이브를 막아냈습니다!";
    }

    void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
