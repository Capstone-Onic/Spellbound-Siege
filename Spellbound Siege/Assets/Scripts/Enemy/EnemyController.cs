// 개별 적의 이동과 상태를 담당하는 클래스
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f; // 기본 이동 속도
    public float health = 100f; // 적의 최대 체력
    public float speedMultiplier = 1f; // 상태 효과로 인한 속도 변화 (예: 감속, 가속)
    public GameObject deathEffectPrefab;

    private List<Transform> waypoints; // 경로 정보
    private int waypointIndex = 0; // 현재 목표로 하는 웨이포인트 인덱스
    public enum EnemyType
    {
        Normal,
        Elite,
        Boss
    }
    private void Start()
    {
        switch (enemyType)
        {
            case EnemyType.Normal:
                rewardGold = 10;
                break;
            case EnemyType.Elite:
                rewardGold = 25;
                break;
            case EnemyType.Boss:
                rewardGold = 100;
                break;
        }
    }

    public EnemyType enemyType = EnemyType.Normal; // 인스펙터에서 선택 가능
    public int rewardGold = 10;
    public void Initialize(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
        transform.position = waypoints[0].position; // 경로의 시작 지점으로 이동
    }

    private void Update()
    {
        if (waypoints == null || waypointIndex >= waypoints.Count) return;

        // 현재 목표 웨이포인트로 이동하기
        Transform targetPoint = waypoints[waypointIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        float adjustedSpeed = speed * speedMultiplier; // 상태 효과 적용된 속도 계산
        transform.position += direction * adjustedSpeed * Time.deltaTime;

        // 목표 지점에 도달했는지 확인
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Count)
            {
                OnDeath(false); // 목적지 도달 시 적 제거 (처치하지 않은 경우)
            }
        }
    }

    // 적이 피해를 입는 함수
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            OnDeath(true); // 적이 죽었을 경우 처리
        }
    }

    // 적이 제거될 때 처리하는 함수
    private void OnDeath(bool killedByPlayer)
    {
        if (killedByPlayer)
        {
            if (GoldManager.instance != null)
            {
                GoldManager.instance.AddGold(rewardGold);
            }
        }

        //이펙트 생성
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}