using UnityEngine;
using UnityEngine.UI;
using Spellbound;

public class CardSelectionItem : MonoBehaviour
{
    public Card cardData;
    public GameObject selectedMark; // ���� ǥ�� �̹��� ����
    public GameObject lockedMark;

    private Button button;
    private bool isSelected = false;
    public bool isLocked => selectedMark.activeSelf;

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
        Debug.Log($"[ī��] {cardData.cardName} �ر� ����: {isUnlocked}");

        // ��� ���¿� ���� ǥ�� ����
        if (lockedMark != null)
        {
            lockedMark.SetActive(!isUnlocked);
            Debug.Log($"[���ǥ��] {cardData.cardName} �� ǥ�õ� ����: {!isUnlocked}");
        }
        else
        {
            Debug.LogWarning($"[����] {cardData.cardName}�� LockedMark�� ã�� �� ����!");
        }

        isSelected = isUnlocked && DeckBuilderManager.Instance.selectedDeck.Contains(card);
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
        if (selectedMark != null)
            selectedMark.SetActive(isSelected);
    }
}