using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;         // 투사체 속도
    public float aoeRadius = 2f;      // 폭발 범위 반경
    private Vector3 targetPos;        // 목표 지점
    private float damage;             // 피해량

    private float aoeRadiusSqr;       // 폭발 범위 거리 제곱값 (최적화용)
    private float explodeThresholdSqr = 0.01f; // 목표 도달 판정용 거리 제곱값

    // 투사체 초기 설정
    public void Initialize(Vector3 targetPosition, float dmg)
    {
        targetPos = targetPosition;
        damage = dmg;
        aoeRadiusSqr = aoeRadius * aoeRadius;
    }

    void Update()
    {
        // 목표 방향으로 직선 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // 목표 지점에 도달했는지 확인 (정확한 위치 비교 대신 거리 제곱값 비교)
        if ((transform.position - targetPos).sqrMagnitude < explodeThresholdSqr)
        {
            Explode();
        }
    }

    // 폭발 처리: 주변 적에게 범위 피해
    void Explode()
    {
        // 폭발 지점 기준으로 구체 범위 내 콜라이더 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (Collider hit in hits)
        {
            // 적 태그 확인 후 데미지 적용
            if (hit.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        // 투사체 제거
        Destroy(gameObject);
    }
}
