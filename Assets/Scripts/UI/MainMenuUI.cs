using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 메인화면 버튼 이벤트 연결
public class MainMenuUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        startButton?.onClick.AddListener(OnStartClicked);
        quitButton?.onClick.AddListener(OnQuitClicked);
    }

    void OnStartClicked()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadGame();
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }

    void OnQuitClicked()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.QuitGame();
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
