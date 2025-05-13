using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    public static bool gameStarted = false;  // �ܺο��� ���� �����ϵ��� static
    public static bool isPlacementPhase = false;
 
    public GameObject unitSelectionPanel;   // ���� ���� UI
    public GameObject startButton;          // ���� ��ư
    public GameObject cardUI;               // ī�� UI ��ü
    public GameObject enemySpawner;         // �� ���� ��Ʈ�ѷ�
    public Button startButtonComponent;     // ���� ��ư (������)
    public CardDrawManager cardDrawManager; // ī�� ��ο� �Ŵ���
    public RoundManager roundManager;

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
        if (!gameStarted)
        {
            gameStarted = true;
            // �ʱ� ���� ó��
        }

        isPlacementPhase = false; // ��ġ �ð� ����
        cardUI.SetActive(true);
        unitSelectionPanel.SetActive(false);
        startButton.SetActive(false);

        roundManager.StartNextRound();
    }
    public void EnterIntermissionPhase()
    {
        isPlacementPhase = true;
        // ���� ���� ��: ī�� �ݰ�, ���� �г� & ��ư ����
        if (cardUI != null) cardUI.SetActive(false);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(true);
        if (startButton != null) startButton.SetActive(true);

        Debug.Log("[StartGameManager] ���� ���� �� ���͹̼� �ܰ�� ��ȯ��");
    }
}