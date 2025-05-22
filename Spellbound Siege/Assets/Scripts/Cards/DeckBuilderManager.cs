using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spellbound;

public class DeckBuilderManager : MonoBehaviour
{
    public static DeckBuilderManager Instance { get; private set; }

    public List<Card> allAvailableCards; // ScriptableObject�� ������� ī�� ��ü ���
    public List<Card> selectedDeck = new(); // ����ڰ� ������ ī���
    public int maxDeckSize = 8;
    public Transform cardListPanel;// ī����� �� �θ� �г�
    public GameObject cardSlotPrefab; // CardSlotUI ������
    public GameObject deckSettingPanel;       // �� ���� UI �г�
    public GameObject mainMenuPanel; // <- MainMenuPanel ���� �ʿ�

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
            Debug.Log($"{card.cardName} ī�尡 ���� �߰���");
        }
    }

    public void RemoveCardFromDeck(Card card)
    {
        if (selectedDeck.Contains(card))
        {
            selectedDeck.Remove(card);
            Debug.Log($"{card.cardName} ī�尡 ������ ���ŵ�");
        }
    }

    public void StartGame()
    {
        // ������ ���� ���� ������ ����
        DeckData.selectedDeck = new List<Card>(selectedDeck);
        SceneManager.LoadScene("Map1"); // ���� �� �̸�
    }

    public void PopulateAllCards()
    {
        // ������ �ִ� ī�� ���� ����
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
            mainMenuPanel.SetActive(false); // �� �޴� ����

        PopulateAllCards(); // ī�� UI ����
    }

    public void HideDeckSettingPanel()
    {
        if (deckSettingPanel != null)
        {
            deckSettingPanel.SetActive(false);
        }
    }
}