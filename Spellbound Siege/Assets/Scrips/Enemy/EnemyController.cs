// ���� ���� �̵��� ���¸� ����ϴ� Ŭ����
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f; // �⺻ �̵� �ӵ�
    public float health = 100f; // ���� �ִ� ü��
    public float speedMultiplier = 1f; // ���� ȿ���� ���� �ӵ� ��ȭ (��: ����, ����)

    private List<Transform> waypoints; // ��� ����
    private int waypointIndex = 0; // ���� ��ǥ�� �ϴ� ��������Ʈ �ε���

    public void Initialize(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
        transform.position = waypoints[0].position; // ����� ���� �������� �̵�
    }

    private void Update()
    {
        if (waypoints == null || waypointIndex >= waypoints.Count) return;

        // ���� ��ǥ ��������Ʈ�� �̵��ϱ�
        Transform targetPoint = waypoints[waypointIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        float adjustedSpeed = speed * speedMultiplier; // ���� ȿ�� ����� �ӵ� ���
        transform.position += direction * adjustedSpeed * Time.deltaTime;

        // ��ǥ ������ �����ߴ��� Ȯ��
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Count)
            {
                OnDeath(false); // ������ ���� �� �� ���� (óġ���� ���� ���)
            }
        }
    }

    // ���� ���ظ� �Դ� �Լ�
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            OnDeath(true); // ���� �׾��� ��� ó��
        }
    }

    // ���� ���ŵ� �� ó���ϴ� �Լ�
    private void OnDeath(bool killedByPlayer)
    {
        if (killedByPlayer)
        {
            // ���, ����ġ ���� ���� ó�� ����
            
        }
        Destroy(gameObject); // �� ������Ʈ ����
    }
}