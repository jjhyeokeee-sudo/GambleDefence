#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Tools > CardRoguelike > Setup Scene 으로 씬을 자동 세팅
public class SceneSetupWizard : EditorWindow
{
    [MenuItem("Tools/CardRoguelike/Setup Scene")]
    public static void SetupScene()
    {
        // 이미 있으면 스킵
        if (GameObject.Find("--- Managers ---") != null)
        {
            Debug.Log("[SceneSetup] 이미 세팅된 씬입니다.");
            return;
        }

        // ── Managers ─────────────────────────────────────────
        var managers = CreateEmpty("--- Managers ---");
        AddMono<GameManager>(CreateEmpty("GameManager", managers));
        AddMono<WaveManager>(CreateEmpty("WaveManager", managers));
        AddMono<CardManager>(CreateEmpty("CardManager", managers));

        var pmGO = CreateEmpty("PlacementManager", managers);
        AddMono<PlacementManager>(pmGO);

        var spawnerGO = CreateEmpty("EnemySpawner", managers);
        AddMono<EnemySpawner>(spawnerGO);

        // ── Map ──────────────────────────────────────────────
        var map = CreateEmpty("--- Map ---");

        // 적 경로 웨이포인트 (직선 6개)
        var pathGO = CreateEmpty("EnemyPath", map);
        AddMono<EnemyPath>(pathGO);
        var waypoints = new List<Transform>();
        float[] xs = { -9f, -5f, -1f, 3f, 7f, 9.5f };
        for (int i = 0; i < xs.Length; i++)
        {
            var wp = CreateEmpty($"Waypoint_{i + 1}", pathGO.transform);
            wp.transform.position = new Vector3(xs[i], 0f, 0f);
            waypoints.Add(wp.transform);
        }
        // EnemyPath에 waypoints 세팅은 수동으로 해야 함 (SerializedField)

        // 배치 타일 9개 (3x3 그리드, 위쪽)
        var tilesRoot = CreateEmpty("PlacementTiles", map);
        for (int row = 0; row < 3; row++)
            for (int col = 0; col < 3; col++)
            {
                var tileGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tileGO.name = $"Tile_{row}_{col}";
                tileGO.transform.SetParent(tilesRoot.transform);
                tileGO.transform.position = new Vector3(-2f + col * 1.5f, 1.5f + row * 1.5f, 0f);
                tileGO.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                DestroyImmediate(tileGO.GetComponent<MeshRenderer>());
                DestroyImmediate(tileGO.GetComponent<MeshFilter>());
                var sr = tileGO.AddComponent<SpriteRenderer>();
                sr.color = new Color(0.35f, 0.80f, 0.35f, 0.55f);
                tileGO.AddComponent<BoxCollider2D>();
                tileGO.AddComponent<PlacementTile>();
                tileGO.layer = LayerMask.NameToLayer("Default");
            }

        Debug.Log("[SceneSetup] 씬 세팅 완료! 다음 단계:");
        Debug.Log("  1. EnemyPath 컴포넌트에 Waypoint 오브젝트 6개 연결");
        Debug.Log("  2. WaveManager에 WaveData 연결, EnemySpawner 연결");
        Debug.Log("  3. EnemySpawner에 EnemyPrefab, EnemyPath 연결");
        Debug.Log("  4. PlacementManager에 CharacterPrefab, ProjectilePrefab, CharacterData 연결");
        Debug.Log("  5. Canvas/UI 세팅 → Tools > CardRoguelike > Setup UI");
    }

    [MenuItem("Tools/CardRoguelike/Setup UI")]
    public static void SetupUI()
    {
        if (GameObject.Find("--- UI ---") != null)
        {
            Debug.Log("[SceneSetup] UI 이미 세팅됨");
            return;
        }

        var uiRoot = CreateEmpty("--- UI ---");

        // Main Canvas
        var canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(uiRoot.transform);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // HUD Panel (상단)
        var hud = CreateUIPanel("HUD", canvasGO.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(0f, -40f), new Vector2(Screen.width, 80f));
        var hudMono = hud.AddComponent<HUDManager>();
        CreateTMPText("WaveText",   hud.transform, "Wave 1/5",    new Vector2(-300f, 0f), 24);
        CreateTMPText("GoldText",   hud.transform, "Gold: 100",   new Vector2(0f,    0f), 24);
        CreateTMPText("HPText",     hud.transform, "20 / 20",     new Vector2(300f,  0f), 24);
        CreateTMPText("AlertText",  hud.transform, "Wave Start!", new Vector2(0f, -60f), 32);

        // CardSelectionPanel (중앙, 기본 비활성)
        var cardPanel = CreateUIPanel("CardSelectionPanel", canvasGO.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 500f));
        var panelImg = cardPanel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.2f, 0.92f);
        cardPanel.AddComponent<CardSelectionPanel>();
        CreateTMPText("InstructionText", cardPanel.transform, "카드를 선택하세요", new Vector2(0f, 180f), 28);
        // 카드 슬롯 3개
        for (int i = 0; i < 3; i++)
        {
            var slot = CreateUIPanel($"CardSlot_{i}", cardPanel.transform,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-280f + i * 280f, -20f), new Vector2(240f, 340f));
            var slotImg = slot.AddComponent<Image>();
            slotImg.color = new Color(0.85f, 0.85f, 0.85f, 1f);
            slot.AddComponent<CardSlotUI>();
            slot.AddComponent<Button>();
            CreateTMPText("NameText",   slot.transform, "Card Name",  new Vector2(0f,  110f), 20);
            CreateTMPText("RarityText", slot.transform, "Common",     new Vector2(0f,  80f),  16);
            CreateTMPText("DescText",   slot.transform, "설명",        new Vector2(0f, -20f),  16);
        }
        // 캐릭터 버튼 부모
        var charList = CreateUIPanel("CharacterList", cardPanel.transform,
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 60f), new Vector2(800f, 80f));
        charList.AddComponent<HorizontalLayoutGroup>();
        cardPanel.SetActive(false);

        // CharacterSelectBar (하단)
        var charBar = CreateUIPanel("CharacterSelectBar", canvasGO.transform,
            new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 50f), new Vector2(0f, 100f));
        charBar.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 0.85f);
        var barMono = charBar.AddComponent<CharacterSelectBar>();
        var barLayout = charBar.AddComponent<HorizontalLayoutGroup>();
        barLayout.childAlignment = TextAnchor.MiddleCenter;
        barLayout.spacing = 10f;

        // GameResultPanel (중앙)
        var resultPanel = CreateUIPanel("GameResultPanel", canvasGO.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(500f, 300f));
        resultPanel.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.15f, 0.95f);
        resultPanel.AddComponent<GameResultPanel>();
        CreateTMPText("TitleText",    resultPanel.transform, "GAME OVER", new Vector2(0f,  80f), 48);
        CreateTMPText("SubtitleText", resultPanel.transform, "...",       new Vector2(0f,  10f), 22);
        var restartBtn = CreateButton("RestartButton", resultPanel.transform, "다시 시작",
            new Vector2(0f, -80f), new Vector2(200f, 50f));
        resultPanel.SetActive(false);

        // EventSystem
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Debug.Log("[SceneSetup] UI 세팅 완료! Inspector에서 각 컴포넌트의 레퍼런스를 연결해주세요.");
    }

    // ─── 헬퍼 ─────────────────────────────────────────────────

    static GameObject CreateEmpty(string name, Transform parent = null)
    {
        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent);
        return go;
    }

    static GameObject CreateEmpty(string name, GameObject parent)
        => CreateEmpty(name, parent.transform);

    static T AddMono<T>(GameObject go) where T : Component
        => go.GetComponent<T>() ?? go.AddComponent<T>();

    static GameObject CreateUIPanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;
        return go;
    }

    static TMP_Text CreateTMPText(string name, Transform parent, string text, Vector2 pos, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(400f, 50f);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return tmp;
    }

    static Button CreateButton(string name, Transform parent, string label, Vector2 pos, Vector2 size)
    {
        var go = CreateUIPanel(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);
        go.AddComponent<Image>().color = new Color(0.3f, 0.6f, 1f);
        var btn = go.AddComponent<Button>();
        CreateTMPText("Label", go.transform, label, Vector2.zero, 22);
        return btn;
    }
}
#endif
