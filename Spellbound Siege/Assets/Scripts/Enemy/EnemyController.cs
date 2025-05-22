using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 유닛의 이동, 체력 관리, 사망 처리, 특수 능력 등을 담당하는 스크립트
/// </summary>
public class EnemyController : MonoBehaviour
{
    // ===== [일반 설정] =====
    public float speed = 3f;                            // 이동 속도
    public float maxHealth = 100f;                      // 최대 체력
    public float health;                               // 현재 체력
    public float speedMultiplier = 1f;                  // 이동 속도 배수 (슬로우 등 상태이상 대응)
    public GameObject deathEffectPrefab;                // 사망 시 이펙트 프리팹

    private List<Transform> waypoints;                  // 이동 경로
    private int waypointIndex = 0;                      // 현재 도달한 웨이포인트 인덱스

    // ===== [적 타입 설정] =====
    public enum EnemyType { Normal, Elite, Boss }       // 적 유닛 타입
    public EnemyType enemyType = EnemyType.Normal;      // 현재 적 타입
    public int rewardGold = 10;                         // 처치 시 보상 골드

    [Header("Health Bar")]
    public GameObject healthBarPrefab;                  // 체력바 프리팹
    private SimpleHealthBar healthBar;                  // 체력바 인스턴스

    private Animator animator;                          // 애니메이터
    private bool isDead = false;                        // 사망 여부
    public bool IsDead => isDead;                       // 외부 접근용 읽기 전용 속성

    // ===== [보스 특수 능력 설정] =====
    [Header("Boss Special Ability")]
    public float invincibleDuration = 1f;               // 무적 지속 시간
    public GameObject invincibleEffectPrefab;           // 무적 이펙트 프리팹
    public float stunRadius = 3f;                       // 기절 범위
    public float stunDuration = 2f;                     // 기절 지속 시간
    public GameObject stunEffectPrefab;                 // 기절 이펙트 프리팹

    private bool isInvincible = false;                  // 무적 여부

    // ========== [시작 시 초기화] ==========
    private void Start()
    {
        animator = GetComponent<Animator>();

        // 적 타입에 따른 보상 설정 및 보스 특수 루틴 시작
        switch (enemyType)
        {
            case EnemyType.Normal: rewardGold = 10; break;
            case EnemyType.Elite: rewardGold = 25; break;
            case EnemyType.Boss:
                rewardGold = 100;
                StartCoroutine(BossSpecialRoutine());
                break;
        }
    }

    // ========== [보스 특수 루틴: 5초마다 무작위 발동] ==========
    private IEnumerator BossSpecialRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(5f);
            int choice = Random.Range(0, 2);
            if (choice == 0)
                StartCoroutine(ActivateInvincibility());
            else
                StartCoroutine(ActivateStun());
        }
    }

    // ========== [무적 효과 실행] ==========
    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true;

        if (invincibleEffectPrefab != null)
        {
            GameObject fx = Instantiate(invincibleEffectPrefab, transform.position, Quaternion.identity, transform);
            Destroy(fx, invincibleDuration);
        }

        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private IEnumerator ActivateStun()
    {
        // 점프 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

        // 4회 반복 (0.65초 간격)
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.65f);

            // 이펙트 생성
            if (stunEffectPrefab != null)
            {
                GameObject fx = Instantiate(stunEffectPrefab, transform.position, Quaternion.identity);
                Destroy(fx, 1f);
            }

            // 매번 이펙트가 발생할 때마다 주변 유닛 기절 처리
            Collider[] hits = Physics.OverlapSphere(transform.position, stunRadius);
            foreach (var hit in hits)
            {
                MonoBehaviour[] comps = hit.GetComponents<MonoBehaviour>();
                foreach (var comp in comps)
                {
                    if (comp is IStunnable stunnable)
                    {
                        stunnable.ApplyStun(stunDuration);
                    }
                }
            }
        }

        // 애니메이션 복귀
        yield return new WaitForSeconds(0.5f);
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
    }


    // ========== [유닛 초기화] ==========
    public void Initialize(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
        transform.position = waypoints[0].position;
        waypointIndex = 0;

        health = maxHealth;
        speedMultiplier = 1f;
        isDead = false;

        if (healthBar == null && healthBarPrefab != null)
        {
            GameObject go = Instantiate(healthBarPrefab);
            healthBar = go.GetComponent<SimpleHealthBar>();
            healthBar.target = this.transform;
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(health, maxHealth);
            healthBar.gameObject.SetActive(true);
        }

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.SetBool("isWalking", true);
        }
    }

    // ========== [유닛 이동 처리] ==========
    private void Update()
    {
        if (isDead || waypoints == null || waypointIndex >= waypoints.Count) return;

        Transform targetPoint = waypoints[waypointIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        float adjustedSpeed = speed * speedMultiplier;

        transform.position += direction * adjustedSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else
        {
            if (animator != null)
                animator.SetBool("isWalking", false);
        }

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            waypointIndex++;

            if (waypointIndex >= waypoints.Count)
            {
                RoundManager rm = FindFirstObjectByType<RoundManager>();
                if (rm != null)
                {
                    rm.DecreaseLife();
                }

                OnDeath(false);
            }
        }
    }

    // ========== [피해 처리] ==========
    public void TakeDamage(float amount)
    {
        if (isDead || isInvincible) return;

        health -= amount;

        if (healthBar != null)
        {
            healthBar.SetHealth(health, maxHealth);
        }

        if (health <= 0)
        {
            OnDeath(true);
        }
    }

    // ========== [사망 처리] ==========
    private void OnDeath(bool killedByPlayer)
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
            animator.SetBool("isWalking", false);
        }

        if (killedByPlayer && GoldManager.instance != null)
        {
            GoldManager.instance.AddGold(rewardGold);
        }

        RoundManager rm = FindFirstObjectByType<RoundManager>();
        if (rm != null)
        {
            rm.NotifyEnemyKilled();
        }

        if (DeathEffectPool.instance != null)
        {
            GameObject fx = DeathEffectPool.instance.GetEffect();
            fx.transform.position = transform.position;
            fx.SetActive(true);
            DeathEffectPool.instance.PlayAndRelease(fx, 1f);
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        StartCoroutine(DeactivateAfterDelay(2f));
    }

    // ========== [비활성화 딜레이 처리] ==========
    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    // ========== [오브젝트 풀 재활성화 시 초기화] ==========
    private void OnEnable()
    {
        isDead = false;

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.SetHealth(maxHealth, maxHealth);
        }

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.SetBool("isWalking", true);
        }
    }
}

/// <summary>
/// 기절 가능한 오브젝트가 구현해야 할 인터페이스
/// </summary>
public interface IStunnable
{
    void ApplyStun(float duration);
}
