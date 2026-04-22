using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private Image cardBg;
    [SerializeField] private Image cardIcon;
    [SerializeField] private Button selectButton;

    // 희귀도별 색상
    static readonly Color ColCommon    = new Color(0.85f, 0.85f, 0.85f);
    static readonly Color ColRare      = new Color(0.40f, 0.70f, 1.00f);
    static readonly Color ColLegendary = new Color(1.00f, 0.80f, 0.10f);

    private CardData cardData;
    private Action<CardData> onSelected;

    public void Setup(CardData data, Action<CardData> callback)
    {
        cardData = data;
        onSelected = callback;

        if (nameText != null)  nameText.text  = data.cardName;
        if (descText != null)  descText.text  = data.description;
        if (rarityText != null) rarityText.text = data.rarity.ToString();
        if (cardIcon != null && data.cardSprite != null) cardIcon.sprite = data.cardSprite;

        if (cardBg != null)
        {
            cardBg.color = data.rarity switch
            {
                CardRarity.Rare      => ColRare,
                CardRarity.Legendary => ColLegendary,
                _                    => ColCommon,
            };
        }

        selectButton?.onClick.RemoveAllListeners();
        selectButton?.onClick.AddListener(() => onSelected?.Invoke(cardData));
    }
}
