using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class RoundManager : MonoBehaviour
{
    [System.Serializable]
    public class RoundData
    {
        public GameObject enemyPrefab;
        public int enemyCount;
    }

    [Header("Round")]
    public List<RoundData> rounds = new();
    public float spawnInterval = 1.5f;
    public EnemySpawner spawner;
    public StartGameManager startGameManager;

    public UnityEvent<int> onRoundStarted;
    public UnityEvent<int> onRoundEnded;

    [Header("UI")]
    public TMP_Text roundText;
    public TMP_Text enemyCountText;

    [Header("Life System")]
    public int maxLife = 10;
    public int lifeCount = 5;
    public List<Image> lifeIcons = new();

    private int currentRound = 0;
    private bool isRunning = false;
    private bool gameOver = false;

    private int totalEnemiesThisRound = 0;
    private int killedEnemiesThisRound = 0;

    private void Start()
    {
        ResetLifeUI(); // 게임 시작 시 하트 리셋
        UpdateLifeUI(); // UI 동기화
    }

    public void StartNextRound()
    {
        if (isRunning || currentRound >= rounds.Count) return;

        RoundData round = rounds[currentRound];
        currentRound++;

        killedEnemiesThisRound = 0;
        totalEnemiesThisRound = round.enemyCount;

        UpdateRoundUI();
        UpdateEnemyUI();

        spawner.PreparePool(round.enemyPrefab, round.enemyCount);
        StartCoroutine(SpawnEnemies(round));
    }

    private IEnumerator SpawnEnemies(RoundData round)
    {
        isRunning = true;
        onRoundStarted?.Invoke(currentRound);

        for (int i = 0; i < round.enemyCount; i++)
        {
            spawner.SpawnEnemy(round.enemyPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }

        UpdateEnemyUI();
    }

    public void NotifyEnemyKilled()
    {
        killedEnemiesThisRound++;
        UpdateEnemyUI();

        if (isRunning && killedEnemiesThisRound >= totalEnemiesThisRound)
        {
            Debug.Log("[RoundManager] 라운드 종료: 모든 적 처치됨");
            isRunning = false;
            onRoundEnded?.Invoke(currentRound);

            if (startGameManager != null)
            {
                startGameManager.EnterIntermissionPhase();
            }
        }
    }

    public void DecreaseLife()
    {
        if (gameOver || lifeCount <= 0) return;

        lifeCount = Mathf.Max(0, lifeCount - 1);
        UpdateLifeUI();

        if (lifeCount < lifeIcons.Count && lifeIcons[lifeCount] != null)
        {
            StartCoroutine(AnimateLifeLoss(lifeIcons[lifeCount]));
        }

        if (lifeCount <= 0)
        {
            gameOver = true;
            HandleGameOver();
        }
    }

    private IEnumerator AnimateLifeLoss(Image heart)
    {
        float duration = 0.3f;
        float time = 0;

        Vector3 originalScale = heart.transform.localScale;
        Color originalColor = heart.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            heart.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            heart.color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 0), t);

            yield return null;
        }

        heart.gameObject.SetActive(false);
    }

    private void HandleGameOver()
    {
        Debug.Log("[RoundManager] Game Over triggered!");
        Time.timeScale = 0;

        // TODO: Game Over UI 열기 등 추가 가능
    }

    private void ResetLifeUI()
    {
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            if (lifeIcons[i] == null) continue;

            lifeIcons[i].gameObject.SetActive(i < lifeCount); // 5개만 표시
            lifeIcons[i].transform.localScale = Vector3.one;
            lifeIcons[i].color = Color.white;
        }
    }

    private void UpdateRoundUI()
    {
        if (roundText != null)
        {
            roundText.text = $"Round: {currentRound}";
        }
    }

    private void UpdateEnemyUI()
    {
        if (enemyCountText != null)
        {
            int remaining = Mathf.Max(0, totalEnemiesThisRound - killedEnemiesThisRound);
            enemyCountText.text = $"Enemies: {remaining} / {totalEnemiesThisRound}";
        }
    }

    private void UpdateLifeUI()
    {
        // 필요 시 숫자 UI와 함께 사용할 수 있도록 확장 가능
    }
}
