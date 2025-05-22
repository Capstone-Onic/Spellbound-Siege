using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛의 타입을 구분하는 열거형 (각 타입마다 공격 방식이 다름)
/// </summary>
public enum UnitType { knight, mage, druid, archer }

/// <summary>
/// 각 유닛들의 공통 동작 및 공격을 담당하는 메인 클래스
/// </summary>
public class BaseUnit : MonoBehaviour, IStunnable
{
    // ========== [기본 유닛 설정값] ==========

    public UnitType unitType;              // 유닛의 타입 설정 (기사, 마법사 등)
    public float attackCooldown = 1f;      // 공격 쿨타임
    public float damage = 10f;             // 공격력
    public GameObject projectilePrefab;    // 공격용 투사체 또는 이펙트 프리팹 (기사도 이걸 사용)

    // ========== [내부 상태값] ==========
    private float nextAttackTime;                  // 다음 공격 가능 시간
    private UnitRangeDetector rangeDetector;       // 범위 내 적 탐지기
    private Animator animator;                     // 애니메이터 컴포넌트

    private bool isStunned = false;                // 기절 상태 여부
    private float stunEndTime = 0f;                // 기절 종료 시간
    private GameObject stunIndicatorInstance;      // 기절 상태 이펙트 인스턴스

    // ========== [기절 시각 효과 프리팹] ==========
    [Header("Stun Visual Effect")]
    public GameObject stunIndicatorPrefab;         // 기절 시 유닛 위에 생성할 이펙트 프리팹

    // ========== [UI / 유닛 정보] ==========
    public string unitName;        // 유닛 이름
    public Sprite unitIcon;        // 유닛 아이콘
    public int goldCost;           // 유닛 소환 비용

    // ========== [드루이드 전용 설정] ==========
    [Header("Druid Area Effect Settings")]
    public float druidDuration = 3f;           // 드루이드 효과 지속 시간
    public float druidTickInterval = 1f;       // 틱 간격 (몇 초마다 데미지를 주는지)
    public float druidEffectRadius = 1.5f;     // 드루이드 효과 범위 반경

    private List<GameObject> activeUpgradeEffects = new(); // 강화 시 등장하는 이펙트

    void Start()
    {
        // 유닛이 바라보는 방향 기본값 설정 (뒤를 보게)
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // Y 위치를 고정 (부유하거나 묻히지 않게)
        Vector3 fixedPosition = transform.position;
        fixedPosition.y = 0.05f;
        transform.position = fixedPosition;

        // 범위 감지기 및 애니메이터 초기화
        rangeDetector = GetComponentInChildren<UnitRangeDetector>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 기절 상태인 경우 행동 중단
        if (isStunned)
        {
            if (Time.time >= stunEndTime)
            {
                isStunned = false;

                // 기절 이펙트 제거
                if (stunIndicatorInstance != null)
                {
                    Destroy(stunIndicatorInstance);
                }

                if (animator != null)
                    animator.SetBool("isWalking", true);
            }
            return;
        }

        // 공격 쿨타임이 지나고 감지기가 존재하면 공격 가능
        if (Time.time >= nextAttackTime && rangeDetector != null)
        {
            EnemyController target = FindClosestTarget(); // 가장 가까운 적 찾기

            if (target != null && !target.IsDead) // 죽은 적은 무시
            {
                // 유닛이 적을 바라보게 회전
                Vector3 direction = (target.transform.position - transform.position).normalized;
                direction.y = 0f;
                if (direction != Vector3.zero)
                    transform.forward = direction;

                // 유닛 타입에 따라 애니메이션 트리거 실행
                if (animator != null)
                {
                    if (unitType == UnitType.knight)
                        animator.SetTrigger("Attack_knight");
                    else if (unitType == UnitType.mage)
                        animator.SetTrigger("Attack_mage");
                    else if (unitType == UnitType.druid)
                        animator.SetTrigger("Attack_druid");
                    else if (unitType == UnitType.archer)
                        animator.SetTrigger("Attack_archer");
                }

                // 유닛 타입별 공격 실행
                switch (unitType)
                {
                    case UnitType.knight:
                        StartCoroutine(KnightAttackCoroutine()); // 기사 전용 근접 반원 공격
                        break;
                    case UnitType.mage:
                        FireProjectile(target, areaDamage: true); // 마법사는 광역 투사체
                        break;
                    case UnitType.druid:
                        SpawnDruidArea(target); // 드루이드는 바닥에 장판
                        break;
                    case UnitType.archer:
                        FireProjectile(target, areaDamage: false); // 궁수는 단일 타겟 투사체
                        break;
                }

                nextAttackTime = Time.time + attackCooldown; // 다음 공격 시간 갱신
            }
        }
    }

    /// <summary>
    /// 기절 효과 적용 함수 (보스에게 당했을 때 호출됨)
    /// </summary>
    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;

        // 기절 이펙트 표시
        if (stunIndicatorPrefab != null && stunIndicatorInstance == null)
        {
            Vector3 indicatorPos = transform.position + Vector3.up * 2f;
            stunIndicatorInstance = Instantiate(stunIndicatorPrefab, indicatorPos, Quaternion.identity, transform);
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetTrigger("Stunned"); // Animator에 해당 트리거가 있어야 함
        }

        Debug.Log($"{gameObject.name} 유닛이 {duration}초 동안 기절함.");
    }

    /// <summary>
    /// 가장 가까운 적을 찾아 반환
    /// </summary>
    EnemyController FindClosestTarget()
    {
        EnemyController closest = null;
        float minDist = float.MaxValue;

        // 리스트에서 죽었거나 비활성화된 적 제거
        rangeDetector.enemiesInRange.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy || e.IsDead);

        foreach (var enemy in rangeDetector.enemiesInRange)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                closest = enemy;
                minDist = dist;
            }
        }

        return closest;
    }

    /// <summary>
    /// 투사체 발사 처리 (마법사 / 궁수용)
    /// </summary>
    void FireProjectile(EnemyController target, bool areaDamage)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Initialize(target.transform.position, damage, areaDamage);
    }

    // ========== [강화 관련 설정] ==========
    private int upgradeLevel = 0; // 0 = 기본, 1 = 1단계, 2 = 2단계, 3 = 최종

    [Header("Upgrade Visual Prefabs")]
    public GameObject[] upgradeEffectPrefabs; // [0]=1단계, [1]=2단계, [2]=3단계 프리팹들

    /// <summary>
    /// 유닛 강화 기능 (최대 3단계까지 강화, 시각적 프리팹 추가 포함)
    /// </summary>
    public void IncreaseStats()
    {
        if (upgradeLevel >= 2)
        {
            Debug.Log("최대 강화에 도달했습니다.");
            return;
        }

        upgradeLevel++;

        switch (upgradeLevel)
        {
            case 1:
            case 2:
                damage *= 1.5f;
                break;
        }

        Debug.Log($"유닛이 {upgradeLevel}단계로 강화되었습니다. 현재 공격력: {damage}, 쿨타임: {attackCooldown}");

        // 프리팹 인덱스는 0부터 시작
        int prefabIndex = upgradeLevel - 1;
        if (upgradeEffectPrefabs != null && prefabIndex < upgradeEffectPrefabs.Length)
        {
            GameObject effect = upgradeEffectPrefabs[prefabIndex];
            if (effect != null)
            {
                Vector3 effectPos = transform.position;
                effectPos.y = 0.05f; // 바닥에 살짝 띄우기
                GameObject instance = Instantiate(effect, effectPos, Quaternion.identity);
                activeUpgradeEffects.Add(instance); // 리스트에 저장
            }
        }
    }
    public void CleanupUpgradeEffects()
    {
        foreach (var fx in activeUpgradeEffects)
        {
            if (fx != null)
                Destroy(fx);
        }
        activeUpgradeEffects.Clear();
    }

    /// <summary>
    /// 드루이드 장판 공격 시작
    /// </summary>
    void SpawnDruidArea(EnemyController target)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = target.transform.position;
        spawnPos.y += 0.1f;

        GameObject field = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        StartCoroutine(DruidEffectCoroutine(field, spawnPos));
    }

    /// <summary>
    /// 드루이드 장판 효과 지속 시간 동안 주기적 데미지
    /// </summary>
    private IEnumerator DruidEffectCoroutine(GameObject field, Vector3 position)
    {
        float elapsed = 0f;

        while (elapsed < druidDuration)
        {
            Collider[] hits = Physics.OverlapSphere(position, druidEffectRadius);
            foreach (var hit in hits)
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.IsDead)
                {
                    enemy.TakeDamage(damage);
                }
            }

            yield return new WaitForSeconds(druidTickInterval);
            elapsed += druidTickInterval;
        }

        if (field != null)
        {
            Destroy(field);
        }
    }

    /// <summary>
    /// 기사 전용 공격: 이펙트를 생성하고 회전시킨 뒤 실제 데미지 처리
    /// </summary>
    private IEnumerator KnightAttackCoroutine()
    {
        yield return new WaitForSeconds(1f); // 애니메이션 타이밍 맞춤

        if (projectilePrefab != null)
        {
            // 유닛 앞쪽에서 프리팹 생성
            Vector3 spawnPos = transform.position + transform.forward * 1f;
            GameObject slash = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(transform.forward));

            // 프리팹을 반원 경로로 회전시키는 코루틴 실행
            StartCoroutine(SlashArcEffect(slash, 2f, 120f, 0.3f));

            // 시각적 효과는 일정 시간 뒤 제거
            Destroy(slash, 0.5f);
        }

        FireMeleeArc(); // 실제 데미지 처리
    }

    /// <summary>
    /// 프리팹을 반원 경로로 회전시키며 이동시키는 시각 효과
    /// </summary>
    private IEnumerator SlashArcEffect(GameObject effect, float radius, float angle, float duration)
    {
        float elapsed = 0f;
        float startAngle = -angle / 2f;
        float endAngle = angle / 2f;

        Vector3 center = transform.position; // 회전 중심

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * radius;

            // 위치와 회전 모두 캐릭터 방향 기준으로 적용
            effect.transform.position = center + transform.rotation * offset;
            effect.transform.rotation = Quaternion.LookRotation(transform.rotation * offset);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 반원 범위 내 적에게 실제 데미지를 주는 함수
    /// </summary>
    private void FireMeleeArc()
    {
        float attackRadius = 2f; // 범위 반지름
        float arcAngle = 120f;     // 전방 반원 각도

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.IsDead)
            {
                Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, toEnemy);

                if (angle <= arcAngle / 2f)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }
}

