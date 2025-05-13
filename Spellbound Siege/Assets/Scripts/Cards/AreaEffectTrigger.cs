using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public class AreaEffectTrigger : MonoBehaviour
{
    public Card sourceCard;
    public float duration = 3f;           // 전체 영역 지속시간
    public float tickInterval = 0.5f;     // 영역 검사 간격

    // 슬로우가 이미 걸린 적을 기록
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
                // 화염 지속 피해 (기존처럼 매 틱마다)
                enemy.StartCoroutine(ApplyBurn(enemy, 6f, 0.5f, sourceCard.effectDuration));
            }
            else if (name.Contains("ice spear"))
            {
                // Ice Spear: 아직 슬로우가 안 걸린 적에게만
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

        // effectDuration만큼 대기
        yield return new WaitForSeconds(duration);

        // 느림 해제
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }
}
