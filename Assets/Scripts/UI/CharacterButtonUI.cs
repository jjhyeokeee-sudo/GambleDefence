using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 하단 캐릭터 소환 바의 각 버튼
public class CharacterButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image portrait;
    [SerializeField] private Button button;

    private CharacterData data;

    private static readonly Color SelectedNormalColor = new Color(1f, 0.85f, 0.2f);
    private static readonly Color DefaultNormalColor  = Color.white;

    public void Setup(CharacterData characterData)
    {
        data = characterData;
        if (nameText != null) nameText.text = data.characterName;
        if (costText != null) costText.text = $"{data.cost}G";
        if (portrait != null && data.sprite != null)
        {
            portrait.sprite = data.sprite;
            portrait.color = Color.white;
        }
        button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(OnClick);

        GameManager.Instance.onGoldChanged.AddListener(OnGoldChanged);
        PlacementManager.OnCharacterSelected += OnSelectionChanged;
        RefreshInteractable();

        // 이미 선택된 캐릭터가 있으면 즉시 반영
        if (PlacementManager.Instance != null)
            OnSelectionChanged(PlacementManager.Instance.SelectedCharacter);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGoldChanged.RemoveListener(OnGoldChanged);
        PlacementManager.OnCharacterSelected -= OnSelectionChanged;
    }

    void OnGoldChanged(int _) => RefreshInteractable();

    void OnClick()
    {
        PlacementManager.Instance?.SelectCharacter(data);
    }

    void OnSelectionChanged(CharacterData selected)
    {
        if (button == null) return;
        var colors = button.colors;
        colors.normalColor = (selected == data) ? SelectedNormalColor : DefaultNormalColor;
        button.colors = colors;
    }

    void RefreshInteractable()
    {
        if (button == null || GameManager.Instance == null) return;
        button.interactable = GameManager.Instance.Gold >= data.cost;
    }
}
