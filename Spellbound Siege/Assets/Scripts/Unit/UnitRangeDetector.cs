using System.Collections.Generic;
using UnityEngine;

// 범위 내에 들어온 적을 추적하는 컴포넌트
public class UnitRangeDetector : MonoBehaviour
{
    // 현재 범위에 들어와 있는 적 리스트
    public List<EnemyController> enemiesInRange = new List<EnemyController>();

    // 적이 범위에 들어왔을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy); // 리스트에 추가
            }
        }
    }

    // 적이 범위에서 나갔을 때 호출
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy); // 리스트에서 제거
            }
        }
    }
}
