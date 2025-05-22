using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ Ÿ���� �����ϴ� ������ (�� Ÿ�Ը��� ���� ����� �ٸ�)
/// </summary>
public enum UnitType { knight, mage, druid, archer }

/// <summary>
/// �� ���ֵ��� ���� ���� �� ������ ����ϴ� ���� Ŭ����
/// </summary>
public class BaseUnit : MonoBehaviour, IStunnable
{
    // ========== [�⺻ ���� ������] ==========

    public UnitType unitType;              // ������ Ÿ�� ���� (���, ������ ��)
    public float attackCooldown = 1f;      // ���� ��Ÿ��
    public float damage = 10f;             // ���ݷ�
    public GameObject projectilePrefab;    // ���ݿ� ����ü �Ǵ� ����Ʈ ������ (��絵 �̰� ���)

    // ========== [���� ���°�] ==========
    private float nextAttackTime;                  // ���� ���� ���� �ð�
    private UnitRangeDetector rangeDetector;       // ���� �� �� Ž����
    private Animator animator;                     // �ִϸ����� ������Ʈ

    private bool isStunned = false;                // ���� ���� ����
    private float stunEndTime = 0f;                // ���� ���� �ð�
    private GameObject stunIndicatorInstance;      // ���� ���� ����Ʈ �ν��Ͻ�

    // ========== [���� �ð� ȿ�� ������] ==========
    [Header("Stun Visual Effect")]
    public GameObject stunIndicatorPrefab;         // ���� �� ���� ���� ������ ����Ʈ ������

    // ========== [UI / ���� ����] ==========
    public string unitName;        // ���� �̸�
    public Sprite unitIcon;        // ���� ������
    public int goldCost;           // ���� ��ȯ ���

    // ========== [����̵� ���� ����] ==========
    [Header("Druid Area Effect Settings")]
    public float druidDuration = 3f;           // ����̵� ȿ�� ���� �ð�
    public float druidTickInterval = 1f;       // ƽ ���� (�� �ʸ��� �������� �ִ���)
    public float druidEffectRadius = 1.5f;     // ����̵� ȿ�� ���� �ݰ�

    private List<GameObject> activeUpgradeEffects = new(); // ��ȭ �� �����ϴ� ����Ʈ

    void Start()
    {
        // ������ �ٶ󺸴� ���� �⺻�� ���� (�ڸ� ����)
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // Y ��ġ�� ���� (�����ϰų� ������ �ʰ�)
        Vector3 fixedPosition = transform.position;
        fixedPosition.y = 0.05f;
        transform.position = fixedPosition;

        // ���� ������ �� �ִϸ����� �ʱ�ȭ
        rangeDetector = GetComponentInChildren<UnitRangeDetector>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ���� ������ ��� �ൿ �ߴ�
        if (isStunned)
        {
            if (Time.time >= stunEndTime)
            {
                isStunned = false;

                // ���� ����Ʈ ����
                if (stunIndicatorInstance != null)
                {
                    Destroy(stunIndicatorInstance);
                }

                if (animator != null)
                    animator.SetBool("isWalking", true);
            }
            return;
        }

        // ���� ��Ÿ���� ������ �����Ⱑ �����ϸ� ���� ����
        if (Time.time >= nextAttackTime && rangeDetector != null)
        {
            EnemyController target = FindClosestTarget(); // ���� ����� �� ã��

            if (target != null && !target.IsDead) // ���� ���� ����
            {
                // ������ ���� �ٶ󺸰� ȸ��
                Vector3 direction = (target.transform.position - transform.position).normalized;
                direction.y = 0f;
                if (direction != Vector3.zero)
                    transform.forward = direction;

                // ���� Ÿ�Կ� ���� �ִϸ��̼� Ʈ���� ����
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

                // ���� Ÿ�Ժ� ���� ����
                switch (unitType)
                {
                    case UnitType.knight:
                        StartCoroutine(KnightAttackCoroutine()); // ��� ���� ���� �ݿ� ����
                        break;
                    case UnitType.mage:
                        FireProjectile(target, areaDamage: true); // ������� ���� ����ü
                        break;
                    case UnitType.druid:
                        SpawnDruidArea(target); // ����̵�� �ٴڿ� ����
                        break;
                    case UnitType.archer:
                        FireProjectile(target, areaDamage: false); // �ü��� ���� Ÿ�� ����ü
                        break;
                }

                nextAttackTime = Time.time + attackCooldown; // ���� ���� �ð� ����
            }
        }
    }

    /// <summary>
    /// ���� ȿ�� ���� �Լ� (�������� ������ �� ȣ���)
    /// </summary>
    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;

        // ���� ����Ʈ ǥ��
        if (stunIndicatorPrefab != null && stunIndicatorInstance == null)
        {
            Vector3 indicatorPos = transform.position + Vector3.up * 2f;
            stunIndicatorInstance = Instantiate(stunIndicatorPrefab, indicatorPos, Quaternion.identity, transform);
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetTrigger("Stunned"); // Animator�� �ش� Ʈ���Ű� �־�� ��
        }

        Debug.Log($"{gameObject.name} ������ {duration}�� ���� ������.");
    }

    /// <summary>
    /// ���� ����� ���� ã�� ��ȯ
    /// </summary>
    EnemyController FindClosestTarget()
    {
        EnemyController closest = null;
        float minDist = float.MaxValue;

        // ����Ʈ���� �׾��ų� ��Ȱ��ȭ�� �� ����
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
    /// ����ü �߻� ó�� (������ / �ü���)
    /// </summary>
    void FireProjectile(EnemyController target, bool areaDamage)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Initialize(target.transform.position, damage, areaDamage);
    }

    // ========== [��ȭ ���� ����] ==========
    private int upgradeLevel = 0; // 0 = �⺻, 1 = 1�ܰ�, 2 = 2�ܰ�, 3 = ����

    [Header("Upgrade Visual Prefabs")]
    public GameObject[] upgradeEffectPrefabs; // [0]=1�ܰ�, [1]=2�ܰ�, [2]=3�ܰ� �����յ�

    /// <summary>
    /// ���� ��ȭ ��� (�ִ� 3�ܰ���� ��ȭ, �ð��� ������ �߰� ����)
    /// </summary>
    public void IncreaseStats()
    {
        if (upgradeLevel >= 2)
        {
            Debug.Log("�ִ� ��ȭ�� �����߽��ϴ�.");
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

        Debug.Log($"������ {upgradeLevel}�ܰ�� ��ȭ�Ǿ����ϴ�. ���� ���ݷ�: {damage}, ��Ÿ��: {attackCooldown}");

        // ������ �ε����� 0���� ����
        int prefabIndex = upgradeLevel - 1;
        if (upgradeEffectPrefabs != null && prefabIndex < upgradeEffectPrefabs.Length)
        {
            GameObject effect = upgradeEffectPrefabs[prefabIndex];
            if (effect != null)
            {
                Vector3 effectPos = transform.position;
                effectPos.y = 0.05f; // �ٴڿ� ��¦ ����
                GameObject instance = Instantiate(effect, effectPos, Quaternion.identity);
                activeUpgradeEffects.Add(instance); // ����Ʈ�� ����
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
    /// ����̵� ���� ���� ����
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
    /// ����̵� ���� ȿ�� ���� �ð� ���� �ֱ��� ������
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
    /// ��� ���� ����: ����Ʈ�� �����ϰ� ȸ����Ų �� ���� ������ ó��
    /// </summary>
    private IEnumerator KnightAttackCoroutine()
    {
        yield return new WaitForSeconds(1f); // �ִϸ��̼� Ÿ�̹� ����

        if (projectilePrefab != null)
        {
            // ���� ���ʿ��� ������ ����
            Vector3 spawnPos = transform.position + transform.forward * 1f;
            GameObject slash = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(transform.forward));

            // �������� �ݿ� ��η� ȸ����Ű�� �ڷ�ƾ ����
            StartCoroutine(SlashArcEffect(slash, 2f, 120f, 0.3f));

            // �ð��� ȿ���� ���� �ð� �� ����
            Destroy(slash, 0.5f);
        }

        FireMeleeArc(); // ���� ������ ó��
    }

    /// <summary>
    /// �������� �ݿ� ��η� ȸ����Ű�� �̵���Ű�� �ð� ȿ��
    /// </summary>
    private IEnumerator SlashArcEffect(GameObject effect, float radius, float angle, float duration)
    {
        float elapsed = 0f;
        float startAngle = -angle / 2f;
        float endAngle = angle / 2f;

        Vector3 center = transform.position; // ȸ�� �߽�

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * radius;

            // ��ġ�� ȸ�� ��� ĳ���� ���� �������� ����
            effect.transform.position = center + transform.rotation * offset;
            effect.transform.rotation = Quaternion.LookRotation(transform.rotation * offset);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// �ݿ� ���� �� ������ ���� �������� �ִ� �Լ�
    /// </summary>
    private void FireMeleeArc()
    {
        float attackRadius = 2f; // ���� ������
        float arcAngle = 120f;     // ���� �ݿ� ����

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

