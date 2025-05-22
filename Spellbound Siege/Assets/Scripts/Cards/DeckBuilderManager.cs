using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spellbound;

public class DeckBuilderManager : MonoBehaviour
{
    public static DeckBuilderManager Instance { get; private set; }

    public List<Card> allAvailableCards; // ScriptableObject로 만들어진 카드 전체 목록
    public List<Card> selectedDeck = new(); // 사용자가 선택한 카드들
    public int maxDeckSize = 8;
    public Transform cardListPanel;// 카드들이 들어갈 부모 패널
    public GameObject cardSlotPrefab; // CardSlotUI 프리팹
    public GameObject deckSettingPanel;       // 덱 설정 UI 패널
    public GameObject mainMenuPanel; // <- MainMenuPanel 연결 필요

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        PopulateAllCards();
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
        // 선택한 덱을 정적 변수로 보관
        DeckData.selectedDeck = new List<Card>(selectedDeck);
        SceneManager.LoadScene("Map1"); // 게임 씬 이름
    }

    public void PopulateAllCards()
    {
        // 기존에 있던 카드 슬롯 제거
        foreach (Transform child in cardListPanel)
            Destroy(child.gameObject);

        foreach (Card card in allAvailableCards)
        {
            GameObject slot = Instantiate(cardSlotPrefab, cardListPanel);
            CardSelectionItem selector = slot.GetComponent<CardSelectionItem>();
            selector.SetCard(card);
        }
    }

    public void ShowDeckSettingPanel()
    {
        if (deckSettingPanel != null)
            deckSettingPanel.SetActive(true);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false); // ← 메뉴 숨김

        PopulateAllCards(); // 카드 UI 생성
    }

    public void HideDeckSettingPanel()
    {
        if (deckSettingPanel != null)
        {
            deckSettingPanel.SetActive(false);
        }
    }
}