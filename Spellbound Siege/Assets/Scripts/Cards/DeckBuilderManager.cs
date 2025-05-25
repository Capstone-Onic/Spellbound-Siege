using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spellbound;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class DeckBuilderManager : MonoBehaviour
{
    public static DeckBuilderManager Instance { get; private set; }

    [Header("전체 카드 목록 및 기본 카드 설정")]
    public List<Card> allAvailableCards;
    public List<Card> defaultUnlockedCards;

    [Header("UI 연결")]
    public GameObject cardSlotPrefab;
    public GameObject deckSettingPanel;
    public GameObject deckSettingButton;
    public GameObject mainMenuPanel;
    public TextMeshProUGUI currentPageText;
    public Transform cardRowContainer;

    [Header("덱 설정")]
    public int maxDeckSize = 8;
    public List<Card> selectedDeck = new();
    private int currentPage = 0;
    private int cardsPerPage = 4;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        selectedDeck = DeckData.selectedDeck
            .GroupBy(d => d.cardName)
            .Select(g => allAvailableCards.Find(c => c.cardName == g.Key))
            .Where(c => c != null)
            .ToList();

        AutoUnlockAndSelectDefaultCards();

        foreach (Card card in selectedDeck)
        {
            if (!card.isUnlockedByDefault)
                card.isUnlockedByDefault = true;
        }

        PopulateAllCards();
    }

    private void AutoUnlockAndSelectDefaultCards()
    {
        foreach (Card card in defaultUnlockedCards)
        {
            if (!card.isUnlockedByDefault)
                card.isUnlockedByDefault = true;

            if (!selectedDeck.Any(c => c.cardName == card.cardName) && selectedDeck.Count < maxDeckSize)
            {
                selectedDeck.Add(card);
                Debug.Log($"[기본카드] {card.cardName} 해금 및 덱 자동 포함");
            }
        }
    }

    public void AddCardToDeck(Card card)
    {
        if (selectedDeck.Count >= maxDeckSize) return;
        if (!selectedDeck.Any(c => c.cardName == card.cardName))
        {
            selectedDeck.Add(card);
            Debug.Log($"{card.cardName} 카드가 덱에 추가됨");
        }
    }

    public void RemoveCardFromDeck(Card card)
    {
        Card found = selectedDeck.Find(c => c.cardName == card.cardName);
        if (found != null)
        {
            selectedDeck.Remove(found);
            Debug.Log($"{card.cardName} 카드가 덱에서 제거됨");
        }
    }

    public void StartGame()
    {
        DeckData.selectedDeck = selectedDeck
            .GroupBy(c => c.cardName)
            .Select(g => g.First())
            .ToList();

        Debug.Log("[StartGame] 최종 덱:");
        foreach (var card in DeckData.selectedDeck)
        {
            Debug.Log($" - {card.cardName} ({card.GetInstanceID()})");
        }

        SceneManager.LoadScene("kjmGameScene");
    }

    public void PopulateAllCards()
    {
        foreach (Transform child in cardRowContainer)
            Destroy(child.gameObject);

        List<Card> sortedCards = new List<Card>(allAvailableCards);
        sortedCards.Sort((a, b) => a.cardName.CompareTo(b.cardName));

        RectTransform containerRect = cardRowContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float yPosition = 0f;

        int displayCount = Mathf.Min(4, sortedCards.Count);

        float cardWidth = 150f;

        for (int i = 0; i < displayCount; i++)
        {
            Card card = sortedCards[i];
            GameObject cardObj = Instantiate(cardSlotPrefab, cardRowContainer);

            RectTransform rt = cardObj.GetComponent<RectTransform>();
            if (i == 0 && rt != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
                cardWidth = rt.rect.width;
                Debug.Log($"[측정됨] 카드 너비: {cardWidth}");
            }

            if (rt != null)
            {
                float slotWidth = containerWidth / 4f;
                float slotCenter = (i + 0.5f) * slotWidth;
                float x = slotCenter - containerWidth / 2f;
                rt.anchoredPosition = new Vector2(x, yPosition);
            }

            CardSelectionItem selector = cardObj.GetComponent<CardSelectionItem>();
            selector.SetCard(card, selectedDeck);

            Transform lockedMark = cardObj.transform.Find("LockedMark");
            if (lockedMark != null)
                lockedMark.gameObject.SetActive(!card.isUnlockedByDefault);

            cardObj.transform.localScale = Vector3.one * 0.85f;
        }

        if (currentPageText != null)
            currentPageText.text = $"표시 중: {displayCount} / {sortedCards.Count} 장";
    }

    public void ShowDeckSettingPanel()
    {
        if (deckSettingPanel != null)
            deckSettingPanel.SetActive(true);

        if (deckSettingButton != null)
            deckSettingButton.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        currentPage = 0;
        StartCoroutine(DelayedDisplayPage());
    }

    private IEnumerator DelayedDisplayPage()
    {
        yield return null;
        DisplayCurrentPage();
    }

    public void HideDeckSettingPanel(bool showDeckButton = true)
    {
        if (deckSettingPanel != null)
            deckSettingPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (deckSettingButton != null)
            deckSettingButton.SetActive(showDeckButton);
    }

    public void DisplayCurrentPage()
    {
        int startIndex = currentPage * cardsPerPage;
        int endIndex = Mathf.Min(startIndex + cardsPerPage, allAvailableCards.Count);

        List<Card> sortedCards = new List<Card>(allAvailableCards);
        sortedCards.Sort((a, b) => a.cardName.CompareTo(b.cardName));

        foreach (Transform child in cardRowContainer)
            Destroy(child.gameObject);

        RectTransform containerRect = cardRowContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float yPosition = 0f;

        float slotWidth = containerWidth / 4f;

        for (int i = startIndex; i < endIndex; i++)
        {
            Card card = sortedCards[i];
            GameObject cardObj = Instantiate(cardSlotPrefab, cardRowContainer);
            RectTransform rt = cardObj.GetComponent<RectTransform>();

            if (rt != null)
            {
                int slotIndex = i - startIndex;
                float slotCenter = (slotIndex + 0.5f) * slotWidth;
                float x = slotCenter - containerWidth / 2f;

                rt.anchoredPosition = new Vector2(x, yPosition);
            }

            CardSelectionItem selector = cardObj.GetComponent<CardSelectionItem>();
            selector.SetCard(card, selectedDeck);

            Transform lockedMark = cardObj.transform.Find("LockedMark");
            if (lockedMark != null)
                lockedMark.gameObject.SetActive(!card.isUnlockedByDefault);

            cardObj.transform.localScale = Vector3.one * 0.85f;
        }

        if (currentPageText != null)
        {
            int totalPages = Mathf.CeilToInt((float)allAvailableCards.Count / cardsPerPage);
            currentPageText.text = $"<b><size=36><color=#FFD700>{currentPage + 1}</color></size></b> / {totalPages} 페이지";
        }
    }

    public void OnClickNextPage()
    {
        if ((currentPage + 1) * cardsPerPage < allAvailableCards.Count)
        {
            currentPage++;
            DisplayCurrentPage();
        }
    }

    public void OnClickPrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            DisplayCurrentPage();
        }
    }
}