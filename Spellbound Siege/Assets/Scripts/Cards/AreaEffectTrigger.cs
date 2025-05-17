using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public class AreaEffectTrigger : MonoBehaviour
{
    public Card sourceCard;
    public float duration = 3f;
    public float tickInterval = 0.2f;

    [Header("Visual Effects")]
    public GameObject iceSlowEffectPrefab;   // Ice 카드 효과 이펙트
    public GameObject fireBurnEffectPrefab;  // Fire 카드 효과 이펙트

    private HashSet<EnemyController> affectedEnemies = new();
    private Dictionary<EnemyController, Coroutine> restoreCoroutines = new();
    private Dictionary<EnemyController, GameObject> activeEffects = new();

    private void Start()
    {
        StartCoroutine(ApplyEffectOverTime());
    }

    private IEnumerator ApplyEffectOverTime()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ApplyEffectToEnemiesInRange();
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        // Ice 효과 클린업
        foreach (var enemy in restoreCoroutines.Keys)
        {
            if (enemy != null)
                enemy.speedMultiplier = 1f;
        }

        foreach (var fx in activeEffects.Values)
        {
            if (fx != null) Destroy(fx);
        }

        affectedEnemies.Clear();
        restoreCoroutines.Clear();
        activeEffects.Clear();

        Destroy(gameObject);
    }

    private void ApplyEffectToEnemiesInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sourceCard.effectRadius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy == null || affectedEnemies.Contains(enemy)) continue;

            affectedEnemies.Add(enemy);

            // Ice 효과: 슬로우 + 이펙트
            if (sourceCard.cardType.Count > 0 && sourceCard.cardType[0] == Card.CardType.Ice)
            {
                enemy.speedMultiplier = 0.5f;

                if (iceSlowEffectPrefab != null)
                {
                    GameObject fx = Instantiate(iceSlowEffectPrefab, enemy.transform);
                    float height = enemy.GetComponent<Renderer>().bounds.size.y;
                    fx.transform.localPosition = new Vector3(0, height * 0.3f, 0);
                    activeEffects[enemy] = fx;
                }

                Coroutine co = StartCoroutine(RestoreSpeedAfterDelay(enemy, sourceCard.effectDuration));
                restoreCoroutines[enemy] = co;
            }

            // Fire 효과: 화상 + 이펙트
            else if (sourceCard.cardType.Contains(Card.CardType.Fire))
            {
                StartCoroutine(ApplyBurnDamage(enemy, 6f, 0.5f, sourceCard.effectDuration));

                if (fireBurnEffectPrefab != null)
                {
                    GameObject fx = Instantiate(fireBurnEffectPrefab, enemy.transform);
                    float height = enemy.GetComponent<Renderer>().bounds.size.y;
                    fx.transform.localPosition = new Vector3(0, height * 0.3f, 0);
                    Destroy(fx, sourceCard.effectDuration);
                }
            }
            // 다른 속성은 무시
        }
    }

    private IEnumerator RestoreSpeedAfterDelay(EnemyController enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemy != null)
            enemy.speedMultiplier = 1f;

        if (activeEffects.TryGetValue(enemy, out GameObject fx))
        {
            if (fx != null) Destroy(fx);
            activeEffects.Remove(enemy);
        }

        restoreCoroutines.Remove(enemy);
    }

    private IEnumerator ApplyBurnDamage(EnemyController enemy, float damage, float interval, float duration)
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