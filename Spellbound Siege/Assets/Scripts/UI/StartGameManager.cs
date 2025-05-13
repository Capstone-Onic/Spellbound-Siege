using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    public static bool gameStarted = false;  // 외부에서 접근 가능하도록 static
    public static bool isPlacementPhase = false;
 
    public GameObject unitSelectionPanel;   // 유닛 선택 UI
    public GameObject startButton;          // 시작 버튼
    public GameObject cardUI;               // 카드 UI 전체
    public GameObject enemySpawner;         // 적 스폰 컨트롤러
    public Button startButtonComponent;     // 연결 버튼 (선택적)
    public CardDrawManager cardDrawManager; // 카드 드로우 매니저
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
            // 초기 시작 처리
        }

        isPlacementPhase = false; // 배치 시간 종료
        cardUI.SetActive(true);
        unitSelectionPanel.SetActive(false);
        startButton.SetActive(false);

        roundManager.StartNextRound();
    }
    public void EnterIntermissionPhase()
    {
        isPlacementPhase = true;
        // 라운드 종료 후: 카드 닫고, 유닛 패널 & 버튼 열기
        if (cardUI != null) cardUI.SetActive(false);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(true);
        if (startButton != null) startButton.SetActive(true);

        Debug.Log("[StartGameManager] 라운드 종료 후 인터미션 단계로 전환됨");
    }
}