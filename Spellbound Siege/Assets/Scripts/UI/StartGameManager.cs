using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    public static bool gameStarted = false;  // �ܺο��� ���� �����ϵ��� static

    public GameObject unitSelectionPanel;   // ���� ���� UI
    public GameObject startButton;          // ���� ��ư
    public GameObject cardUI;               // ī�� UI ��ü
    public GameObject enemySpawner;         // �� ���� ��Ʈ�ѷ�
    public Button startButtonComponent;     // ���� ��ư (������)
    public CardDrawManager cardDrawManager; // ī�� ��ο� �Ŵ���

    void Start()
    {
        gameStarted = false;

        cardUI.SetActive(false);
        startButton.SetActive(true);
        unitSelectionPanel.SetActive(true);
        enemySpawner.SetActive(false);

        if (startButtonComponent != null)
        {
            startButtonComponent.onClick.AddListener(OnStartButtonClicked);
        }
    }

    public void OnStartButtonClicked()
    {
        if (gameStarted) return;

        gameStarted = true;

        startButton.SetActive(false);
        unitSelectionPanel.SetActive(false);
        cardUI.SetActive(true);
        enemySpawner.SetActive(true);

        // ī�� ��ο� ����
        if (cardDrawManager != null)
        {
            cardDrawManager.ResetDeck();
            cardDrawManager.DrawCard(5);
            cardDrawManager.StartCoroutine(cardDrawManager.AutoDraw()); // ���� ��� �ڵ� ��ο�
        }
    }
}