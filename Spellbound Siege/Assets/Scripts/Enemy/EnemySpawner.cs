using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 생성할 적 프리팹
    public Transform spawnPoint; // 적이 생성될 위치
    public float spawnInterval = 2f; // 적 생성 간격 (초)

    private void Start()
    {
        // 적 생성 코루틴 시작
        StartCoroutine(SpawnEnemies());
    }

    // 적을 일정 간격으로 생성하는 코루틴 함수
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // 적 생성 및 초기화
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemy.GetComponent<EnemyController>().Initialize(GetComponent<PathSystem>().waypoints);
            // 다음 적 생성까지 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}


