using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spellbound;

public class UnlockCardManager : MonoBehaviour
{
    public GameObject unlockPanel;
    public Transform cardContainer;
    public GameObject cardSlotPrefab; // CardSelectionItem 기반
    public GameObject cardSelectPanel;
    public GameObject deckSettingButton; // 덱 설정 버튼
    public DeckBuilderManager deckBuilderManager; // 인스펙터 연결

    public GameObject selectFrame; // 선택 강조 프레임

    private List<Card> unlockCandidates = new();
    private Card selectedCard;

    public void ShowUnlockOptions()
    {
        unlockPanel.SetActive(true);
        ClearContainer();
        selectFrame.SetActive(false); // 초기에는 숨김

        // 잠금 카드 수집
        var allSelectors = FindObjectsOfType<CardSelectionItem>();
        List<Card> lockedCards = new();
        foreach (var selector in allSelectors)
        {
            if (selector.isLocked)
                lockedCards.Add(selector.cardData);
        }

        // 무작위로 최대 2장 선택
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

        // 카드 UI 생성
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

        // 모든 카드의 선택 마크 비활성화
        foreach (Transform child in cardContainer)
        {
            var mark = child.GetComponentInChildren<CardSelectionItem>()?.selectedMark;
            if (mark != null) mark.SetActive(false);
        }

        // 현재 선택된 카드만 선택 마크 표시
        obj.GetComponentInChildren<CardSelectionItem>().selectedMark.SetActive(true);

        // SelectFrame을 선택된 카드 위로 이동
        RectTransform targetRT = obj.GetComponent<RectTransform>();
        RectTransform frameRT = selectFrame.GetComponent<RectTransform>();

        frameRT.SetParent(cardContainer); // 같은 부모 기준
        frameRT.anchorMin = targetRT.anchorMin;
        frameRT.anchorMax = targetRT.anchorMax;
        frameRT.pivot = targetRT.pivot;
        frameRT.anchoredPosition = targetRT.anchoredPosition;
        frameRT.sizeDelta = targetRT.sizeDelta;
        frameRT.SetSiblingIndex(targetRT.GetSiblingIndex() + 1); // 위에 오도록
        selectFrame.SetActive(true);
    }

    public void ConfirmUnlock()
    {
        if (selectedCard != null)
        {
            DeckData.selectedDeck.Add(selectedCard);
            Debug.Log($"[해금 완료] {selectedCard.cardName}");
        }

        cardSelectPanel.SetActive(false);

        // 라운드 보상 이후 덱 설정 가능하게
        if (deckSettingButton != null)
            deckSettingButton.SetActive(true);
    }

    private void ClearContainer()
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);
    }
}