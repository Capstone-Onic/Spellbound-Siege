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
    public Button confirmButton; // 선택 버튼 연결

    private List<Card> unlockCandidates = new();
    private Card selectedCard;
    private List<Card> selectedDeck => DeckBuilderManager.Instance.selectedDeck;


    public void ShowUnlockOptions()
    {
        unlockPanel.SetActive(true);
        ClearContainer();

        // 잠긴 카드 계산
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

        // 무작위로 최대 2장 선택
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

        // 보상 카드 UI 생성
        float spacing = 250f; // 카드 간격
        int count = unlockCandidates.Count;
        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            Card card = unlockCandidates[i];
            GameObject obj = Instantiate(cardSlotPrefab, cardContainer);

            // 보상용 카드: 기능 제거
            Destroy(obj.GetComponent<CardDragHandler>());
            Destroy(obj.GetComponent<CardHoldZoom>());

            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.blocksRaycasts = false;
                cg.interactable = false;
            }

            // 카드 데이터 설정
            CardSelectionItem selector = obj.GetComponent<CardSelectionItem>();
            selector.SetCard(card, selectedDeck, true); // 보상 카드에서는 LockedMark 숨기도록
            selector.selectedMark.SetActive(false);

            // 보상 UI에서는 잠금 마크 숨기기
            Transform lockedMark = obj.transform.Find("LockedMark");
            if (lockedMark != null)
                lockedMark.gameObject.SetActive(false);

            // 클릭 이벤트
            Button btn = obj.GetComponent<Button>();
            btn.onClick.AddListener(() => SelectCard(card, obj));

            // 중심 정렬 위치 지정
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + i * spacing, 0);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners(); // 기존 이벤트 제거
            confirmButton.onClick.AddListener(ConfirmUnlock); // 새로 등록
            confirmButton.interactable = true; // 클릭 가능하도록
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
    }

    public void ConfirmUnlock()
    {
        if (selectedCard != null)
        {
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

        cardSelectPanel.SetActive(false); // 보상 창 닫기

        if (deckSettingButton != null)
            deckSettingButton.SetActive(true); // 덱 설정 다시 허용
    }

    private void ClearContainer()
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);
    }
}