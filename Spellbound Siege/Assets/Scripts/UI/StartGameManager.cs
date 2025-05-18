using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    public static bool gameStarted = false;        // 게임이 시작되었는지 여부 (static)
    public static bool isPlacementPhase = true;   // 배치 단계인지 여부

    public GameObject unitSelectionPanel;          // 유닛 선택 UI 패널
    public GameObject startButton;                 // Start 버튼 오브젝트
    public GameObject cardUI;                      // 카드 UI 전체 오브젝트
    public GameObject enemySpawner;                // EnemySpawner 오브젝트
    public Button startButtonComponent;            // Start 버튼 컴포넌트 (UI Button)
    public CardDrawManager cardDrawManager;        // 카드 드로우 매니저
    public RoundManager roundManager;              // 라운드 매니저

    void Start()
    {
        // 씬 시작 시 기본 상태 설정
        gameStarted = false;

        if (cardUI != null) cardUI.SetActive(false);
        if (startButton != null) startButton.SetActive(true);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(true);
        if (enemySpawner != null) enemySpawner.SetActive(false);

        // Start 버튼 클릭 리스너 등록
        if (startButtonComponent != null)
            startButtonComponent.onClick.AddListener(OnStartButtonClicked);
    }

    // Start 버튼을 클릭했을 때 호출됩니다.
    public void OnStartButtonClicked()
    {
        // 게임 시작 플래그 설정 (최초 1회만)
        if (!gameStarted)
        {
            gameStarted = true;
            // (필요한 초기 시작 처리 추가 가능)
        }

        // 배치 단계 종료 → 본격 플레이 모드 진입
        isPlacementPhase = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseUI();
        }

        if (cardUI != null) cardUI.SetActive(true);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(false);
        if (startButton != null) startButton.SetActive(false);

        ManaManager.Instance?.StartRegen(); // 마나 회복 시작

        // ─── 카드 드로우 초기화 및 실행 ─────────────────────────
        // 1) 덱 초기화 및 셔플
        cardDrawManager.ResetDeck();
        // 2) 초기 손패 5장 드로우 (원하는 장수로 변경 가능)
        cardDrawManager.DrawCard(5);
        // 3) 자동 드로우 코루틴 시작 (예: 5초마다 1장)
        StartCoroutine(cardDrawManager.AutoDraw());

        // ─── 필요 시 적 스포너 활성화 ────────────────────────────
        if (enemySpawner != null)
            enemySpawner.SetActive(true);

        // ─── 다음 라운드 시작 ────────────────────────────────────
        roundManager.StartNextRound();
    }

    // 인터미션(배치) 단계로 돌아갈 때 호출합니다.
    public void EnterIntermissionPhase()
    {
        isPlacementPhase = true;

        if (cardUI != null) cardUI.SetActive(false);
        if (unitSelectionPanel != null) unitSelectionPanel.SetActive(true);
        if (startButton != null) startButton.SetActive(true);

        Debug.Log("[StartGameManager] 인터미션 단계 진입");
    }
}