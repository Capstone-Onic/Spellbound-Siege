using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    public static bool gameStarted = false;        // ������ ���۵Ǿ����� ���� (static)
    public static bool isPlacementPhase = true;   // ��ġ �ܰ����� ����

    public GameObject unitSelectionPanel;          // ���� ���� UI �г�
    public GameObject startButton;                 // Start ��ư ������Ʈ
    public GameObject cardUI;                      // ī�� UI ��ü ������Ʈ
    public GameObject enemySpawner;                // EnemySpawner ������Ʈ
    public Button startButtonComponent;            // Start ��ư ������Ʈ (UI Button)
    public CardDrawManager cardDrawManager;        // ī�� ��ο� �Ŵ���
    public RoundManager roundManager;              // ���� �Ŵ���

    void Start()
    {
        // �� ���� �� �⺻ ���� ����
        gameStarted = false;

        if (cardUI != null) cardUI.SetActive(false);
        if (startButton != null) startButton.SetActive(true);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(true);
        if (enemySpawner != null) enemySpawner.SetActive(false);

        // Start ��ư Ŭ�� ������ ���
        if (startButtonComponent != null)
            startButtonComponent.onClick.AddListener(OnStartButtonClicked);
    }

    // Start ��ư�� Ŭ������ �� ȣ��˴ϴ�.
    public void OnStartButtonClicked()
    {
        // ���� ���� �÷��� ���� (���� 1ȸ��)
        if (!gameStarted)
        {
            gameStarted = true;
            // (�ʿ��� �ʱ� ���� ó�� �߰� ����)
        }

        // ��ġ �ܰ� ���� �� ���� �÷��� ��� ����
        isPlacementPhase = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseUI();
        }

        if (cardUI != null) cardUI.SetActive(true);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(false);
        if (startButton != null) startButton.SetActive(false);

        ManaManager.Instance?.StartRegen(); // ���� ȸ�� ����

        // ������ ī�� ��ο� �ʱ�ȭ �� ���� ��������������������������������������������������
        // 1) �� �ʱ�ȭ �� ����
        cardDrawManager.ResetDeck();
        // 2) �ʱ� ���� 5�� ��ο� (���ϴ� ����� ���� ����)
        cardDrawManager.DrawCard(5);
        // 3) �ڵ� ��ο� �ڷ�ƾ ���� (��: 5�ʸ��� 1��)
        StartCoroutine(cardDrawManager.AutoDraw());

        // ������ �ʿ� �� �� ������ Ȱ��ȭ ��������������������������������������������������������
        if (enemySpawner != null)
            enemySpawner.SetActive(true);

        // ������ ���� ���� ���� ������������������������������������������������������������������������
        roundManager.StartNextRound();
    }

    // ���͹̼�(��ġ) �ܰ�� ���ư� �� ȣ���մϴ�.
    public void EnterIntermissionPhase()
    {
        isPlacementPhase = true;

        if (cardUI != null) cardUI.SetActive(false);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(true);
        if (startButton != null) startButton.SetActive(true);

        Debug.Log("[StartGameManager] ���͹̼� �ܰ� ����");
    }
}