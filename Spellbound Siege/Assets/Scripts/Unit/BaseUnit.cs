using UnityEngine;

// 유닛 타입을 구분하는 열거형
public enum UnitType { Melee, Ranged }

public class BaseUnit : MonoBehaviour
{
    // 유닛 설정 값들 (Inspector에서 조절 가능)
    public UnitType unitType;                   // 유닛 타입 (근거리 / 원거리)
    public float attackCooldown = 1f;           // 공격 간격
    public float damage = 10f;                  // 기본 공격력
    public GameObject projectilePrefab;         // 원거리 유닛일 경우 사용할 투사체 프리팹

    private float nextAttackTime;               // 다음 공격 가능한 시간
    private UnitRangeDetector rangeDetector;    // 범위 내 적 감지 컴포넌트

    // 시작 시 rangeDetector 컴포넌트 찾기
    void Start()
    {
        rangeDetector = GetComponentInChildren<UnitRangeDetector>();
    }

    void Update()
    {
        // 쿨타임이 지났고, 적 감지기가 존재할 때
        if (Time.time >= nextAttackTime && rangeDetector != null)
        {
            EnemyController target = FindClosestTarget(); // 범위 내 가장 가까운 적 찾기

            if (target != null)
            {
                // 근거리 유닛이면 즉시 데미지
                if (unitType == UnitType.Melee)
                {
                    target.TakeDamage(damage);
                }
                // 원거리 유닛이면 투사체 발사
                else
                {
                    FireProjectile(target);
                }

                // 다음 공격 시간 갱신
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    // 범위 내 가장 가까운 적을 찾는 함수
    EnemyController FindClosestTarget()
    {
        EnemyController closest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in rangeDetector.enemiesInRange)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                closest = enemy;
                minDist = dist;
            }
        }

        return closest;
    }

    // 투사체를 생성하여 적을 향해 발사
    void FireProjectile(EnemyController target)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Initialize(target.transform.position, damage);
    }
}
