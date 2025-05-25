using UnityEngine;
using UnityEngine.UI;
using Spellbound;
using System.Collections.Generic;

public class CardSelectionItem : MonoBehaviour
{
    public Card cardData;
    public GameObject selectedMark; // 선택 표시 이미지 연결
    public GameObject lockedMark;

    private Button button;
    private bool isSelected = false;
    public bool isLocked => selectedMark.activeSelf;
    private bool suppressLockedMark = false; // 보상용 카드인지 구분

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("[카드선택] Button 컴포넌트 없음!");
            return;
        }

        button.onClick.AddListener(OnClickToggleSelect);
        Debug.Log($"[카드선택] 버튼 이벤트 연결됨: {cardData?.cardName}");
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
                Debug.LogWarning($"[선택마크 없음] {card.cardName} 카드의 SelectedMark를 찾지 못했습니다.");
        }

        if (lockedMark == null)
            lockedMark = cardDisplay?.transform.Find("LockedMark")?.gameObject;

        bool isUnlocked = cardData.isUnlockedByDefault;
        Debug.Log($"[카드] {cardData.cardName} 해금 여부: {isUnlocked}");

        // 잠금 상태에 따라 표시 설정
        if (lockedMark != null)
        {
            // suppressLock이 true일 경우 강제로 숨김
            lockedMark.SetActive(suppressLockedMark ? false : !isUnlocked);
            Debug.Log($"[잠금표시] {cardData.cardName} → 표시됨 여부: {!isUnlocked}");
        }
        else
        {
            Debug.LogWarning($"[오류] {cardData.cardName}의 LockedMark를 찾을 수 없음!");
        }

        bool isInDeck = selectedDeck.Contains(card);
        Debug.Log($"[카드비교] {card.cardName} → Deck에 포함되어 있나? {isInDeck}");

        isSelected = selectedDeck.Contains(card);
        UpdateSelectionUI();
    }

    private void OnClickToggleSelect()
    {
        if (!cardData.isUnlockedByDefault)
        {
            Debug.Log("이 카드는 잠겨 있습니다.");
            return;
        }

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
        Debug.Log($"[선택상태] {cardData.cardName} → 선택됨: {isSelected}");
        if (selectedMark != null)
        {
            selectedMark.SetActive(isSelected);
            Debug.Log($"[선택마크] {cardData.cardName} → 활성화됨: {isSelected}");
        }
        else
        {
            Debug.LogWarning($"[선택마크] {cardData.cardName} → selectedMark가 null입니다!");
        }
    }
}