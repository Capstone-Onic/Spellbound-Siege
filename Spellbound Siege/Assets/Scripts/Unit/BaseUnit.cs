using UnityEngine;

// ���� Ÿ���� �����ϴ� ������
public enum UnitType { Melee, Ranged }

public class BaseUnit : MonoBehaviour
{
    public UnitType unitType;                   // ���� Ÿ�� ����
    public float attackCooldown = 1f;           // ���� ��Ÿ��
    public float damage = 10f;                  // ���ݷ�
    public GameObject projectilePrefab;         // ���Ÿ��� ����ü ������

    private float nextAttackTime;               // ���� ���� ���� �ð�
    private UnitRangeDetector rangeDetector;    // �� ������
    private Animator animator;                  // �ִϸ�����    

    // ���� �� rangeDetector ������Ʈ ã��
    void Start()
    {
        rangeDetector = GetComponentInChildren<UnitRangeDetector>();
    }

    void Update()
    {
        // ��Ÿ���� ������, �� �����Ⱑ ������ ��
        if (Time.time >= nextAttackTime && rangeDetector != null)
        {
            EnemyController target = FindClosestTarget(); // ���� �� ���� ����� �� ã��

            if (target != null)
            {
                // �ٰŸ� �����̸� ��� ������
                if (unitType == UnitType.Melee)
                {
                    target.TakeDamage(damage);
                }
                // ���Ÿ� �����̸� ����ü �߻�
                else
                {
                    FireProjectile(target);
                }

                // ���� ���� �ð� ����
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    // ���� �� ���� ����� ���� ã�� �Լ�
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

    // ����ü�� �����Ͽ� ���� ���� �߻�
    void FireProjectile(EnemyController target)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.Initialize(target.transform.position, damage);
    }
}
