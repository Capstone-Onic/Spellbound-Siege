using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public class AreaEffectTrigger : MonoBehaviour
{
    public Card sourceCard;
    public float duration = 3f;           // ��ü ���� ���ӽð�
    public float tickInterval = 0.5f;     // ���� �˻� ����

    // ���ο찡 �̹� �ɸ� ���� ���
    private HashSet<EnemyController> affected = new HashSet<EnemyController>();

    private void Start()
    {
        StartCoroutine(ApplyEffectOverTime());
    }

    private IEnumerator ApplyEffectOverTime()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            ApplyToNearbyEnemies();
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Destroy(gameObject);
    }

    private void ApplyToNearbyEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sourceCard.effectRadius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy == null) continue;

            string name = sourceCard.cardName.ToLower();

            if (name.Contains("fire wall"))
            {
                // ȭ�� ���� ���� (����ó�� �� ƽ����)
                enemy.StartCoroutine(ApplyBurn(enemy, 6f, 0.5f, sourceCard.effectDuration));
            }
            else if (name.Contains("ice spear"))
            {
                // Ice Spear: ���� ���ο찡 �� �ɸ� �����Ը�
                if (!affected.Contains(enemy))
                {
                    affected.Add(enemy);
                    enemy.StartCoroutine(ApplySlow(enemy, 0.5f, sourceCard.effectDuration));
                }
            }
        }
    }

    private IEnumerator ApplyBurn(EnemyController enemy, float damage, float interval, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (enemy == null) yield break;
            enemy.TakeDamage(damage);
            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator ApplySlow(EnemyController enemy, float rate, float duration)
    {
        if (enemy == null) yield break;

        float originalSpeed = enemy.speedMultiplier;
        enemy.speedMultiplier = rate;

        // effectDuration��ŭ ���
        yield return new WaitForSeconds(duration);

        // ���� ����
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }
}
