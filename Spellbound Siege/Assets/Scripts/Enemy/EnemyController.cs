using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 유닛의 이동, 체력 관리, 사망 처리, 특수 능력 등을 담당하는 스크립트
/// </summary>
public class EnemyController : MonoBehaviour
{
    // ===== [일반 설정] =====
    public float speed = 3f;
    public float maxHealth = 100f;
    public float health;
    public float speedMultiplier = 1f;
    public GameObject deathEffectPrefab;

    private List<Transform> waypoints;
    private int waypointIndex = 0;

    public enum EnemyType { Normal, Elite, Boss }
    public EnemyType enemyType = EnemyType.Normal;
    public int rewardGold = 10;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    private SimpleHealthBar healthBar;

    private Animator animator;
    private bool isDead = false;
    public bool IsDead => isDead;

    // ===== [보스 특수 능력 설정] =====
    [Header("Boss Special Ability")]
    public float invincibleDuration = 1f;
    public GameObject invincibleEffectPrefab;
    public float stunRadius = 3f;
    public float stunDuration = 2f;
    public GameObject stunEffectPrefab;

    private bool isInvincible = false;

    // ===== [데미지 팝업 설정 추가] =====
    [Header("Damage Popup")]
    public GameObject damagePopupPrefab; // 인스펙터에서 연결

    // ========== [시작 시 초기화] ==========
    private void Start()
    {
        animator = GetComponent<Animator>();

        switch (enemyType)
        {
            case EnemyType.Boss:
                rewardGold = 100;
                StartCoroutine(BossSpecialRoutine());
                break;
        }
    }

    // ========== [보스 특수 루틴: 무작위 발동] ==========
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
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.65f);

            if (stunEffectPrefab != null)
            {
                GameObject fx = Instantiate(stunEffectPrefab, transform.position, Quaternion.identity);
                Destroy(fx, 1f);
            }

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

        // ✅ 데미지 숫자 팝업 생성
        ShowDamagePopup(amount);

        if (health <= 0)
        {
            OnDeath(true);
        }
    }

    // ✅ 데미지 팝업 UI 표시 함수
    private void ShowDamagePopup(float amount)
    {
        if (damagePopupPrefab == null) return;

        Vector3 popupPos = transform.position + new Vector3(0, 2f, 0.5f); // 머리 위
        GameObject popup = Instantiate(damagePopupPrefab, popupPos, Quaternion.identity);


        popup.transform.Rotate(70, 0, 0);

        DamagePopup popupScript = popup.GetComponent<DamagePopup>();
        if (popupScript != null)
        {
            popupScript.Setup(amount);
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
