using UnityEngine;
using UnityEngine.UI;
using Spellbound;

public class CardSelectionItem : MonoBehaviour
{
    public Card cardData;
    public GameObject selectedMark; // ���� ǥ�� �̹��� ����

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

        // ���� ���� �ʱ�ȭ
        isSelected = DeckBuilderManager.Instance.selectedDeck.Contains(card);
        UpdateSelectionUI();
    }

    private void OnClickToggleSelect()
    {
        if (!isSelected)
        {
            if (DeckBuilderManager.Instance.selectedDeck.Count >= DeckBuilderManager.Instance.maxDeckSize)
            {
                Debug.Log("�� �ִ� �� �ʰ�");
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