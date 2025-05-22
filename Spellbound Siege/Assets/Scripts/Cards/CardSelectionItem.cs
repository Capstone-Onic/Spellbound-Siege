using UnityEngine;
using UnityEngine.UI;
using Spellbound;

public class CardSelectionItem : MonoBehaviour
{
    public Card cardData;
    public GameObject selectedMark; // 선택 표시 이미지 연결

    private Button button;
    private bool isSelected = false;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClickToggleSelect);
    }

    public void SetCard(Card card)
    {
        cardData = card;
        GetComponentInChildren<CardDisplay>()?.SetCard(card);

        // 선택 상태 초기화
        isSelected = DeckBuilderManager.Instance.selectedDeck.Contains(card);
        UpdateSelectionUI();
    }

    private void OnClickToggleSelect()
    {
        if (!isSelected)
        {
            if (DeckBuilderManager.Instance.selectedDeck.Count >= DeckBuilderManager.Instance.maxDeckSize)
            {
                Debug.Log("덱 최대 수 초과");
                return;
            }

            DeckBuilderManager.Instance.AddCardToDeck(cardData);
            isSelected = true;
        }
        else
        {
            DeckBuilderManager.Instance.RemoveCardFromDeck(cardData);
            isSelected = false;
        }

        UpdateSelectionUI();
    }

    private void UpdateSelectionUI()
    {
        if (selectedMark != null)
            selectedMark.SetActive(isSelected);
    }
}