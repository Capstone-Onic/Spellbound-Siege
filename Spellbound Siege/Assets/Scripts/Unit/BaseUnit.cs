using UnityEngine;

// 유닛 타입을 구분하는 열거형
public enum UnitType { Melee, Ranged }

public class BaseUnit : MonoBehaviour
{
    public UnitType unitType;                   
    public float attackCooldown = 1f;           
    public float damage = 10f;                  
    public GameObject projectilePrefab;         

    private float nextAttackTime;               
    private UnitRangeDetector rangeDetector;    
    private Animator animator;                  

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
