using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 웨이브 클리어 후 카드 3장을 보여주고 플레이어가 하나를 선택하게 함
public class CardSelectionPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private List<CardSlotUI> cardSlots;

    [Header("캐릭터 선택 안내")]
    [SerializeField] private TMP_Text instructionText;

    [Header("캐릭터 리스트 (카드 적용 대상 선택용)")]
    [SerializeField] private Transform characterListRoot;
    [SerializeField] private GameObject characterButtonPrefab;

    private CardData pendingCard;
    private bool waitingForTarget;

    void Awake()
    {
        panel?.SetActive(false);
    }

    void Start()
    {
        CardManager.Instance.onCardsDraw.AddListener(ShowCards);
    }

    void OnDestroy()
    {
        if (CardManager.Instance != null)
            CardManager.Instance.onCardsDraw.RemoveListener(ShowCards);
    }

    void ShowCards(List<CardData> cards)
    {
        panel?.SetActive(true);

        for (int i = 0; i < cardSlots.Count; i++)
        {
            bool active = i < cards.Count;
            cardSlots[i].gameObject.SetActive(active);
            if (active)
            {
                var card = cards[i];
                cardSlots[i].Setup(card, OnCardPicked);
            }
        }

        if (instructionText != null)
            instructionText.text = "카드를 선택하세요";

        waitingForTarget = false;
        ClearCharacterButtons();
    }

    void OnCardPicked(CardData card)
    {
        pendingCard = card;
        waitingForTarget = true;
        ShowCharacterTargets();
    }

    void ShowCharacterTargets()
    {
        ClearCharacterButtons();

        var characters = CardManager.Instance.AllCharacters;
        if (characters.Count == 0)
        {
            if (instructionText != null)
                instructionText.text = "배치된 캐릭터가 없습니다.\n2초 후 다음 웨이브로 이동합니다.";
            foreach (var slot in cardSlots) slot.gameObject.SetActive(false);
            StartCoroutine(DelayedSkip());
            return;
        }

        if (instructionText != null)
            instructionText.text = "적용할 캐릭터를 선택하세요";

        // 카드 슬롯 숨기고 캐릭터 버튼 표시
        foreach (var slot in cardSlots) slot.gameObject.SetActive(false);

        foreach (var ch in characters)
        {
            if (characterButtonPrefab == null || characterListRoot == null) break;
            var go = Instantiate(characterButtonPrefab, characterListRoot);
            var btn = go.GetComponent<Button>();
            var label = go.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = ch.Data?.characterName ?? "캐릭터";

            var captured = ch;
            btn?.onClick.AddListener(() => ApplyToCharacter(captured));
        }
    }

    void ApplyToCharacter(PlacedCharacter target)
    {
        if (pendingCard == null) return;
        panel?.SetActive(false);
        ClearCharacterButtons();
        CardManager.Instance.ApplyCard(pendingCard, target);
        pendingCard = null;
    }

    System.Collections.IEnumerator DelayedSkip()
    {
        yield return new WaitForSecondsRealtime(2f);
        SkipCard();
    }

    void SkipCard()
    {
        panel?.SetActive(false);
        ClearCharacterButtons();
        pendingCard = null;
        Time.timeScale = 1f;
        WaveManager.Instance.StartNextWaveManually();
    }

    void ClearCharacterButtons()
    {
        if (characterListRoot == null) return;
        foreach (Transform child in characterListRoot)
            Destroy(child.gameObject);
    }
}
