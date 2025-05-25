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
    public UnitType unitType;
    public float attackCooldown = 1f;
    public float damage = 10f;
    public GameObject projectilePrefab;

    // ========== [���� ���°�] ==========
    private float nextAttackTime;
    private UnitRangeDetector rangeDetector;
    private Animator animator;
    private bool isStunned = false;
    private float stunEndTime = 0f;
    private GameObject stunIndicatorInstance;

    // ========== [���� ȿ��] ==========
    [Header("Stun Visual Effect")]
    public GameObject stunIndicatorPrefab;

    // ========== [UI / ���� ����] ==========
    public string unitName;
    public Sprite unitIcon;
    public int goldCost;
    public float uiOffsetY = 2.5f;

    // ========== [����̵� ���� ����] ==========
    [Header("Druid Area Effect Settings")]
    public float druidDuration = 3f;
    public float druidTickInterval = 1f;
    public float druidEffectRadius = 1.5f;

    // ========== [��ȭ ����Ʈ �� ����] ==========
    [Header("Upgrade Visual Prefabs")]
    public GameObject[] upgradeEffectPrefabs;

    [Header("Upgrade Weapon Models")]
    public GameObject[] upgradeWeaponModels; // [0]: �⺻, [1]: 1�ܰ�, [2]: 2�ܰ�

    private List<GameObject> activeUpgradeEffects = new();
    public int upgradeLevel = 0;
    //===========[����]================
    [Header("Audio")]
    public AudioClip attackSound;  // ���� ���� �� ����� �Ҹ�
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("[BaseUnit] AudioSource�� ���� �����մϴ�.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        Vector3 fixedPosition = transform.position;
        fixedPosition.y = 0.05f;
        transform.position = fixedPosition;

        rangeDetector = GetComponentInChildren<UnitRangeDetector>();
        animator = GetComponent<Animator>();

        UpdateWeaponModel(0); // �⺻ ���� ����
    }

    void Update()
    {
        if (isStunned)
        {
            if (Time.time >= stunEndTime)
            {
                isStunned = false;
                if (stunIndicatorInstance != null) Destroy(stunIndicatorInstance);
                if (animator != null) animator.SetBool("isWalking", true);
            }
            return;
        }

        if (Time.time >= nextAttackTime && rangeDetector != null)
        {
            EnemyController target = FindClosestTarget();
            if (target != null && !target.IsDead)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                direction.y = 0f;
                if (direction != Vector3.zero) transform.forward = direction;

                if (animator != null)
                {
                    string trigger = $"Attack_{unitType.ToString().ToLower()}";
                    animator.SetTrigger(trigger);
                }

                switch (unitType)
                {
                    case UnitType.knight:
                        StartCoroutine(KnightAttackCoroutine());
                        break;
                    case UnitType.mage:
                        FireProjectile(target, true);
                        break;
                    case UnitType.druid:
                        SpawnDruidArea(target);
                        break;
                    case UnitType.archer:
                        FireProjectile(target, false);
                        break;
                }

                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    public void ApplyStun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;

        if (stunIndicatorPrefab != null && stunIndicatorInstance == null)
        {
            Vector3 pos = transform.position + Vector3.up * 2f;
            stunIndicatorInstance = Instantiate(stunIndicatorPrefab, pos, Quaternion.identity, transform);
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetTrigger("Stunned");
        }

        Debug.Log($"{gameObject.name} ������ {duration}�� ���� ������.");
    }

    EnemyController FindClosestTarget()
    {
        EnemyController closest = null;
        float minDist = float.MaxValue;

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

    void FireProjectile(EnemyController target, bool areaDamage)
    {
        if (projectilePrefab == null) return;

        PlayAttackSound();
        
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Initialize(target.transform.position, damage, areaDamage);
    }

    public void IncreaseStats()
    {
        if (upgradeLevel >= 2)
        {
            Debug.Log("�ִ� ��ȭ�� �����߽��ϴ�.");
            return;
        }

        upgradeLevel++;

        if (upgradeLevel <= 2) damage *= 1.5f;

        Debug.Log($"������ {upgradeLevel}�ܰ�� ��ȭ�Ǿ����ϴ�. ���ݷ�: {damage}");

        UpdateWeaponModel(upgradeLevel);

        int prefabIndex = upgradeLevel - 1;
        if (upgradeEffectPrefabs != null && prefabIndex < upgradeEffectPrefabs.Length)
        {
            GameObject fx = upgradeEffectPrefabs[prefabIndex];
            if (fx != null)
            {
                Vector3 pos = transform.position;
                pos.y = 0.05f;
                GameObject instance = Instantiate(fx, pos, Quaternion.identity);
                activeUpgradeEffects.Add(instance);
            }
        }
    }

    private void UpdateWeaponModel(int level)
    {
        for (int i = 0; i < upgradeWeaponModels.Length; i++)
        {
            if (upgradeWeaponModels[i] != null)
                upgradeWeaponModels[i].SetActive(i == level);
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

    private void SpawnDruidArea(EnemyController target)
    {
        if (projectilePrefab == null) return;

        Vector3 pos = target.transform.position;
        pos.y += 0.1f;

        GameObject field = Instantiate(projectilePrefab, pos, Quaternion.identity);
        StartCoroutine(DruidEffectCoroutine(field, pos));
    }

    private IEnumerator DruidEffectCoroutine(GameObject field, Vector3 position)
    {
        float elapsed = 0f;
        PlayAttackSound();
        while (elapsed < druidDuration)
        {
            Collider[] hits = Physics.OverlapSphere(position, druidEffectRadius);
            foreach (var hit in hits)
            {
                var enemy = hit.GetComponent<EnemyController>();
                if (enemy != null && !enemy.IsDead)
                {
                    enemy.TakeDamage(damage);
                }
            }

            yield return new WaitForSeconds(druidTickInterval);
            elapsed += druidTickInterval;
        }

        Destroy(field);
    }

    private IEnumerator KnightAttackCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.speed = 1.5f;
        yield return new WaitForSeconds(0.6f);

        if (projectilePrefab != null)
        {
            Vector3 spawnPos = transform.position + transform.forward * 1f;
            GameObject slash = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(transform.forward));
            PlayAttackSound();
            StartCoroutine(SlashArcEffect(slash, 2f, 120f, 0.15f));
            Destroy(slash, 0.5f);
        }

        FireMeleeArc();
    }

    private IEnumerator SlashArcEffect(GameObject effect, float radius, float angle, float duration)
    {
        float elapsed = 0f;
        float startAngle = -angle / 2f;
        float endAngle = angle / 2f;
        Vector3 center = transform.position;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * radius;

            effect.transform.position = center + transform.rotation * offset;
            effect.transform.rotation = Quaternion.LookRotation(transform.rotation * offset);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void FireMeleeArc()
    {
        float attackRadius = 2f;
        float arcAngle = 120f;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy != null && !enemy.IsDead)
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
    public int GetSellValue()
    {
        float baseRatio = 0.6f;
        float upgradeBonus = 0.2f; // ��ȭ 1ȸ�� 20%�� ����

        float ratio = baseRatio + (upgradeLevel * upgradeBonus);
        float rawPrice = goldCost * ratio;

        return Mathf.RoundToInt(rawPrice);
    }

    /// <summary>
    /// ��ȭ ��� ���: ��ȯ �ڽ�Ʈ�� 50%�� ���� ����
    /// </summary>
    public int GetUpgradeCost()
    {
        float costPerUpgrade = goldCost * 0.5f;
        return Mathf.RoundToInt(costPerUpgrade * (upgradeLevel + 1));
    }
    private void PlayAttackSound()
    {
        if (attackSound == null)
        {
            Debug.LogWarning("[AttackSound] AudioClip�� �������� �ʾҽ��ϴ�.");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogWarning("[AttackSound] AudioSource�� �����ϴ�.");
            return;
        }
        if (unitType == UnitType.druid)
        {
            audioSource.volume = 0.2f; // 0.0 ~ 1.0 ����
        }
        else if(unitType == UnitType.archer)
        {
            audioSource.volume = 1.5f;
        }
        else
        {
            audioSource.volume = 1.0f; // �⺻ ����
        }
        audioSource.PlayOneShot(attackSound);
    }
}
