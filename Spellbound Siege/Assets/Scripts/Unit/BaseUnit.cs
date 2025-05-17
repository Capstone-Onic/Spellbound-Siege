using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { knight, mage, druid }

public class BaseUnit : MonoBehaviour
{
    public UnitType unitType;
    public float attackCooldown = 1f;
    public float damage = 10f;
    public GameObject projectilePrefab;

    private float nextAttackTime;
    private UnitRangeDetector rangeDetector;
    private Animator animator;

    public string unitName;
    public Sprite unitIcon;
    public int goldCost;

    [Header("Druid Area Effect Settings")]
    public float druidDuration = 3f;             // 효과 지속 시간
    public float druidTickInterval = 1f;         // 데미지 간격
    public float druidEffectRadius = 1.5f;       // 범위 반경

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        rangeDetector = GetComponentInChildren<UnitRangeDetector>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime && rangeDetector != null)
        {
            EnemyController target = FindClosestTarget();

            if (target != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                direction.y = 0f;
                if (direction != Vector3.zero)
                {
                    transform.forward = direction;
                }

                if (animator != null)
                {
                    if (unitType == UnitType.knight)
                        animator.SetTrigger("Attack_knight");
                    else
                        animator.SetTrigger("Attack_mage"); // mage, druid 공유
                }

                if (unitType == UnitType.knight)
                {
                    target.TakeDamage(damage);
                }
                else if (unitType == UnitType.mage)
                {
                    FireProjectile(target);
                }
                else if (unitType == UnitType.druid)
                {
                    SpawnDruidArea(target);
                }

                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    EnemyController FindClosestTarget()
    {
        EnemyController closest = null;
        float minDist = float.MaxValue;

        rangeDetector.enemiesInRange.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy);

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

    void FireProjectile(EnemyController target)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Initialize(target.transform.position, damage);
    }
}