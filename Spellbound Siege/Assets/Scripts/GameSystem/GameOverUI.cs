using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // 혹시 슬로우모션 중일 경우 복원
        SceneManager.LoadScene("MainMenuScene"); // 메인화면 씬 이름
    }
}

