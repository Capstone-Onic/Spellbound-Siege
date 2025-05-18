using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;             // �⺻ ������ (�׽�Ʈ��)
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
    /// ���� ���� ���� �ʿ��� ����ŭ Ǯ ä���
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
    /// Ǯ���� ��Ȱ��ȭ�� ���� ���� ����
    /// </summary>
    public void SpawnEnemy(GameObject prefab)
    {
        GameObject enemy = GetPooledEnemy(prefab);
        if (enemy != null)
        {
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = Quaternion.identity;

            // �ʱ�ȭ �� Ȱ��ȭ
            var controller = enemy.GetComponent<EnemyController>();
            controller.Initialize(pathSystem.waypoints);

            enemy.SetActive(true);

        }
    }

    /// <summary>
    /// ��Ȱ��ȭ�� �� �ϳ� ��ȯ
    /// </summary>
    private GameObject GetPooledEnemy(GameObject prefab)
    {
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy && enemy.name.StartsWith(prefab.name))
                return enemy;
        }

        // Ǯ�� ������ ���� ����
        GameObject newEnemy = Instantiate(prefab);
        newEnemy.SetActive(false);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }
}
