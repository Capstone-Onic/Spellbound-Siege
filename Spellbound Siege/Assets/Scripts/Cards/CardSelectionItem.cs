using UnityEngine;
using UnityEngine.UI;
using Spellbound;

public class CardSelectionItem : MonoBehaviour
{
    public Card cardData;
    public GameObject selectedMark; // 선택 표시 이미지 연결
    public GameObject lockedMark;

    private Button button;
    private bool isSelected = false;
    public bool isLocked => selectedMark.activeSelf;

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

    public void SetCard(Card card)
    {
        cardData = card;
        var cardDisplay = GetComponentInChildren<CardDisplay>();
        cardDisplay?.SetCard(card);

        if (selectedMark == null)
            selectedMark = cardDisplay?.transform.Find("SelectedMark")?.gameObject;

        if (lockedMark == null)
            lockedMark = cardDisplay?.transform.Find("LockedMark")?.gameObject;

        bool isUnlocked = cardData.isUnlockedByDefault;
        Debug.Log($"[카드] {cardData.cardName} 해금 여부: {isUnlocked}");

        // 잠금 상태에 따라 표시 설정
        if (lockedMark != null)
        {
            lockedMark.SetActive(!isUnlocked);
            Debug.Log($"[잠금표시] {cardData.cardName} → 표시됨 여부: {!isUnlocked}");
        }
        else
        {
            Debug.LogWarning($"[오류] {cardData.cardName}의 LockedMark를 찾을 수 없음!");
        }

        isSelected = isUnlocked && DeckBuilderManager.Instance.selectedDeck.Contains(card);
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
        if (selectedMark != null)
            selectedMark.SetActive(isSelected);
    }
}