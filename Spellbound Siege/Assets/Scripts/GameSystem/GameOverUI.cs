using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Ȥ�� ���ο��� ���� ��� ����
        SceneManager.LoadScene("MainMenuScene"); // ����ȭ�� �� �̸�
    }
}

