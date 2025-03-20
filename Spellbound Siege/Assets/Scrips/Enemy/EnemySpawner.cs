using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // ������ �� ������
    public Transform spawnPoint; // ���� ������ ��ġ
    public float spawnInterval = 2f; // �� ���� ���� (��)

    private void Start()
    {
        // �� ���� �ڷ�ƾ ����
        StartCoroutine(SpawnEnemies());
    }

    // ���� ���� �������� �����ϴ� �ڷ�ƾ �Լ�
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // �� ���� �� �ʱ�ȭ
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemy.GetComponent<EnemyController>().Initialize(GetComponent<PathSystem>().waypoints);
            // ���� �� �������� ���
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}


