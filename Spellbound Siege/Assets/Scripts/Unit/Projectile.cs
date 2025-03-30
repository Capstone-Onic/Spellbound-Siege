using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;         // ����ü �ӵ�
    public float aoeRadius = 2f;      // ���� ���� �ݰ�
    private Vector3 targetPos;        // ��ǥ ����
    private float damage;             // ���ط�

    private float aoeRadiusSqr;       // ���� ���� �Ÿ� ������ (����ȭ��)
    private float explodeThresholdSqr = 0.01f; // ��ǥ ���� ������ �Ÿ� ������

    // ����ü �ʱ� ����
    public void Initialize(Vector3 targetPosition, float dmg)
    {
        targetPos = targetPosition;
        damage = dmg;
        aoeRadiusSqr = aoeRadius * aoeRadius;
    }

    void Update()
    {
        // ��ǥ �������� ���� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // ��ǥ ������ �����ߴ��� Ȯ�� (��Ȯ�� ��ġ �� ��� �Ÿ� ������ ��)
        if ((transform.position - targetPos).sqrMagnitude < explodeThresholdSqr)
        {
            Explode();
        }
    }

    // ���� ó��: �ֺ� ������ ���� ����
    void Explode()
    {
        // ���� ���� �������� ��ü ���� �� �ݶ��̴� Ž��
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (Collider hit in hits)
        {
            // �� �±� Ȯ�� �� ������ ����
            if (hit.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        // ����ü ����
        Destroy(gameObject);
    }
}
