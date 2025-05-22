using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public class AreaEffectTrigger : MonoBehaviour
{
    public Card sourceCard;           // 사용된 카드 정보
    public float duration = 3f;       // 전체 지속 시간 (초)
    public float tickInterval = 0.2f; // ApplyEffect 호출 간격 (초)

    // 이미 처리한 적을 다시 처리하지 않기 위한 집합
    private HashSet<EnemyController> affectedEnemies = new HashSet<EnemyController>();

    private void Start()
    {
        if (sourceCard == null)
        {
            Debug.LogError("[AreaEffectTrigger] sourceCard가 설정되지 않았습니다.");
            Destroy(gameObject);
            return;
        }

        StartCoroutine(ApplyEffectOverTime());
    }

    // duration 동안 tickInterval마다 ApplyEffectToEnemiesInRange 실행
    private IEnumerator ApplyEffectOverTime()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            ApplyEffectToEnemiesInRange();
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Destroy(gameObject);
    }

    private void ApplyEffectToEnemiesInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sourceCard.effectRadius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy == null || affectedEnemies.Contains(enemy))
                continue;

            affectedEnemies.Add(enemy);

            // Ice 계열 카드 (Ice Spear, Blizzard 등)
            if (sourceCard.cardType.Contains(Card.CardType.Ice))
            {
                // instantDamage: IceSpear = 0, Blizzard = 30 (SO에서 설정된 damage 값)
                float instantDamage = sourceCard.damage;
                StartCoroutine(ApplyDamageAndSlow(
                    enemy,
                    instantDamage,
                    0.5f,                           // 슬로우 배율
                    sourceCard.statusEffectDuration // IceSpear=3, Blizzard=5 등
                ));
            }
            // Fire 계열 카드 (지속 도트)
            else if (sourceCard.cardType.Contains(Card.CardType.Fire))
            {
                float instantDamage = sourceCard.damage;
                StartCoroutine(ApplyBurnDamage(
                    enemy,
                    instantDamage,                // 틱당 데미지
                    0.5f,                         // 틱 간격
                    sourceCard.effectDuration     // SO에 설정된 지속시간
                ));
            }
            // 필요하다면 다른 속성 분기 추가…
        }
    }

    // 즉시 대미지 + 슬로우를 처리하고, duration 후 슬로우 해제
    private IEnumerator ApplyDamageAndSlow(
        EnemyController enemy,
        float instantDamage,
        float slowMultiplier,
        float duration)
    {
        // 즉시 대미지
        if (instantDamage > 0f)
            enemy.TakeDamage(instantDamage);

        // 슬로우 적용
        float originalSpeed = enemy.speedMultiplier;
        enemy.speedMultiplier = slowMultiplier;

        // 유지 시간 대기
        yield return new WaitForSeconds(duration);

        // 속도 복구
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }

    
    // 일정 시간 동안 interval 간격으로 데미지
    private IEnumerator ApplyBurnDamage(
        EnemyController enemy,
        float damage,
        float interval,
        float duration)
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
}