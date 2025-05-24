using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spellbound;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DeckBuilderManager : MonoBehaviour
{
    public static DeckBuilderManager Instance { get; private set; }

    [Header("전체 카드 목록 및 기본 카드 설정")]
    public List<Card> allAvailableCards;          // ScriptableObject로 만들어진 카드 전체 목록
    public List<Card> defaultUnlockedCards;       // 기본 해금 카드 (에디터에서 설정)

    [Header("UI 연결")]
    public GameObject cardSlotPrefab;
    public GameObject deckSettingPanel;
    public GameObject deckSettingButton;
    public GameObject mainMenuPanel;
    public TextMeshProUGUI currentPageText;
    public Transform cardRowContainer;

    [Header("덱 설정")]
    public int maxDeckSize = 8;
    public List<Card> selectedDeck = new(); // 사용자가 선택한 카드들
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
        AutoUnlockAndSelectDefaultCards(); // 기본 해금 카드 설정
        PopulateAllCards();                // 카드 UI 생성
    }

    private void AutoUnlockAndSelectDefaultCards()
    {
        selectedDeck.Clear();

        foreach (Card card in defaultUnlockedCards)
        {
            if (!card.isUnlockedByDefault)
                card.isUnlockedByDefault = true;

            if (!selectedDeck.Contains(card) && selectedDeck.Count < maxDeckSize)
            {
                selectedDeck.Add(card);
                Debug.Log($"[기본카드] {card.cardName} 해금 및 덱 자동 포함");
            }
        }
    }

    public void AddCardToDeck(Card card)
    {
        if (selectedDeck.Count >= maxDeckSize) return;
        if (!selectedDeck.Contains(card))
        {
            selectedDeck.Add(card);
            Debug.Log($"{card.cardName} 카드가 덱에 추가됨");
        }
    }

    public void RemoveCardFromDeck(Card card)
    {
        if (selectedDeck.Contains(card))
        {
            selectedDeck.Remove(card);
            Debug.Log($"{card.cardName} 카드가 덱에서 제거됨");
        }
    }

    public void StartGame()
    {
        DeckData.selectedDeck = new List<Card>(selectedDeck);
        SceneManager.LoadScene("GameScene");
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

        float cardWidth = 150f; // 기본값 (실패 대비)

        for (int i = 0; i < displayCount; i++)
        {
            Card card = sortedCards[i];
            GameObject cardObj = Instantiate(cardSlotPrefab, cardRowContainer);

            RectTransform rt = cardObj.GetComponent<RectTransform>();

            // 첫 번째 카드로부터 가로 길이 측정
            if (i == 0 && rt != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rt); // 만약 사이즈가 아직 적용되지 않은 경우
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
            selector.SetCard(card);

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
            deckSettingPanel.SetActive(true); // 먼저 보이게

        if (deckSettingButton != null)
            deckSettingButton.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        currentPage = 0;

        // 프레임 끝에 정렬 처리 (UI 레이아웃 적용 이후)
        StartCoroutine(DelayedDisplayPage());
    }

    private IEnumerator DelayedDisplayPage()
    {
        yield return null; // UI가 완전히 활성화된 다음 프레임까지 대기
        DisplayCurrentPage(); // 카드 위치 재정렬
    }

    public void HideDeckSettingPanel()
    {
        if (deckSettingPanel != null)
            deckSettingPanel.SetActive(false);

        if (deckSettingButton != null)
            deckSettingButton.SetActive(true);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
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
                int slotIndex = i - startIndex; // 페이지 내에서 0~3 인덱스
                float slotCenter = (slotIndex + 0.5f) * slotWidth;
                float x = slotCenter - containerWidth / 2f;

                rt.anchoredPosition = new Vector2(x, yPosition);
            }

            CardSelectionItem selector = cardObj.GetComponent<CardSelectionItem>();
            selector.SetCard(card);

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