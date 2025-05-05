using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public int poolSize = 20;

    private List<GameObject> enemyPool = new List<GameObject>();
    private PathSystem pathSystem;

    private void Start()
    {
        pathSystem = GetComponent<PathSystem>();
        InitializePool();
        StartCoroutine(SpawnEnemies());
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    private GameObject GetPooledEnemy()
    {
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
                return enemy;
        }

        // 풀에 여유가 없다면 새로 생성해도 되지만, 기본은 풀 사이즈 내에서 재사용
        return null;
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            GameObject enemy = GetPooledEnemy();

            if (enemy != null)
            {
                enemy.transform.position = spawnPoint.position;
                enemy.transform.rotation = Quaternion.identity;
                enemy.SetActive(true);

                var controller = enemy.GetComponent<EnemyController>();
                controller.Initialize(pathSystem.waypoints);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
