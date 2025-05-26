using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spellbound;

public class UnlockCardManager : MonoBehaviour
{
    public GameObject unlockPanel;
    public Transform cardContainer;
    public GameObject rewardCardSlotPrefab;
    public GameObject cardSelectPanel;
    public GameObject deckSettingButton;
    public DeckBuilderManager deckBuilderManager;
    public Button confirmButton;

    private List<Card> unlockCandidates = new();
    private Card selectedCard;
    private List<Card> selectedDeck => DeckBuilderManager.Instance.selectedDeck;

    public void ShowUnlockOptions()
    {
        unlockPanel.SetActive(true);
        ClearContainer();

        List<Card> lockedCards = new();
        foreach (var card in deckBuilderManager.allAvailableCards)
        {
            if (!DeckData.selectedDeck.Contains(card))
                lockedCards.Add(card);
        }

        if (lockedCards.Count == 0)
        {
            Debug.LogWarning("[UnlockCardManager] 해금 가능한 카드가 없습니다.");
            return;
        }

        unlockCandidates = new List<Card>();
        if (lockedCards.Count >= 2)
        {
            int first = Random.Range(0, lockedCards.Count);
            int second;
            do { second = Random.Range(0, lockedCards.Count); } while (second == first);
            unlockCandidates.Add(lockedCards[first]);
            unlockCandidates.Add(lockedCards[second]);
        }
        else
        {
            unlockCandidates = new List<Card>(lockedCards);
        }

        float spacing = 250f;
        int count = unlockCandidates.Count;
        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            Card card = unlockCandidates[i];
            GameObject obj = Instantiate(rewardCardSlotPrefab, cardContainer);

            CardSelectionItem selector = obj.GetComponent<CardSelectionItem>();
            selector.SetCard(card, selectedDeck, true);
            selector.selectedMark.SetActive(false);

            Transform lockedMark = obj.transform.Find("LockedMark");
            if (lockedMark != null)
                lockedMark.gameObject.SetActive(false);

            Button btn = obj.GetComponent<Button>();
            btn.onClick.AddListener(() => SelectCard(card, obj));

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + i * spacing, 0);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmUnlock);
            confirmButton.interactable = true;
        }
    }

    private void SelectCard(Card card, GameObject obj)
    {
        selectedCard = card;

        foreach (Transform child in cardContainer)
        {
            var mark = child.GetComponentInChildren<CardSelectionItem>()?.selectedMark;
            if (mark != null) mark.SetActive(false);
        }

        obj.GetComponentInChildren<CardSelectionItem>().selectedMark.SetActive(true);
    }

    public void ConfirmUnlock()
    {
        if (selectedCard != null)
        {
            selectedCard.isUnlockedByDefault = true;
            if (!DeckData.selectedDeck.Contains(selectedCard))
            {
                DeckData.selectedDeck.Add(selectedCard);
                Debug.Log($"[해금 완료] {selectedCard.cardName}");
            }
        }
        else
        {
            Debug.LogWarning("[보상 선택] 선택된 카드가 없습니다.");
        }

        cardSelectPanel.SetActive(false);

        if (deckSettingButton != null)
            deckSettingButton.SetActive(true);
    }

    private void ClearContainer()
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);
    }
}