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

    void Start()
    {
        gameStarted = false;

        cardUI.SetActive(false);
        startButton.SetActive(true);
        unitSelectionPanel.SetActive(true);
        enemySpawner.SetActive(false);

        // 코드로 onClick 연결해도 됨 (선택사항)
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
    }
}
