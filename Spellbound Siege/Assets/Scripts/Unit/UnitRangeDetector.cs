using System.Collections.Generic;
using UnityEngine;

// ���� ���� ���� ���� �����ϴ� ������Ʈ
public class UnitRangeDetector : MonoBehaviour
{
    // ���� ������ ���� �ִ� �� ����Ʈ
    public List<EnemyController> enemiesInRange = new List<EnemyController>();

    // ���� ������ ������ �� ȣ��
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy); // ����Ʈ�� �߰�
            }
        }
    }

    // ���� �������� ������ �� ȣ��
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy); // ����Ʈ���� ����
            }
        }
    }
}
