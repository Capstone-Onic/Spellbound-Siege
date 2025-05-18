using UnityEngine;

public class Projectile : MonoBehaviour
{
    // ========== [����ü ���� ������] ==========

    public float speed = 10f;         // ����ü�� ��ǥ �������� �̵��ϴ� �ӵ� (����: �ʴ� ����)
    public float aoeRadius = 2f;      // ���� ������ ���, ���� �ݰ� (����: �Ÿ�)
    public float launchDelay = 0.2f;  // ����ü �߻������ ���� �ð� (�ִϸ��̼ǰ��� ��ũ �����)

    // ========== [���� ����: �ʱ�ȭ �� ������] ==========

    private Vector3 targetPos;        // ����ü�� ���� ���� ���� (Ÿ�� ��ġ)
    private float damage;             // ����ü�� ���ϴ� ���ط�
    private bool isAreaDamage = true; // ���� ���ظ� ������ ����ü���� ����

    // �Ÿ� ��� ����ȭ�� ���� ���� �� (��Ʈ ��� ����)
    private float aoeRadiusSqr;            // aoeRadius�� ������
    private float explodeThresholdSqr = 0.01f; // ��ǥ ���� �Ǵ� ���� �Ÿ��� ������

    private bool launched = false;         // �߻簡 �Ǿ����� ���� (������ ���� true�� ��ȯ)

    // ========================================
    // ����ü�� �ܺ� �ʱ�ȭ �Լ�
    // ========================================
    public void Initialize(Vector3 targetPosition, float dmg, bool areaDamage)
    {
        targetPos = targetPosition;
        damage = dmg;
        isAreaDamage = areaDamage;
        aoeRadiusSqr = aoeRadius * aoeRadius;

        // ?? ����ü�� ��ǥ �������� ȸ��
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        Invoke(nameof(Launch), launchDelay);
    }

    // ========================================
    // ���� ����ü �߻縦 �����ϴ� �Լ� (������ �� ȣ���)
    // ========================================
    private void Launch()
    {
        launched = true;
    }

    // ========================================
    // �� �����Ӹ��� ����Ǵ� ������Ʈ �Լ�
    // ========================================
    void Update()
    {
        // ���� �߻簡 �� �Ǿ����� �ƹ��͵� ���� ����
        if (!launched) return;

        // ��ǥ ������ ���� ���� �ӵ��� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // ��ǥ ������ ���� �����ߴ��� Ȯ�� (���� �Ÿ��� ��)
        if ((transform.position - targetPos).sqrMagnitude < explodeThresholdSqr)
        {
            Explode(); // ���������� ���� ó��
        }
    }

    // ========================================
    // ���� ó�� �Լ� (���� �Ǵ� ���� ���� ó�� �� �ı�)
    // ========================================
    void Explode()
    {
        if (isAreaDamage)
        {
            // ���� ����: ���� ������ �߽����� aoeRadius �Ÿ� �� ��� ���� Ž��
            Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
            foreach (Collider hit in hits)
            {
                // �� �±װ� ���� ������Ʈ�� ���͸�
                if (hit.CompareTag("Enemy"))
                {
                    EnemyController enemy = hit.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage); // ���� ����
                    }
                }
            }
        }
        else
        {
            // ���� ����: ���� ���� �ݰ�(0.2f) ������ �� Ž�� (����)
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.2f);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    EnemyController enemy = hit.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage); // ���� ������
                        break; // �ϳ��� ������ ����
                    }
                }
            }
        }

        // ���� ����Ʈ/�ִϸ��̼��� ���� ó������ �ʴ� ���, ����ü ������Ʈ ����
        Destroy(gameObject);
    }
}
