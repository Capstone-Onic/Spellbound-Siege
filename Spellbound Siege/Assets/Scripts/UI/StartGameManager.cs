using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spellbound;

public class StartGameManager : MonoBehaviour
{
    public static bool gameStarted = false;
    public static bool isPlacementPhase = true;

    public GameObject unitSelectionPanel;
    public GameObject startButton;
    public GameObject cardUI;
    public GameObject enemySpawner;
    public Button startButtonComponent;
    public CardDrawManager cardDrawManager;
    public RoundManager roundManager;

    public GameObject deckSettingButton;
    public DeckBuilderManager deckBuilderManager;
    public GameObject manaUI;

    void Start()
    {
        gameStarted = false;
        BGMManager.Instance?.PlayIntermissionMusic();

        if (manaUI != null)
            manaUI.SetActive(false);

        if (cardUI != null) cardUI.SetActive(false);
        if (startButton != null) startButton.SetActive(true);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(true);
        if (enemySpawner != null) enemySpawner.SetActive(false);

        if (startButtonComponent != null)
            startButtonComponent.onClick.AddListener(OnStartButtonClicked);

        if (DeckData.selectedDeck.Count > 0 && cardDrawManager != null)
        {
            cardDrawManager.deck = new List<Card>(DeckData.selectedDeck);
            cardDrawManager.ResetDeck();
        }
    }

    public void OnStartButtonClicked()
    {
        Debug.Log("[StartGameManager] OnStartButtonClicked хёО©╫О©╫О©╫");

        gameStarted = true;
        isPlacementPhase = false;

        if (deckBuilderManager != null)
        {
            DeckData.selectedDeck = new List<Card>(deckBuilderManager.selectedDeck);

            Debug.Log($"[DeckData] ╪╠ец╣х ╣╕ д╚╣Е ╪Ж: {DeckData.selectedDeck.Count}");
            foreach (var c in DeckData.selectedDeck)
                Debug.Log($" - {c.cardName} / {c.GetInstanceID()}");
        }

        if (cardDrawManager != null)
        {
            cardDrawManager.deck = new List<Card>(DeckData.selectedDeck);
            cardDrawManager.ResetDeck();
            cardDrawManager.DrawCard(5);
            StartCoroutine(cardDrawManager.AutoDraw());
        }

        if (deckSettingButton != null)
        {
            deckSettingButton.SetActive(false);
            Debug.Log("О©╫О©╫ О©╫О©╫О©╫О©╫ О©╫О©╫ф╟ О©╫О©╫О©╫О©╫");
        }

        if (manaUI != null)
        {
            manaUI.SetActive(true);
            Debug.Log("О©╫О©╫О©╫О©╫ UI г╔О©╫О©╫");
        }

        if (deckBuilderManager != null)
            deckBuilderManager.HideDeckSettingPanel(false);

        BGMManager.Instance?.PlayBattleMusic();

        if (UIManager.Instance != null)
            UIManager.Instance.CloseUI();

        if (cardUI != null)
            cardUI.SetActive(true);

        if (unitSelectionPanel != null)
            unitSelectionPanel.SetActive(false);

        if (startButton != null)
            startButton.SetActive(false);

        ManaManager.Instance?.StartRegen();

        if (enemySpawner != null)
            enemySpawner.SetActive(true);

        roundManager.StartNextRound();
    }

    public void EnterIntermissionPhase()
    {
        isPlacementPhase = true;
        BGMManager.Instance?.PlayIntermissionMusic();

        if (cardUI != null) cardUI.SetActive(false);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(true);
        if (startButton != null) startButton.SetActive(true);

        if (manaUI != null)
            manaUI.SetActive(false);

        Debug.Log("[StartGameManager] О©╫О©╫О©╫м╧л╪О©╫ О©╫э╟О©╫ О©╫О©╫О©╫О©╫");
    }
}