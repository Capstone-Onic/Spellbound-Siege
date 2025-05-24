using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spellbound;

public class UnlockCardManager : MonoBehaviour
{
    public GameObject unlockPanel;
    public Transform cardContainer;
    public GameObject cardSlotPrefab; // CardSelectionItem ���
    public GameObject cardSelectPanel;
    public GameObject deckSettingButton; // �� ���� ��ư
    public DeckBuilderManager deckBuilderManager; // �ν����� ����

    public GameObject selectFrame; // ���� ���� ������

    private List<Card> unlockCandidates = new();
    private Card selectedCard;

    public void ShowUnlockOptions()
    {
        unlockPanel.SetActive(true);
        ClearContainer();
        selectFrame.SetActive(false); // �ʱ⿡�� ����

        // ��� ī�� ����
        var allSelectors = FindObjectsOfType<CardSelectionItem>();
        List<Card> lockedCards = new();
        foreach (var selector in allSelectors)
        {
            if (selector.isLocked)
                lockedCards.Add(selector.cardData);
        }

        // �������� �ִ� 2�� ����
        unlockCandidates = new List<Card>();
        if (lockedCards.Count >= 2)
        {
            int first = Random.Range(0, lockedCards.Count);
            int second;
            do
            {
                second = Random.Range(0, lockedCards.Count);
            } while (second == first);

            unlockCandidates.Add(lockedCards[first]);
            unlockCandidates.Add(lockedCards[second]);
        }
        else
        {
            unlockCandidates = new List<Card>(lockedCards);
        }

        // ī�� UI ����
        foreach (var card in unlockCandidates)
        {
            GameObject obj = Instantiate(cardSlotPrefab, cardContainer);
            CardSelectionItem selector = obj.GetComponent<CardSelectionItem>();
            selector.SetCard(card);
            selector.selectedMark.SetActive(false);

            Button btn = obj.GetComponent<Button>();
            btn.onClick.AddListener(() => SelectCard(card, obj));
        }
    }

    private void SelectCard(Card card, GameObject obj)
    {
        selectedCard = card;

        // ��� ī���� ���� ��ũ ��Ȱ��ȭ
        foreach (Transform child in cardContainer)
        {
            var mark = child.GetComponentInChildren<CardSelectionItem>()?.selectedMark;
            if (mark != null) mark.SetActive(false);
        }

        // ���� ���õ� ī�常 ���� ��ũ ǥ��
        obj.GetComponentInChildren<CardSelectionItem>().selectedMark.SetActive(true);

        // SelectFrame�� ���õ� ī�� ���� �̵�
        RectTransform targetRT = obj.GetComponent<RectTransform>();
        RectTransform frameRT = selectFrame.GetComponent<RectTransform>();

        frameRT.SetParent(cardContainer); // ���� �θ� ����
        frameRT.anchorMin = targetRT.anchorMin;
        frameRT.anchorMax = targetRT.anchorMax;
        frameRT.pivot = targetRT.pivot;
        frameRT.anchoredPosition = targetRT.anchoredPosition;
        frameRT.sizeDelta = targetRT.sizeDelta;
        frameRT.SetSiblingIndex(targetRT.GetSiblingIndex() + 1); // ���� ������
        selectFrame.SetActive(true);
    }

    public void ConfirmUnlock()
    {
        if (selectedCard != null)
        {
            DeckData.selectedDeck.Add(selectedCard);
            Debug.Log($"[�ر� �Ϸ�] {selectedCard.cardName}");
        }

        cardSelectPanel.SetActive(false);

        // ���� ���� ���� �� ���� �����ϰ�
        if (deckSettingButton != null)
            deckSettingButton.SetActive(true);
    }

    private void ClearContainer()
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);
    }
}