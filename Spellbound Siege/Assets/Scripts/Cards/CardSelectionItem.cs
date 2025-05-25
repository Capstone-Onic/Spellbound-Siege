using UnityEngine;
using UnityEngine.UI;
using Spellbound;
using System.Collections.Generic;

public class CardSelectionItem : MonoBehaviour
{
    public Card cardData;
    public GameObject selectedMark; // ���� ǥ�� �̹��� ����
    public GameObject lockedMark;

    private Button button;
    private bool isSelected = false;
    public bool isLocked => selectedMark.activeSelf;
    private bool suppressLockedMark = false; // ����� ī������ ����

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("[ī�弱��] Button ������Ʈ ����!");
            return;
        }

        button.onClick.AddListener(OnClickToggleSelect);
        Debug.Log($"[ī�弱��] ��ư �̺�Ʈ �����: {cardData?.cardName}");
    }

    public void SetCard(Card card, List<Card> selectedDeck, bool suppressLock = false)
    {
        suppressLockedMark = suppressLock;

        cardData = card;
        var cardDisplay = GetComponentInChildren<CardDisplay>();
        cardDisplay?.SetCard(card);

        if (selectedMark == null)
        {
            selectedMark = cardDisplay?.transform.Find("SelectedMark")?.gameObject;
            if (selectedMark == null)
                Debug.LogWarning($"[���ø�ũ ����] {card.cardName} ī���� SelectedMark�� ã�� ���߽��ϴ�.");
        }

        if (lockedMark == null)
            lockedMark = cardDisplay?.transform.Find("LockedMark")?.gameObject;

        bool isUnlocked = cardData.isUnlockedByDefault;
        Debug.Log($"[ī��] {cardData.cardName} �ر� ����: {isUnlocked}");

        // ��� ���¿� ���� ǥ�� ����
        if (lockedMark != null)
        {
            // suppressLock�� true�� ��� ������ ����
            lockedMark.SetActive(suppressLockedMark ? false : !isUnlocked);
            Debug.Log($"[���ǥ��] {cardData.cardName} �� ǥ�õ� ����: {!isUnlocked}");
        }
        else
        {
            Debug.LogWarning($"[����] {cardData.cardName}�� LockedMark�� ã�� �� ����!");
        }

        bool isInDeck = selectedDeck.Contains(card);
        Debug.Log($"[ī���] {card.cardName} �� Deck�� ���ԵǾ� �ֳ�? {isInDeck}");

        isSelected = selectedDeck.Contains(card);
        UpdateSelectionUI();
    }

    private void OnClickToggleSelect()
    {
        if (!cardData.isUnlockedByDefault)
        {
            Debug.Log("�� ī��� ��� �ֽ��ϴ�.");
            return;
        }

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
        Debug.Log($"[���û���] {cardData.cardName} �� ���õ�: {isSelected}");
        if (selectedMark != null)
        {
            selectedMark.SetActive(isSelected);
            Debug.Log($"[���ø�ũ] {cardData.cardName} �� Ȱ��ȭ��: {isSelected}");
        }
        else
        {
            Debug.LogWarning($"[���ø�ũ] {cardData.cardName} �� selectedMark�� null�Դϴ�!");
        }
    }
}