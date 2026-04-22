using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private CharacterData defaultCharacter;

    // CharacterSelectBar에서 노출될 전체 캐릭터 목록
    [SerializeField] public System.Collections.Generic.List<CharacterData> availableCharacters;

    private CharacterData selected;

    public static event System.Action<CharacterData> OnCharacterSelected;
    public CharacterData SelectedCharacter => selected;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        selected = defaultCharacter;
    }

    void Start()
    {
        OnCharacterSelected?.Invoke(selected);
    }

    public void SelectCharacter(CharacterData data)
    {
        selected = data;
        OnCharacterSelected?.Invoke(selected);
    }

    public bool CanAffordSelected()
    {
        var data = selected ?? defaultCharacter;
        return data != null && GameManager.Instance != null && GameManager.Instance.Gold >= data.cost;
    }

    public void TryPlace(PlacementTile tile)
    {
        var data = selected ?? defaultCharacter;
        if (data == null) { Debug.LogWarning("[PlacementManager] 선택된 캐릭터 없음"); return; }
        if (!GameManager.Instance.SpendGold(data.cost))
        {
            HUDManager.Instance?.ShowWaveAlert("골드 부족!");
            return;
        }

        var go = Instantiate(characterPrefab, tile.transform.position, Quaternion.identity);
        var ch = go.GetComponent<PlacedCharacter>();
        ch.Initialize(data, projectilePrefab);
        tile.SetOccupied(ch);
        Debug.Log($"[PlacementManager] {data.characterName} 배치 완료!");
    }
}
