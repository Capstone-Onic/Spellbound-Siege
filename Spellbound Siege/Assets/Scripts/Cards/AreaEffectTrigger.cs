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
    public GameObject iceSlowEffectPrefab;
    public GameObject fireBurnEffectPrefab;

    private HashSet<EnemyController> affectedEnemies = new();
    private Dictionary<EnemyController, Coroutine> restoreCoroutines = new();
    private Dictionary<EnemyController, GameObject> activeEffects = new();

    private void Start()
    {
        if (sourceCard == null)
        {
            Debug.LogError("[AreaEffectTrigger] sourceCard�� �������� �ʾҽ��ϴ�. ����Ʈ ������ �ߴܵ˴ϴ�.");
            return;
        }

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

            // Ice ī��: �̵� �ӵ� ���� + ����Ʈ
            if (sourceCard.cardType.Count > 0 && sourceCard.cardType[0] == Card.CardType.Ice)
            {
                enemy.speedMultiplier = 0.5f;

                if (iceSlowEffectPrefab != null)
                {
                    GameObject fx = Instantiate(iceSlowEffectPrefab, enemy.transform);
                    Renderer rend = enemy.GetComponent<Renderer>();
                    float height = rend != null ? rend.bounds.size.y : 2f;
                    fx.transform.localPosition = new Vector3(0, height * 0.3f, 0);
                    activeEffects[enemy] = fx;
                }

                Coroutine co = StartCoroutine(RestoreSpeedAfterDelay(enemy, sourceCard.statusEffectDuration));
                restoreCoroutines[enemy] = co;
            }

            // Fire ī��: ��Ʈ ������ + ����Ʈ
            else if (sourceCard.cardType.Contains(Card.CardType.Fire))
            {
                // ��Ʈ �������� enemy ���ؿ��� ����
                enemy.StartCoroutine(ApplyBurnDamage(enemy, 6f, 0.5f, sourceCard.statusEffectDuration));

                // Burn ����Ʈ�� enemy ������ �ٰ�, 3�� �� �ڵ� ���ŵ�
                if (fireBurnEffectPrefab != null)
                {
                    GameObject fx = Instantiate(fireBurnEffectPrefab, enemy.transform);
                    Renderer rend = enemy.GetComponent<Renderer>();
                    float height = rend != null ? rend.bounds.size.y : 2f;
                    fx.transform.localPosition = new Vector3(0, height * 0.3f, 0);
                    Destroy(fx, sourceCard.statusEffectDuration);
                }
            }
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
        int repeatCount = Mathf.FloorToInt(duration / interval);
        for (int i = 0; i < repeatCount; i++)
        {
            if (enemy == null) yield break;
            enemy.TakeDamage(damage);
            yield return new WaitForSeconds(interval);
        }
    }
}