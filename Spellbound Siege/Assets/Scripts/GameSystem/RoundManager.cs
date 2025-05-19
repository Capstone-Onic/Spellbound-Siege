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
        public GameObject enemyPrefabA;
        public int enemyCountA;
        public GameObject enemyPrefabB;
        public int enemyCountB;
    }

    [Header("Round")]
    public List<RoundData> rounds = new();
    public float spawnInterval = 1.5f;
    public EnemySpawner spawnerA;
    public EnemySpawner spawnerB;
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
        ResetLifeUI();
        UpdateLifeUI();
    }

    public void StartNextRound()
    {
        if (isRunning || currentRound >= rounds.Count) return;

        RoundData round = rounds[currentRound];
        currentRound++;

        killedEnemiesThisRound = 0;
        totalEnemiesThisRound = round.enemyCountA + round.enemyCountB;

        UpdateRoundUI();
        UpdateEnemyUI();

        spawnerA.PreparePool(round.enemyPrefabA, round.enemyCountA);
        spawnerB.PreparePool(round.enemyPrefabB, round.enemyCountB);

        ManaManager.Instance.currentMana = 0;
        ManaManager.Instance.UpdateManaUI();
        ManaManager.Instance.StartRegen();

        StartCoroutine(SpawnEnemiesDual(round));
    }

    private IEnumerator SpawnEnemiesDual(RoundData round)
    {
        isRunning = true;
        onRoundStarted?.Invoke(currentRound);

        int maxCount = Mathf.Max(round.enemyCountA, round.enemyCountB);
        for (int i = 0; i < maxCount; i++)
        {
            if (i < round.enemyCountA)
                spawnerA.SpawnEnemy(round.enemyPrefabA);

            if (i < round.enemyCountB)
                spawnerB.SpawnEnemy(round.enemyPrefabB);

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
            Debug.Log("[RoundManager] ���� ����: ��� �� óġ��");
            isRunning = false;
            onRoundEnded?.Invoke(currentRound);

            ManaManager.Instance?.StopRegen();

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
        // Game Over UI ���� �� �߰� ����
    }

    private void ResetLifeUI()
    {
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            if (lifeIcons[i] == null) continue;

            lifeIcons[i].gameObject.SetActive(i < lifeCount);
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
        // �ʿ� �� ���� UI�� �Բ� ����� �� �ֵ��� Ȯ�� ����
    }
