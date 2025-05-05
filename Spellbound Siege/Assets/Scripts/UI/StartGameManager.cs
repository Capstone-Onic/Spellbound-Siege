using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    public static bool gameStarted = false;  // 외부에서 접근 가능하도록 static

    public GameObject unitSelectionPanel;   // 유닛 선택 UI
    public GameObject startButton;          // 시작 버튼
    public GameObject cardUI;               // 카드 UI 전체
    public GameObject enemySpawner;         // 적 스폰 컨트롤러
    public Button startButtonComponent;     // 연결 버튼 (선택적)
    public CardDrawManager cardDrawManager; // 카드 드로우 매니저

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

        // 카드 드로우 시작
        if (cardDrawManager != null)
        {
            cardDrawManager.ResetDeck();
            cardDrawManager.DrawCard(5);
            cardDrawManager.StartCoroutine(cardDrawManager.AutoDraw()); // 원할 경우 자동 드로우
        }
    }
}