using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;             // 기본 프리팹 (테스트용)
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public RoundManager roundManager;

    private List<GameObject> enemyPool = new List<GameObject>();
    private PathSystem pathSystem;

    private void Awake()
    {
        pathSystem = GetComponent<PathSystem>();
    }

    /// <summary>
    /// 라운드 시작 전에 필요한 수만큼 풀 채우기
    /// </summary>
    public void PreparePool(GameObject prefab, int requiredCount)
    {
        while (enemyPool.Count < requiredCount)
        {
            GameObject enemy = Instantiate(prefab);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    /// <summary>
    /// 풀에서 비활성화된 적을 꺼내 스폰
    /// </summary>
    public void SpawnEnemy(GameObject prefab)
    {
        GameObject enemy = GetPooledEnemy(prefab);
        if (enemy != null)
        {
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = Quaternion.identity;

            // 초기화 후 활성화
            var controller = enemy.GetComponent<EnemyController>();
            controller.Initialize(pathSystem.waypoints);

            enemy.SetActive(true);

        }
    }

    /// <summary>
    /// 비활성화된 적 하나 반환
    /// </summary>
    private GameObject GetPooledEnemy(GameObject prefab)
    {
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy && enemy.name.StartsWith(prefab.name))
                return enemy;
        }

        // 풀에 없으면 새로 생성
        GameObject newEnemy = Instantiate(prefab);
        newEnemy.SetActive(false);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }
}
