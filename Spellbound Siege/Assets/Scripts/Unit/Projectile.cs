using UnityEngine;

public class Projectile : MonoBehaviour
{
    // ========== [투사체 관련 설정값] ==========

    public float speed = 10f;         // 투사체가 목표 지점까지 이동하는 속도 (단위: 초당 유닛)
    public float aoeRadius = 2f;      // 범위 공격일 경우, 폭발 반경 (단위: 거리)
    public float launchDelay = 0.2f;  // 투사체 발사까지의 지연 시간 (애니메이션과의 싱크 맞춤용)

    // ========== [내부 변수: 초기화 시 설정됨] ==========

    private Vector3 targetPos;        // 투사체의 최종 도달 지점 (타겟 위치)
    private float damage;             // 투사체가 가하는 피해량
    private bool isAreaDamage = true; // 범위 피해를 입히는 투사체인지 여부

    // 거리 계산 최적화를 위한 제곱 값 (루트 계산 생략)
    private float aoeRadiusSqr;            // aoeRadius의 제곱값
    private float explodeThresholdSqr = 0.01f; // 목표 도달 판단 기준 거리의 제곱값

    private bool launched = false;         // 발사가 되었는지 여부 (딜레이 이후 true로 전환)

    // ========================================
    // 투사체의 외부 초기화 함수
    // ========================================
    public void Initialize(Vector3 targetPosition, float dmg, bool areaDamage)
    {
        targetPos = targetPosition;
        damage = dmg;
        isAreaDamage = areaDamage;
        aoeRadiusSqr = aoeRadius * aoeRadius;

        // ?? 투사체를 목표 방향으로 회전
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        Invoke(nameof(Launch), launchDelay);
    }

    // ========================================
    // 실제 투사체 발사를 시작하는 함수 (딜레이 후 호출됨)
    // ========================================
    private void Launch()
    {
        launched = true;
    }

    // ========================================
    // 매 프레임마다 실행되는 업데이트 함수
    // ========================================
    void Update()
    {
        // 아직 발사가 안 되었으면 아무것도 하지 않음
        if (!launched) return;

        // 목표 지점을 향해 일정 속도로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // 목표 지점에 거의 도달했는지 확인 (제곱 거리로 비교)
        if ((transform.position - targetPos).sqrMagnitude < explodeThresholdSqr)
        {
            Explode(); // 도달했으면 폭발 처리
        }
    }

    // ========================================
    // 폭발 처리 함수 (단일 또는 범위 피해 처리 후 파괴)
    // ========================================
    void Explode()
    {
        if (isAreaDamage)
        {
            // 범위 피해: 폭발 지점을 중심으로 aoeRadius 거리 내 모든 적을 탐지
            Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
            foreach (Collider hit in hits)
            {
                // 적 태그가 붙은 오브젝트만 필터링
                if (hit.CompareTag("Enemy"))
                {
                    EnemyController enemy = hit.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage); // 피해 입힘
                    }
                }
            }
        }
        else
        {
            // 단일 피해: 아주 작은 반경(0.2f) 내에서 적 탐색 (직격)
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.2f);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    EnemyController enemy = hit.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage); // 피해 입히고
                        break; // 하나만 맞히면 종료
                    }
                }
            }
        }

        // 폭발 이펙트/애니메이션을 따로 처리하지 않는 경우, 투사체 오브젝트 삭제
        Destroy(gameObject);
    }
}
