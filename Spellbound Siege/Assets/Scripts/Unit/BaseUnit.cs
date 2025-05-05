using UnityEngine;

// 유닛 타입 정의
public enum UnitType { knight, mage }

public class BaseUnit : MonoBehaviour
{
    public UnitType unitType;                   // 유닛 타입 설정
    public float attackCooldown = 1f;           // 공격 쿨타임
    public float damage = 10f;                  // 공격력
    public GameObject projectilePrefab;         // 원거리용 투사체 프리팹

    private float nextAttackTime;               // 다음 공격 가능 시간
    private UnitRangeDetector rangeDetector;    // 적 감지기
    private Animator animator;                  // 애니메이터

    void Start()
    {
        // 시작 시 유닛 X축 180도 회전
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        rangeDetector = GetComponentInChildren<UnitRangeDetector>();
        animator = GetComponent<Animator>(); // Animator 연결
    }

    void Update()
    {
        if (Time.time >= nextAttackTime && rangeDetector != null)
        {
            EnemyController target = FindClosestTarget();

            if (target != null)
            {
                // ✅ 적을 바라보도록 회전
                Vector3 direction = (target.transform.position - transform.position).normalized;
                direction.y = 0f;
                if (direction != Vector3.zero)
                {
                    transform.forward = direction;
                }

                // ✅ 애니메이션 실행 (Attack 트리거)
                if (animator != null)
                {
                    if (unitType == UnitType.knight)
                    {
                        animator.SetTrigger("Attack_knight");
                        target.TakeDamage(damage);
                    }
                    else if(unitType == UnitType.mage)
                    {
                        animator.SetTrigger("Attack_mage");
                        FireProjectile(target);
                    }
                }
                // ✅ 공격 처리
                if (unitType == UnitType.knight)
                {
                    target.TakeDamage(damage);
                }
                else if(unitType == UnitType.mage)
                {
                    FireProjectile(target);
                }

                // ✅ 쿨타임 갱신
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    // 가장 가까운 적을 탐색
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

    // 투사체 발사
    void FireProjectile(EnemyController target)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Initialize(target.transform.position, damage);
    }
}